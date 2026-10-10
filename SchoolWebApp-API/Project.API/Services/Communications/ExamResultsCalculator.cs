using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.DTOs.Communications;
using SchoolWebApp.Core.Entities.Enums;

namespace SchoolWebApp.API.Services.Communications
{
    public class ClassResults
    {
        public int SchoolClassId { get; set; }
        public required string ClassName { get; set; }
        public List<StudentResultDto> Results { get; set; } = new();
    }

    public class SchoolExamResults
    {
        public int SchoolExamId { get; set; }
        public required string ExamName { get; set; }
        public string? TermName { get; set; }
        public List<ClassResults> Classes { get; set; } = new();
    }

    /// <summary>
    /// Each learner's results in a school exam, class by class, worked out exactly
    /// as the Broadsheet report does (broadsheet.component.ts loadBroadsheet) so a
    /// results message sent on release matches the printed broadsheet:
    /// - subject grade and points from the percentage, on the grading scale set
    ///   for the class's education level (Grading: ExamResults[:levelId]);
    /// - mean over the subjects done, or over every examined subject the learner
    ///   is allocated when MeanBasis is "subjects_expected";
    /// - mean grade from the mean points; position by mean marks or total points
    ///   (RankingMethod[:levelId]), equal values sharing a position.
    /// Keep the two in step if either changes.
    /// </summary>
    public class ExamResultsCalculator
    {
        private readonly ApplicationDbContext _db;

        public ExamResultsCalculator(ApplicationDbContext db)
        {
            _db = db;
        }

        // JavaScript's Math.round(x * 10) / 10 - halves round up, unlike
        // Math.Round's default banker's rounding.
        private static double R1(double x) => Math.Floor(x * 10 + 0.5) / 10;

        private static string Num(double x) => R1(x).ToString("0.#");

        private record GradeBand(double Min, double Max, double Points, string Abbr);

        public async Task<SchoolExamResults?> ComputeAsync(int schoolExamId, IReadOnlyCollection<int>? schoolClassIds = null, CancellationToken ct = default)
        {
            var header = await _db.SchoolExams.AsNoTracking()
                .Where(e => e.Id == schoolExamId)
                .Select(e => new
                {
                    e.Id,
                    Type = e.ExamType!.Name,
                    e.Description,
                    Session = e.Session!.SessionName,
                    Year = e.Session.AcademicYear!.Name
                })
                .FirstOrDefaultAsync(ct);
            if (header == null) return null;

            var result = new SchoolExamResults
            {
                SchoolExamId = header.Id,
                ExamName = string.IsNullOrWhiteSpace(header.Description) ? header.Type : $"{header.Type} - {header.Description}",
                TermName = $"{header.Session} {header.Year}".Trim()
            };

            var exams = await _db.Exams.AsNoTracking()
                .Where(e => e.SchoolExamId == schoolExamId)
                .Select(e => new
                {
                    e.Id,
                    e.SchoolClassId,
                    e.SubjectId,
                    ExamMark = (double)e.ExamMark,
                    Key = e.Subject!.Abbr != "" ? e.Subject.Abbr : e.Subject.Name,
                    SubjectName = e.Subject.Name,
                    SubjectRank = e.Subject.Rank
                })
                .ToListAsync(ct);
            if (schoolClassIds != null && schoolClassIds.Count > 0)
                exams = exams.Where(e => schoolClassIds.Contains(e.SchoolClassId)).ToList();
            if (exams.Count == 0) return result;

            var classIds = exams.Select(e => e.SchoolClassId).Distinct().ToList();
            var examIds = exams.Select(e => e.Id).ToList();

            var classes = await _db.SchoolClasses.AsNoTracking()
                .Where(c => classIds.Contains(c.Id))
                .Select(c => new
                {
                    c.Id,
                    c.Rank,
                    Name = c.LearningLevel!.Name + " " + c.SchoolStream!.Name,
                    c.LearningLevel.EducationLevelId
                })
                .ToListAsync(ct);
            var enrolments = await _db.StudentClasses.AsNoTracking()
                .Where(sc => classIds.Contains(sc.SchoolClassId) && sc.Student!.Status == Status.Active)
                .Select(sc => new { sc.Id, sc.StudentId, sc.SchoolClassId })
                .ToListAsync(ct);
            var allocations = await _db.StudentSubjects.AsNoTracking()
                .Where(s => classIds.Contains(s.StudentClass!.SchoolClassId))
                .Select(s => new { s.StudentClassId, s.SubjectId })
                .ToListAsync(ct);
            var scores = await _db.ExamResults.AsNoTracking()
                .Where(r => examIds.Contains(r.ExamId))
                .Select(r => new { r.StudentId, r.ExamId, Score = (double)r.Score })
                .ToListAsync(ct);
            var allGrades = await _db.Grades.AsNoTracking()
                .Select(g => new { g.Category, g.MinScore, g.MaxScore, g.Points, g.Abbr, g.Rank })
                .ToListAsync(ct);
            var settings = await _db.GlobalSettings.AsNoTracking()
                .Where(s => s.Module == "Grading")
                .ToDictionaryAsync(s => s.SettingKey, s => s.SettingValue, ct);

            string Setting(string key) => settings.GetValueOrDefault(key) ?? "";
            var useExpected = (Setting("MeanBasis") is { Length: > 0 } mb ? mb : "subjects_done") == "subjects_expected";

            foreach (var cls in classes.OrderBy(c => c.Rank))
            {
                var levelId = cls.EducationLevelId;
                var category = Setting($"ExamResults:{levelId}") is { Length: > 0 } c1 ? c1
                    : Setting("ExamResults") is { Length: > 0 } c2 ? c2 : "4-Point";
                var ranking = Setting($"RankingMethod:{levelId}") is { Length: > 0 } r1 ? r1
                    : Setting("RankingMethod") is { Length: > 0 } r2 ? r2 : "mean_marks";
                var grades = allGrades.Where(g => g.Category == category).OrderBy(g => g.Rank)
                    .Select(g => new GradeBand(g.MinScore, g.MaxScore, g.Points, g.Abbr)).ToList();
                var maxPoints = grades.Count > 0 ? grades.Max(g => g.Points) : 0;

                GradeBand? ByPercent(double pct) => grades.FirstOrDefault(g => pct >= g.Min && pct <= g.Max);
                string GradeForPoints(double pts)
                {
                    if (grades.Count == 0) return "";
                    var exact = grades.FirstOrDefault(g => g.Points == pts);
                    if (exact != null) return exact.Abbr;
                    return grades.Aggregate((prev, curr) => Math.Abs(curr.Points - pts) < Math.Abs(prev.Points - pts) ? curr : prev).Abbr;
                }

                var clsExams = exams.Where(e => e.SchoolClassId == cls.Id).ToList();
                var subjects = clsExams.GroupBy(e => e.Key)
                    .Select(g => new { Key = g.Key, g.First().SubjectName, g.First().SubjectRank, g.First().SubjectId, g.First().ExamMark })
                    .OrderBy(s => s.SubjectRank).ToList();
                var examByKey = clsExams.GroupBy(e => e.Key).ToDictionary(g => g.Key, g => g.Select(e => e.Id).ToHashSet());
                var markById = clsExams.ToDictionary(e => e.Id, e => e.ExamMark);
                var clsExamIds = markById.Keys.ToHashSet();
                var scoresByStudent = scores.Where(r => clsExamIds.Contains(r.ExamId))
                    .GroupBy(r => r.StudentId).ToDictionary(g => g.Key, g => g.ToList());

                var rows = new List<(StudentResultDto Dto, bool HasMarks, double Average, double TotalPoints)>();
                foreach (var en in enrolments.Where(e => e.SchoolClassId == cls.Id))
                {
                    double total = 0, totalPoints = 0;
                    var count = 0;
                    var dto = new StudentResultDto { StudentId = en.StudentId };
                    foreach (var s in subjects)
                    {
                        // As the broadsheet: the last result recorded for the subject key wins.
                        var entry = scoresByStudent.GetValueOrDefault(en.StudentId)?.LastOrDefault(r => examByKey[s.Key].Contains(r.ExamId));
                        if (entry == null)
                        {
                            dto.Subjects.Add(new SubjectScoreDto { Subject = s.Key, SubjectName = s.SubjectName });
                            continue;
                        }
                        var mark = markById[entry.ExamId];
                        var pct = mark > 0 ? entry.Score / mark * 100 : 0;
                        var band = ByPercent(pct);
                        dto.Subjects.Add(new SubjectScoreDto
                        {
                            Subject = s.Key,
                            SubjectName = s.SubjectName,
                            Score = Num(entry.Score),
                            Grade = band?.Abbr ?? ""
                        });
                        total += entry.Score;
                        totalPoints += band?.Points ?? 0;
                        count++;
                    }

                    var allocated = allocations.Where(a => a.StudentClassId == en.Id).Select(a => a.SubjectId).ToHashSet();
                    var expected = useExpected && count > 0 && allocated.Count > 0
                        ? subjects.Where(s => allocated.Contains(s.SubjectId)).ToList()
                        : null;
                    var denom = expected?.Count ?? count;
                    var outOf = expected?.Sum(s => s.ExamMark)
                        ?? subjects.Where(s => dto.Subjects.Any(x => x.Subject == s.Key && x.Score != null)).Sum(s => s.ExamMark);
                    var average = denom > 0 ? R1(total / denom) : 0;
                    var meanPoints = denom > 0 ? R1(totalPoints / denom) : 0;

                    dto.TotalMarks = $"{Num(total)}/{Num(outOf)}";
                    dto.MeanScore = Num(average);
                    dto.MeanGrade = GradeForPoints(meanPoints);
                    rows.Add((dto, count > 0, average, totalPoints));
                }

                // Position among the learners with marks; equal values share it.
                var marked = rows.Where(r => r.HasMarks).ToList();
                long RankValue((StudentResultDto Dto, bool HasMarks, double Average, double TotalPoints) r) =>
                    (long)Math.Floor((ranking == "mean_marks" ? r.Average : r.TotalPoints) * 10 + 0.5);
                marked = marked.OrderByDescending(RankValue).ToList();
                for (var i = 0; i < marked.Count; i++)
                {
                    marked[i].Dto.Position = i > 0 && RankValue(marked[i]) == RankValue(marked[i - 1])
                        ? marked[i - 1].Dto.Position
                        : i + 1;
                    marked[i].Dto.ClassSize = marked.Count;
                }

                result.Classes.Add(new ClassResults
                {
                    SchoolClassId = cls.Id,
                    ClassName = cls.Name,
                    Results = marked.Select(r => r.Dto).ToList()
                });
            }
            return result;
        }
    }
}
