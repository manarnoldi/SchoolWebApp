using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SchoolWebApp.Core.DTOs.Dashboard;
using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.Enums;
using Project.Infrastructure.Data;

namespace SchoolWebApp.API.Controllers.Dashboard
{
    /// <summary>
    /// Dashboard-specific aggregation endpoints. Each method here exists to
    /// collapse what would otherwise be a fan-out of dozens of client-side
    /// HTTP requests into one server-side call. Cuts both the round-trip
    /// count and the load the shared-host rate limiter sees.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMemoryCache _cache;
        private readonly ILogger<DashboardController> _logger;

        // Short TTL — dashboard data is allowed to lag the canonical scores by
        // a few minutes in exchange for the rate-limit / round-trip savings.
        // Five minutes is short enough that fresh exam entry feels responsive
        // (one refresh away) and long enough to absorb several dashboard
        // reloads by the same user without re-running the aggregation.
        private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5);

        public DashboardController(
            ApplicationDbContext db,
            IMemoryCache cache,
            ILogger<DashboardController> logger)
        {
            _db = db;
            _cache = cache;
            _logger = logger;
        }

        /// <summary>
        /// Single-request replacement for the dashboard's "Class Exam
        /// Performance" widget. Previously the client fired:
        ///   * 1 exam-search per class
        ///   * 1 student-class lookup per class
        ///   * 1 results lookup per exam per class
        /// On a populated school that's 100+ requests and routinely tripped
        /// site4now's perimeter rate limiter into 403s without CORS headers.
        /// We now do everything server-side in 4 EF queries (exams, students,
        /// results, class metadata) and cache for 5 minutes.
        /// </summary>
        [HttpGet("classExamSummary")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ClassExamSummaryDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetClassExamSummary(
            [FromQuery] int academicYearId,
            [FromQuery] int curriculumId,
            [FromQuery] int sessionId,
            [FromQuery] int examTypeId)
        {
            if (academicYearId <= 0 || curriculumId <= 0 || sessionId <= 0 || examTypeId <= 0)
                return BadRequest("academicYearId, curriculumId, sessionId and examTypeId are all required.");

            var cacheKey = $"dash.classExamSummary.{academicYearId}.{curriculumId}.{sessionId}.{examTypeId}";
            if (_cache.TryGetValue<List<ClassExamSummaryDto>>(cacheKey, out var cached) && cached != null)
                return Ok(cached);

            try
            {
                // 1. Resolve school-wide settings that influence the math.
                var averageMethod = await _db.GlobalSettings
                    .Where(g => g.Module == "General" && g.SettingKey == "AverageCalculation")
                    .Select(g => g.SettingValue)
                    .FirstOrDefaultAsync();
                var useAllAllocated = string.Equals(averageMethod, "all_allocated_students", StringComparison.OrdinalIgnoreCase);

                var gradingCategory = await _db.GlobalSettings
                    .Where(g => g.Module == "Grading" && g.SettingKey == "ExamResults")
                    .Select(g => g.SettingValue)
                    .FirstOrDefaultAsync() ?? "4-Point";

                var grades = await _db.Grades
                    .AsNoTracking()
                    .Where(g => g.Category == gradingCategory)
                    .OrderByDescending(g => g.MinScore)
                    .ToListAsync();

                // 2. Single query for all matching exams. Filtering on the
                //    Session FK gives us the academicYearId + curriculumId
                //    scope for free.
                var exams = await _db.Exams
                    .AsNoTracking()
                    .Where(e => e.SessionId == sessionId
                                && e.ExamTypeId == examTypeId
                                && e.Session != null
                                && e.Session.AcademicYearId == academicYearId
                                && e.Session.CurriculumId == curriculumId)
                    .Select(e => new {e.Id, e.SchoolClassId, e.ExamMark})
                    .ToListAsync();

                if (exams.Count == 0)
                {
                    var emptyResult = new List<ClassExamSummaryDto>();
                    _cache.Set(cacheKey, emptyResult, CacheTtl);
                    return Ok(emptyResult);
                }

                var examIds = exams.Select(e => e.Id).ToList();
                var classIds = exams.Select(e => e.SchoolClassId).Distinct().ToList();

                // 3. Active students enrolled in any of those classes, in one query.
                var studentRows = await _db.StudentClasses
                    .AsNoTracking()
                    .Where(sc => classIds.Contains(sc.SchoolClassId)
                                 && sc.Student != null
                                 && sc.Student.Status == Status.Active)
                    .Select(sc => new {sc.SchoolClassId, sc.StudentId, FullName = sc.Student!.FullName})
                    .ToListAsync();

                // 4. All results for the matching exams, in one query.
                var resultRows = await _db.ExamResults
                    .AsNoTracking()
                    .Where(r => examIds.Contains(r.ExamId))
                    .Select(r => new {r.ExamId, r.StudentId, r.Score})
                    .ToListAsync();

                // 5. Class metadata for naming.
                var schoolClasses = await _db.SchoolClasses
                    .AsNoTracking()
                    .Include(c => c.LearningLevel)
                    .Include(c => c.SchoolStream)
                    .Where(c => classIds.Contains(c.Id))
                    .ToListAsync();

                // Group results once for cheap repeated lookup.
                var resultsByExam = resultRows
                    .GroupBy(r => r.ExamId)
                    .ToDictionary(g => g.Key, g => g.ToList());

                var summaries = new List<ClassExamSummaryDto>();
                foreach (var classId in classIds)
                {
                    var schoolClass = schoolClasses.FirstOrDefault(c => c.Id == classId);
                    if (schoolClass == null) continue;

                    var className = (schoolClass.LearningLevel?.Name ?? string.Empty);
                    if (!string.IsNullOrEmpty(schoolClass.SchoolStream?.Name))
                        className += " - " + schoolClass.SchoolStream.Name;

                    var classExams = exams.Where(e => e.SchoolClassId == classId).ToList();
                    var classStudents = studentRows.Where(sc => sc.SchoolClassId == classId).ToList();

                    // Per-student percentage totals across this class's exams.
                    var studentTotals = new Dictionary<int, (double Total, int Count)>();
                    foreach (var exam in classExams)
                    {
                        var examMark = exam.ExamMark > 0 ? exam.ExamMark : 100;
                        if (!resultsByExam.TryGetValue(exam.Id, out var examResults)) continue;
                        foreach (var r in examResults)
                        {
                            if (!studentTotals.TryGetValue(r.StudentId, out var t))
                                t = (0, 0);
                            var pct = (r.Score / examMark) * 100.0;
                            studentTotals[r.StudentId] = (t.Total + pct, t.Count + 1);
                        }
                    }

                    // Apply averaging policy (students with scores vs. all allocated).
                    var studentAvgs = new List<(string Name, double Avg)>();
                    foreach (var sc in classStudents)
                    {
                        if (studentTotals.TryGetValue(sc.StudentId, out var t) && t.Count > 0)
                        {
                            studentAvgs.Add((sc.FullName ?? string.Empty, t.Total / t.Count));
                        }
                        else if (useAllAllocated)
                        {
                            studentAvgs.Add((sc.FullName ?? string.Empty, 0));
                        }
                    }

                    if (studentAvgs.Count == 0) continue;

                    var avgs = studentAvgs.Select(s => s.Avg).ToList();
                    var classAvg = avgs.Average();
                    var highest = avgs.Max();
                    var lowest = avgs.Min();
                    var top = studentAvgs.OrderByDescending(s => s.Avg).First().Name;

                    summaries.Add(new ClassExamSummaryDto
                    {
                        SchoolClassId = classId,
                        ClassName = className,
                        StudentCount = studentAvgs.Count,
                        ClassAverage = Math.Round(classAvg * 10) / 10,
                        ClassAvgGrade = GetGradeAbbr(classAvg, grades),
                        HighestAvg = Math.Round(highest * 10) / 10,
                        HighestGrade = GetGradeAbbr(highest, grades),
                        LowestAvg = Math.Round(lowest * 10) / 10,
                        LowestGrade = GetGradeAbbr(lowest, grades),
                        TopStudent = string.IsNullOrEmpty(top) ? "-" : top
                    });
                }

                // Sort by class average descending — matches the previous
                // client-side behaviour so the table layout stays familiar.
                summaries = summaries.OrderByDescending(s => s.ClassAverage).ToList();

                _cache.Set(cacheKey, summaries, CacheTtl);
                return Ok(summaries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to compute dashboard class exam summary.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to compute dashboard class exam summary.");
            }
        }

        /// <summary>
        /// Mirrors the client's old grade-lookup logic: prefer the exact range
        /// match; fall back to the band with the closest midpoint if the value
        /// falls in a gap (which can happen with custom grading scales).
        /// </summary>
        private static string GetGradeAbbr(double percent, List<Grade> grades)
        {
            if (grades.Count == 0) return string.Empty;
            var rounded = Math.Round(percent * 10) / 10;
            var direct = grades.FirstOrDefault(g => rounded >= g.MinScore && rounded <= g.MaxScore);
            if (direct != null) return direct.Abbr;

            var closest = grades
                .OrderBy(g => Math.Abs(((g.MinScore + g.MaxScore) / 2.0) - rounded))
                .FirstOrDefault();
            return closest?.Abbr ?? string.Empty;
        }
    }
}
