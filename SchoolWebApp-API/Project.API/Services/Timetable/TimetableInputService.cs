using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.DTOs.Timetable;
using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.Timetable;

namespace SchoolWebApp.API.Services.Timetable
{
    /// <summary>
    /// Turns the school's records for a term into what has to be timetabled:
    /// for each class, the subjects it takes (lessons per week from Lesson
    /// Allocation, teachers from subject allocations), less subjects left out,
    /// with option-group subjects (CRE / IRE ...) joined into one meeting. Used
    /// by the readiness check and by the generator, so both see the same week.
    /// </summary>
    public class TimetableInputService
    {
        private readonly ApplicationDbContext _db;

        public TimetableInputService(ApplicationDbContext db) => _db = db;

        public async Task<TimetableInput?> LoadAsync(int sessionId)
        {
            var session = await _db.Sessions.AsNoTracking().FirstOrDefaultAsync(s => s.Id == sessionId);
            if (session == null) return null;
            var input = new TimetableInput { Session = session };

            var schedules = await _db.BellSchedules.AsNoTracking()
                .Include(b => b.Slots).Include(b => b.Levels).Include(b => b.FixedSlots)
                .ToListAsync();
            var scheduleByLevel = schedules.SelectMany(b => b.Levels.Select(l => (l.LearningLevelId, b)))
                .ToDictionary(x => x.LearningLevelId, x => x.b);

            var classes = await _db.SchoolClasses.AsNoTracking().Include(c => c.LearningLevel)
                .Where(c => c.AcademicYearId == session.AcademicYearId)
                .OrderBy(c => c.LearningLevel!.Rank).ThenBy(c => c.Name)
                .ToListAsync();
            var levelIds = classes.Select(c => c.LearningLevelId).Distinct().ToList();
            var classIds = classes.Select(c => c.Id).ToList();

            var allocations = await _db.Set<LessonAllocation>().AsNoTracking()
                .Where(a => levelIds.Contains(a.LearningLevelId))
                .Select(a => new { a.LearningLevelId, a.SubjectId, a.LessonsPerWeek })
                .ToListAsync();
            var subjects = await _db.Subjects.AsNoTracking()
                .Select(s => new { s.Id, s.Name, s.Abbr, s.Rank }).ToDictionaryAsync(s => s.Id);
            var teachers = await _db.StaffSubjects.AsNoTracking()
                .Where(s => classIds.Contains(s.SchoolClassId))
                .Select(s => new { s.Id, s.SchoolClassId, s.SubjectId, s.StaffDetailsId, Name = s.StaffDetails!.FullName })
                .OrderBy(s => s.Id)
                .ToListAsync();
            var rules = await _db.TimetableSubjectRules.AsNoTracking().ToListAsync();
            var groups = await _db.TimetableOptionGroups.AsNoTracking().Include(g => g.Subjects).ToListAsync();
            input.TeacherRules = await _db.TeacherTimetableRules.AsNoTracking().ToDictionaryAsync(r => r.StaffDetailsId);
            input.Unavailable = (await _db.TeacherUnavailabilities.AsNoTracking().ToListAsync())
                .GroupBy(u => u.StaffDetailsId).ToDictionary(g => g.Key, g => g.ToList());
            input.Schedules = schedules;

            foreach (var c in classes)
            {
                var tc = new TimetableClass
                {
                    SchoolClass = c,
                    Schedule = scheduleByLevel.GetValueOrDefault(c.LearningLevelId)
                };
                // Only grades on a bell schedule are timetabled (Day Care or a
                // special unit may well have no timetable).
                if (tc.Schedule == null)
                {
                    input.LeftOut.Add(c.Name);
                    continue;
                }
                input.Classes.Add(tc);

                var units = new List<TimetableSubjectNeed>();
                foreach (var a in allocations.Where(a => a.LearningLevelId == c.LearningLevelId && a.LessonsPerWeek > 0))
                {
                    var rule = RuleFor(rules, a.SubjectId, c.LearningLevelId);
                    var classTeachers = teachers.Where(t => t.SchoolClassId == c.Id && t.SubjectId == a.SubjectId).ToList();
                    var subject = subjects.GetValueOrDefault(a.SubjectId);
                    var need = new TimetableSubjectNeed
                    {
                        SubjectId = a.SubjectId,
                        SubjectName = subject?.Name ?? $"Subject {a.SubjectId}",
                        SubjectAbbr = subject?.Abbr ?? "",
                        LessonsPerWeek = a.LessonsPerWeek,
                        StaffDetailsId = classTeachers.FirstOrDefault()?.StaffDetailsId,
                        TeacherName = classTeachers.FirstOrDefault()?.Name,
                        Rule = rule,
                        Excluded = rule?.Excluded == true
                    };
                    units.Add(need);
                    if (need.Excluded) continue;
                    if (need.StaffDetailsId == null)
                        input.Problems.Add($"{c.Name}: {need.SubjectName} has no teacher allocated.");
                    if (classTeachers.Select(t => t.StaffDetailsId).Distinct().Count() > 1)
                        input.Warnings.Add($"{c.Name}: {need.SubjectName} has {classTeachers.Count} teachers allocated; the timetable uses {need.TeacherName}.");
                }
                tc.Subjects = units.OrderBy(u => subjects.GetValueOrDefault(u.SubjectId)?.Rank ?? 0).ThenBy(u => u.SubjectName).ToList();

                // Subjects taught to the class with no lessons per week for its grade.
                var noLessons = teachers.Where(t => t.SchoolClassId == c.Id).Select(t => t.SubjectId).Distinct()
                    .Where(t => !allocations.Any(a => a.LearningLevelId == c.LearningLevelId && a.SubjectId == t && a.LessonsPerWeek > 0))
                    .Select(t => subjects.GetValueOrDefault(t)?.Abbr ?? "?").ToList();
                if (noLessons.Count > 0)
                    input.Warnings.Add($"{c.Name}: {string.Join(", ", noLessons)} {(noLessons.Count == 1 ? "has" : "have")} a teacher but no lessons per week " +
                        $"for {c.LearningLevel?.Name} (Lesson Allocation), so {(noLessons.Count == 1 ? "it is" : "they are")} left off.");

                // Option groups: the grade's own groups win over school-wide ones.
                var active = tc.Subjects.Where(s => !s.Excluded).ToList();
                var used = new HashSet<int>();
                foreach (var g in groups.OrderBy(g => g.LearningLevelId == null ? 1 : 0))
                {
                    if (g.LearningLevelId != null && g.LearningLevelId != c.LearningLevelId) continue;
                    var members = active.Where(s => g.Subjects.Any(m => m.SubjectId == s.SubjectId) && !used.Contains(s.SubjectId)).ToList();
                    if (members.Count < 2) continue;
                    // One teacher cannot take two groups at once: those subjects stay apart.
                    var shared = members.Where(m => m.StaffDetailsId != null)
                        .GroupBy(m => m.StaffDetailsId).Where(x => x.Count() > 1).SelectMany(x => x).ToList();
                    if (shared.Any())
                    {
                        input.Warnings.Add($"{c.Name}: {string.Join(" and ", shared.Select(s => s.SubjectAbbr))} are in the option group {g.Name} " +
                            $"but have the same teacher ({shared[0].TeacherName}), so they are timetabled one after the other.");
                        members = members.Except(shared.Skip(1)).ToList();
                        if (members.Count < 2) continue;
                    }
                    foreach (var m in members) { used.Add(m.SubjectId); m.OptionGroupName = g.Name; m.OptionGroupId = g.Id; }
                }
            }
            return input;
        }

        /// <summary>The grade's own rule for a subject, or else the subject's rule for every grade.</summary>
        public static TimetableSubjectRule? RuleFor(IEnumerable<TimetableSubjectRule> rules, int subjectId, int learningLevelId) =>
            rules.FirstOrDefault(r => r.SubjectId == subjectId && r.LearningLevelId == learningLevelId)
            ?? rules.FirstOrDefault(r => r.SubjectId == subjectId && r.LearningLevelId == null);

        public static List<int> ParseDays(string? days) =>
            (days ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(d => int.TryParse(d.Trim(), out var n) ? n : 0).Where(n => n >= 1 && n <= 7)
                .Distinct().OrderBy(n => n).ToList();

        /// <summary>The readiness check: lessons needed against slots free, per class and per teacher.</summary>
        public TimetableReadinessDto Readiness(TimetableInput input)
        {
            var dto = new TimetableReadinessDto { SessionId = input.Session.Id };
            dto.Problems.AddRange(input.Problems);
            if (input.LeftOut.Count > 0)
                dto.Warnings.Insert(0, $"Not on any bell schedule, so left out of the timetable: {string.Join(", ", input.LeftOut)}.");
            dto.Warnings.AddRange(input.Warnings);
            dto.LeftOut = input.LeftOut;
            if (input.Classes.Count == 0)
                dto.Problems.Add(input.LeftOut.Count > 0 ? "No class's grade is on a bell schedule yet." : "The term's academic year has no classes.");

            foreach (var c in input.Classes)
            {
                var row = new ReadinessClassDto
                {
                    SchoolClassId = c.SchoolClass.Id,
                    ClassName = c.SchoolClass.Name,
                    LearningLevelId = c.SchoolClass.LearningLevelId,
                    BellScheduleName = c.Schedule?.Name,
                    LessonsNeeded = c.LessonsNeeded,
                    SlotsAvailable = c.Schedule == null ? 0 : SlotsFor(c.Schedule, c.SchoolClass.LearningLevelId),
                    SubjectsWithoutTeacher = c.Subjects.Where(s => !s.Excluded && s.StaffDetailsId == null).Select(s => s.SubjectName).ToList(),
                    Subjects = c.Subjects.Select(s => new ReadinessSubjectDto
                    {
                        SubjectId = s.SubjectId,
                        SubjectName = s.OptionGroupName == null ? s.SubjectName : $"{s.SubjectName} ({s.OptionGroupName})",
                        LessonsPerWeek = s.LessonsPerWeek,
                        StaffDetailsId = s.StaffDetailsId,
                        TeacherName = s.TeacherName,
                        Excluded = s.Excluded
                    }).ToList()
                };
                if (c.Schedule != null && row.LessonsNeeded > row.SlotsAvailable)
                    dto.Problems.Add($"{row.ClassName}: needs {row.LessonsNeeded} lessons a week but {c.Schedule.Name} has only {row.SlotsAvailable} free lesson slots.");
                dto.Classes.Add(row);
            }

            // Teachers: their lessons against the slots of the busiest schedule they teach on.
            var byTeacher = input.Classes
                .SelectMany(c => c.Subjects.Where(s => !s.Excluded && s.StaffDetailsId != null).Select(s => (Class: c, Need: s)))
                .GroupBy(x => x.Need.StaffDetailsId!.Value);
            foreach (var g in byTeacher)
            {
                var schedulesTaught = g.Select(x => x.Class.Schedule).Where(s => s != null).Distinct().ToList();
                var free = schedulesTaught.Count == 0 ? 0 : schedulesTaught.Max(s => TeacherSlotsFor(s!, input.Unavailable.GetValueOrDefault(g.Key)));
                var row = new ReadinessTeacherDto
                {
                    StaffDetailsId = g.Key,
                    Name = g.First().Need.TeacherName ?? "",
                    LessonsAssigned = g.Sum(x => x.Need.LessonsPerWeek),
                    SlotsAvailable = free,
                    Classes = g.Select(x => x.Class.SchoolClass.Id).Distinct().Count()
                };
                if (row.LessonsAssigned > row.SlotsAvailable && row.SlotsAvailable > 0)
                    dto.Problems.Add($"{row.Name}: has {row.LessonsAssigned} lessons a week but only {row.SlotsAvailable} slots they can teach in.");
                var rule = input.TeacherRules.GetValueOrDefault(g.Key);
                if (rule?.MaxLessonsPerDay is int max && schedulesTaught.Count > 0)
                {
                    var days = schedulesTaught.Max(s => ParseDays(s!.Days).Count);
                    if (row.LessonsAssigned > max * days)
                        dto.Problems.Add($"{row.Name}: has {row.LessonsAssigned} lessons a week but at most {max} a day over {days} days allows {max * days}.");
                }
                dto.Teachers.Add(row);
            }
            dto.Teachers = dto.Teachers.OrderByDescending(t => t.LessonsAssigned).ToList();
            dto.Ready = dto.Problems.Count == 0;
            return dto;
        }

        /// <summary>Lesson slots a week for a grade on a schedule, less slots blocked for activities.</summary>
        public static int SlotsFor(BellSchedule s, int learningLevelId)
        {
            var days = ParseDays(s.Days).Count;
            var lessons = s.Slots.Count(x => x.Kind == BellSlotKind.Lesson);
            var blocked = s.FixedSlots.Count(f => f.SubjectId == null && (f.LearningLevelId == null || f.LearningLevelId == learningLevelId)
                && s.Slots.Any(x => x.Id == f.BellSlotId && x.Kind == BellSlotKind.Lesson));
            return days * lessons - blocked;
        }

        private static int TeacherSlotsFor(BellSchedule s, List<TeacherUnavailability>? off)
        {
            var count = 0;
            foreach (var day in ParseDays(s.Days))
                foreach (var slot in s.Slots.Where(x => x.Kind == BellSlotKind.Lesson))
                    if (off == null || !off.Any(u => u.Day == day && u.StartTime < slot.EndTime && slot.StartTime < u.EndTime))
                        count++;
            return count;
        }
    }

    public class TimetableInput
    {
        public required Session Session { get; set; }
        public List<BellSchedule> Schedules { get; set; } = new();
        public List<TimetableClass> Classes { get; set; } = new();
        public Dictionary<int, TeacherTimetableRule> TeacherRules { get; set; } = new();
        public Dictionary<int, List<TeacherUnavailability>> Unavailable { get; set; } = new();
        // Stop generation.
        public List<string> Problems { get; set; } = new();
        // Worth knowing, but a timetable can still be made.
        public List<string> Warnings { get; set; } = new();
        // Classes whose grade is on no bell schedule, so they get no timetable.
        public List<string> LeftOut { get; set; } = new();
    }

    public class TimetableClass
    {
        public required SchoolClass SchoolClass { get; set; }
        public BellSchedule? Schedule { get; set; }
        public List<TimetableSubjectNeed> Subjects { get; set; } = new();

        /// <summary>Lesson slots the class needs: an option group meets as often as its busiest subject.</summary>
        public int LessonsNeeded =>
            Subjects.Where(s => !s.Excluded && s.OptionGroupId == null).Sum(s => s.LessonsPerWeek)
            + Subjects.Where(s => !s.Excluded && s.OptionGroupId != null)
                .GroupBy(s => s.OptionGroupId).Sum(g => g.Max(s => s.LessonsPerWeek));
    }

    public class TimetableSubjectNeed
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = "";
        public string SubjectAbbr { get; set; } = "";
        public int LessonsPerWeek { get; set; }
        public int? StaffDetailsId { get; set; }
        public string? TeacherName { get; set; }
        public TimetableSubjectRule? Rule { get; set; }
        public bool Excluded { get; set; }
        public int? OptionGroupId { get; set; }
        public string? OptionGroupName { get; set; }
    }
}
