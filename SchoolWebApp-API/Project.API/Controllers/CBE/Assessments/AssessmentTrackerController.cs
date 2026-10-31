using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.API.Licensing;
using SchoolWebApp.API.Services;
using SchoolWebApp.API.Services.TeachingDocuments;
using SchoolWebApp.Core.DTOs.CBE.Assessments;
using SchoolWebApp.Core.DTOs.TeachingDocuments;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.Enums;

namespace SchoolWebApp.API.Controllers.CBE.Assessments
{
    /// <summary>
    /// How far each teacher has got with formative assessments this term: one row
    /// per teacher x subject x class. The scheme of work says what is due - a
    /// sub-strand's specific outcomes fall due a week after the last week that
    /// teaches it (by the end of term at the latest) - and the grades entered say
    /// what is done. Teachers see their own rows, heads of department their
    /// departments, and administrators and assessments admins the whole school.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [RequiresModule(AppModules.Academics)]
    public class AssessmentTrackerController : ControllerBase
    {
        // Days after a sub-strand's last scheme week for its outcomes to be graded.
        private const int GraceDays = 7;
        // An outcome counts as graded for the class once this share of its learners
        // have a grade - the rest may have been absent.
        private const double GradedShare = 0.8;
        private const string AdminPath = "/cbe/assessments/admin";

        private readonly ApplicationDbContext _db;
        private readonly TeachingDocumentService _svc;
        private readonly MenuAccessService _access;

        public AssessmentTrackerController(ApplicationDbContext db, TeachingDocumentService svc, MenuAccessService access)
        {
            _db = db;
            _svc = svc;
            _access = access;
        }

        // GET api/assessmentTracker?sessionId=6
        [HttpGet]
        public async Task<IActionResult> Tracker(int sessionId)
        {
            var me = await _svc.CurrentAsync();
            if (me == null) return Unauthorized();
            var session = await _db.Sessions.AsNoTracking().Include(s => s.AcademicYear).FirstOrDefaultAsync(s => s.Id == sessionId);
            if (session == null) return NotFound(new { message = "Term not found." });
            var wholeSchool = me.WholeSchool || await _access.CanAnyAsync(AdminPath);

            var assignments = await _svc.AssignmentsAsync(session, me, oversee: true, wholeSchool);
            var data = await LoadAsync(session, assignments.Select(a => (a.SchoolClassId, a.SubjectId, a.SchemeOfWorkId)).ToList());

            var result = new AssessmentTrackerDto
            {
                Session = TeachingDocumentService.ToSessionDto(session),
                CurriculumId = session.CurriculumId,
                Scope = wholeSchool ? "school" : me.HodDepartmentIds.Count > 0 ? "department" : "own",
                GraceDays = GraceDays
            };
            foreach (var a in assignments)
            {
                var learners = data.LearnersOf(a.SchoolClassId, a.SubjectId).Count;
                var subStrands = a.SchemeOfWorkId is int id ? data.SchemeSubStrands.GetValueOrDefault(id) ?? new() : new();
                var due = subStrands.Where(s => s.Due).ToList();
                var dueOutcomes = due.SelectMany(s => data.OutcomesOf(s.SubStrandId)).ToList();
                var graded = dueOutcomes.Select(o => data.GradedCount(a.SchoolClassId, o)).ToList();
                var next = subStrands.Where(s => !s.Due).OrderBy(s => s.DueAt).FirstOrDefault();
                var row = new AssessmentTrackerRowDto
                {
                    StaffDetailsId = a.StaffDetailsId,
                    StaffName = a.StaffName,
                    SubjectId = a.SubjectId,
                    SubjectName = a.SubjectName,
                    SubjectRank = a.SubjectRank,
                    SchoolClassId = a.SchoolClassId,
                    ClassName = a.ClassName,
                    LearningLevelId = a.LearningLevelId,
                    LearningLevelName = a.LearningLevelName,
                    LearningLevelRank = a.LearningLevelRank,
                    DepartmentId = a.DepartmentId,
                    DepartmentName = a.DepartmentName,
                    SchemeOfWorkId = a.SchemeOfWorkId,
                    Learners = learners,
                    SubStrandsPlanned = subStrands.Count,
                    SubStrandsDue = due.Count,
                    OutcomesDue = dueOutcomes.Count,
                    OutcomesGraded = graded.Count(g => IsGraded(g, learners)),
                    OutcomesPartlyGraded = graded.Count(g => g > 0 && !IsGraded(g, learners)),
                    OutcomesOverdue = graded.Count(g => g == 0),
                    GradesEntered = graded.Sum(g => learners > 0 ? Math.Min(g, learners) : g),
                    GradesExpected = dueOutcomes.Count * learners,
                    OutcomesGradedThisTerm = data.Grades.Count(g => g.SchoolClassId == a.SchoolClassId && g.SubjectId == a.SubjectId),
                    NextSubStrand = next == null ? null : data.SubStrandNames.GetValueOrDefault(next.SubStrandId),
                    NextDueAt = next?.DueAt,
                    LastEntry = data.Grades.Where(g => g.SchoolClassId == a.SchoolClassId && g.SubjectId == a.SubjectId)
                        .Max(g => (DateTime?)g.LastEntry)
                };
                row.Status = a.SchemeOfWorkId == null ? AssessmentTrackStatus.NoScheme
                    : row.OutcomesDue == 0 ? AssessmentTrackStatus.NothingDue
                    : row.OutcomesOverdue == row.OutcomesDue ? AssessmentTrackStatus.NotStarted
                    : row.OutcomesOverdue > 0 ? AssessmentTrackStatus.Behind
                    : row.OutcomesPartlyGraded > 0 ? AssessmentTrackStatus.Incomplete
                    : AssessmentTrackStatus.UpToDate;
                result.Rows.Add(row);
            }
            return Ok(result);
        }

        // GET api/assessmentTracker/detail?sessionId=6&schoolClassId=3&subjectId=4&staffDetailsId=12
        // One row opened: each sub-strand of the scheme (and any other graded this
        // term) with its outcomes' grade counts, and the learners missing grades.
        [HttpGet("detail")]
        public async Task<IActionResult> Detail(int sessionId, int schoolClassId, int subjectId, int staffDetailsId)
        {
            var me = await _svc.CurrentAsync();
            if (me == null) return Unauthorized();
            var session = await _db.Sessions.AsNoTracking().FirstOrDefaultAsync(s => s.Id == sessionId);
            if (session == null) return NotFound(new { message = "Term not found." });
            var wholeSchool = me.WholeSchool || await _access.CanAnyAsync(AdminPath);
            var a = (await _svc.AssignmentsAsync(session, me, oversee: true, wholeSchool))
                .FirstOrDefault(x => x.SchoolClassId == schoolClassId && x.SubjectId == subjectId && x.StaffDetailsId == staffDetailsId);
            if (a == null) return NotFound(new { message = "That subject allocation is not one you can see." });

            var data = await LoadAsync(session, new() { (a.SchoolClassId, a.SubjectId, a.SchemeOfWorkId) });
            var planned = a.SchemeOfWorkId is int id ? data.SchemeSubStrands.GetValueOrDefault(id) ?? new() : new();
            // Sub-strands graded this term but not in the scheme still show, after the scheme's.
            var gradedOutcomeIds = data.Grades.Where(g => g.SchoolClassId == a.SchoolClassId && g.SubjectId == a.SubjectId)
                .Select(g => g.SpecificOutcomeId).ToList();
            var extraSubStrandIds = await _db.SpecificOutcomes.AsNoTracking().Where(o => gradedOutcomeIds.Contains(o.Id))
                .Select(o => o.SubStrandId).Distinct().ToListAsync();
            var subStrandIds = planned.Select(p => p.SubStrandId).Union(extraSubStrandIds).ToList();
            var subStrands = await _db.SubStrands.AsNoTracking().Where(s => subStrandIds.Contains(s.Id))
                .Select(s => new { s.Id, s.Name, s.Rank, s.StrandId, StrandName = s.Strand!.Name, StrandRank = s.Strand.Rank })
                .ToListAsync();
            var outcomes = await _db.SpecificOutcomes.AsNoTracking().Where(o => subStrandIds.Contains(o.SubStrandId))
                .OrderBy(o => o.Rank).ThenBy(o => o.Id).Select(o => new { o.Id, o.SubStrandId, o.Name }).ToListAsync();

            var learners = data.LearnersOf(a.SchoolClassId, a.SubjectId);
            var dto = new AssessmentTrackerDetailDto { Learners = learners.Count };
            foreach (var s in subStrands.OrderBy(s => planned.FindIndex(p => p.SubStrandId == s.Id) is var i && i < 0 ? int.MaxValue : i)
                         .ThenBy(s => s.StrandRank).ThenBy(s => s.Rank))
            {
                var p = planned.FirstOrDefault(x => x.SubStrandId == s.Id);
                dto.SubStrands.Add(new AssessmentTrackerSubStrandDto
                {
                    StrandId = s.StrandId,
                    StrandName = s.StrandName,
                    SubStrandId = s.Id,
                    SubStrandName = s.Name,
                    LastWeek = p?.LastWeek,
                    DueAt = p?.DueAt,
                    Due = p?.Due ?? false,
                    InScheme = p != null,
                    Outcomes = outcomes.Where(o => o.SubStrandId == s.Id)
                        .Select(o => new AssessmentTrackerOutcomeDto { Id = o.Id, Name = o.Name, Graded = data.GradedCount(a.SchoolClassId, o.Id) })
                        .ToList()
                });
            }

            // Learners without a grade on outcomes that are due.
            var dueOutcomeIds = dto.SubStrands.Where(s => s.Due).SelectMany(s => s.Outcomes.Select(o => o.Id)).ToList();
            if (dueOutcomeIds.Count > 0 && learners.Count > 0)
            {
                var have = (await _db.StudentAssessments.AsNoTracking()
                        .Where(x => x.SessionId == session.Id && x.SchoolClassId == a.SchoolClassId
                                    && x.SpecificOutcomeId != null && dueOutcomeIds.Contains(x.SpecificOutcomeId.Value))
                        .Select(x => new { x.StudentId, x.SpecificOutcomeId }).Distinct().ToListAsync())
                    .GroupBy(x => x.StudentId).ToDictionary(g => g.Key, g => g.Count());
                var people = await _db.Students.AsNoTracking().Where(st => learners.Contains(st.Id))
                    .Select(st => new { st.Id, st.FullName, st.UPI }).ToListAsync();
                dto.LearnersMissing = people
                    .Select(st => new AssessmentTrackerLearnerDto
                    {
                        StudentId = st.Id,
                        Name = st.FullName,
                        Upi = st.UPI,
                        Missing = dueOutcomeIds.Count - have.GetValueOrDefault(st.Id)
                    })
                    .Where(x => x.Missing > 0)
                    .OrderByDescending(x => x.Missing).ThenBy(x => x.Name).ToList();
            }
            return Ok(dto);
        }

        private static bool IsGraded(int graded, int learners) =>
            learners == 0 ? graded > 0 : graded >= Math.Ceiling(learners * GradedShare);

        private record PlannedSubStrand(int SubStrandId, int LastWeek, DateTime DueAt, bool Due);
        private record GradeCell(int SchoolClassId, int SubjectId, int SpecificOutcomeId, int Learners, DateTime LastEntry);

        private class TrackerData
        {
            public Dictionary<int, List<PlannedSubStrand>> SchemeSubStrands { get; set; } = new();
            public Dictionary<int, string> SubStrandNames { get; set; } = new();
            public Dictionary<int, List<int>> Outcomes { get; set; } = new();
            public List<GradeCell> Grades { get; set; } = new();
            public Dictionary<(int, int), int> GradedByOutcome { get; set; } = new();
            public Dictionary<int, List<int>> ClassLearners { get; set; } = new();
            public Dictionary<(int, int), List<int>> SubjectLearners { get; set; } = new();

            public List<int> OutcomesOf(int subStrandId) => Outcomes.GetValueOrDefault(subStrandId) ?? new();
            public int GradedCount(int classId, int outcomeId) => GradedByOutcome.GetValueOrDefault((classId, outcomeId));

            // The learners taking the subject in the class; a class with nobody
            // registered for it falls back to the whole class.
            public List<int> LearnersOf(int classId, int subjectId) =>
                SubjectLearners.GetValueOrDefault((classId, subjectId)) ?? ClassLearners.GetValueOrDefault(classId) ?? new();
        }

        // What the rows need, in a handful of queries: each scheme's sub-strands and
        // when they fall due, their outcomes, the term's grades by class and outcome,
        // and the learners of each class and subject.
        private async Task<TrackerData> LoadAsync(Session session, List<(int ClassId, int SubjectId, int? SchemeId)> keys)
        {
            var data = new TrackerData();
            var classIds = keys.Select(k => k.ClassId).Distinct().ToList();
            var schemeIds = keys.Where(k => k.SchemeId.HasValue).Select(k => k.SchemeId!.Value).Distinct().ToList();

            var lastWeeks = await _db.SchemeOfWorkLessons.AsNoTracking()
                .Where(l => schemeIds.Contains(l.SchemeOfWorkId) && l.SubStrandId != null)
                .GroupBy(l => new { l.SchemeOfWorkId, SubStrandId = l.SubStrandId!.Value })
                .Select(g => new { g.Key.SchemeOfWorkId, g.Key.SubStrandId, LastWeek = g.Max(l => l.WeekNo) })
                .ToListAsync();
            var today = DateTime.Today;
            DateTime DueAt(int week)
            {
                var at = TeachingDocumentService.WeekRange(session, week).End.Date.AddDays(GraceDays);
                return at > session.EndDate.Date ? session.EndDate.Date : at;
            }
            data.SchemeSubStrands = lastWeeks.GroupBy(l => l.SchemeOfWorkId).ToDictionary(g => g.Key, g => g
                .OrderBy(l => l.LastWeek)
                .Select(l => new PlannedSubStrand(l.SubStrandId, l.LastWeek, DueAt(l.LastWeek), today > DueAt(l.LastWeek)))
                .ToList());

            var subStrandIds = lastWeeks.Select(l => l.SubStrandId).Distinct().ToList();
            data.SubStrandNames = await _db.SubStrands.AsNoTracking().Where(s => subStrandIds.Contains(s.Id))
                .ToDictionaryAsync(s => s.Id, s => s.Name);
            data.Outcomes = (await _db.SpecificOutcomes.AsNoTracking().Where(o => subStrandIds.Contains(o.SubStrandId))
                    .Select(o => new { o.Id, o.SubStrandId }).ToListAsync())
                .GroupBy(o => o.SubStrandId).ToDictionary(g => g.Key, g => g.Select(o => o.Id).ToList());

            data.Grades = (await _db.StudentAssessments.AsNoTracking()
                    .Where(x => x.SessionId == session.Id && x.SpecificOutcomeId != null && classIds.Contains(x.SchoolClassId))
                    .GroupBy(x => new { x.SchoolClassId, x.SpecificOutcome!.SubStrand!.Strand!.SubjectId, SpecificOutcomeId = x.SpecificOutcomeId!.Value })
                    .Select(g => new
                    {
                        g.Key.SchoolClassId,
                        g.Key.SubjectId,
                        g.Key.SpecificOutcomeId,
                        Learners = g.Select(x => x.StudentId).Distinct().Count(),
                        LastEntry = g.Max(x => x.Modified ?? x.Created)
                    })
                    .ToListAsync())
                .Select(g => new GradeCell(g.SchoolClassId, g.SubjectId, g.SpecificOutcomeId, g.Learners, g.LastEntry ?? DateTime.MinValue))
                .ToList();
            data.GradedByOutcome = data.Grades.ToDictionary(g => (g.SchoolClassId, g.SpecificOutcomeId), g => g.Learners);

            var inClass = await _db.StudentClasses.AsNoTracking()
                .Where(sc => classIds.Contains(sc.SchoolClassId) && sc.Student!.Status == Status.Active)
                .Select(sc => new { sc.SchoolClassId, sc.StudentId }).ToListAsync();
            data.ClassLearners = inClass.GroupBy(x => x.SchoolClassId)
                .ToDictionary(g => g.Key, g => g.Select(x => x.StudentId).Distinct().ToList());
            var taking = await _db.StudentSubjects.AsNoTracking()
                .Where(ss => classIds.Contains(ss.StudentClass!.SchoolClassId) && ss.StudentClass.Student!.Status == Status.Active)
                .Select(ss => new { ss.StudentClass!.SchoolClassId, ss.SubjectId, ss.StudentClass.StudentId }).ToListAsync();
            data.SubjectLearners = taking.GroupBy(x => (x.SchoolClassId, x.SubjectId))
                .ToDictionary(g => g.Key, g => g.Select(x => x.StudentId).Distinct().ToList());
            return data;
        }
    }
}
