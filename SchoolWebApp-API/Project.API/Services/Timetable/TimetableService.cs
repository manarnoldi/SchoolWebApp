using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.DTOs.Timetable;
using SchoolWebApp.Core.Entities.Timetable;

namespace SchoolWebApp.API.Services.Timetable
{
    /// <summary>Timetables: what the pages show, generating, and publishing.</summary>
    public class TimetableService
    {
        private readonly ApplicationDbContext _db;
        private readonly TimetableInputService _input;

        public TimetableService(ApplicationDbContext db, TimetableInputService input)
        {
            _db = db;
            _input = input;
        }

        public async Task<List<TimetableSummaryDto>> SummariesAsync(int? sessionId)
        {
            var q = _db.SchoolTimetables.AsNoTracking().AsQueryable();
            if (sessionId != null) q = q.Where(t => t.SessionId == sessionId);
            return await q.OrderByDescending(t => t.Status == TimetableStatus.Published).ThenByDescending(t => t.Modified)
                .Select(t => new TimetableSummaryDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    SessionId = t.SessionId,
                    SessionName = t.Session!.AcademicYear!.Name + " " + t.Session.SessionName,
                    Status = t.Status,
                    EffectiveFrom = t.EffectiveFrom,
                    GeneratedAt = t.GeneratedAt,
                    PublishedAt = t.PublishedAt,
                    Notes = t.Notes,
                    Lessons = t.Lessons.Count,
                    Unplaced = t.Lessons.Count(l => l.Day == null),
                    Modified = t.Modified
                }).ToListAsync();
        }

        /// <summary>A timetable with everything its pages draw.</summary>
        public async Task<TimetableDetailDto?> DetailAsync(int id)
        {
            var t = await _db.SchoolTimetables.AsNoTracking().Include(x => x.Session).ThenInclude(s => s!.AcademicYear)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (t == null) return null;
            var cards = await _db.TimetableLessons.AsNoTracking().Where(l => l.SchoolTimetableId == id).ToListAsync();

            var schedules = await _db.BellSchedules.AsNoTracking().Include(b => b.Slots).Include(b => b.Levels).Include(b => b.FixedSlots)
                .OrderBy(b => b.Name).ToListAsync();
            var scheduleByLevel = schedules.SelectMany(b => b.Levels.Select(l => (l.LearningLevelId, b.Id))).ToDictionary(x => x.LearningLevelId, x => x.Id);
            var classes = await _db.SchoolClasses.AsNoTracking().Include(c => c.LearningLevel)
                .Where(c => c.AcademicYearId == t.Session!.AcademicYearId)
                .OrderBy(c => c.LearningLevel!.Rank).ThenBy(c => c.Name).ToListAsync();
            var staffIds = cards.Where(c => c.StaffDetailsId != null).Select(c => c.StaffDetailsId!.Value).Distinct().ToList();
            var teachers = await _db.StaffDetails.AsNoTracking().Where(s => staffIds.Contains(s.Id))
                .Select(s => new { s.Id, s.FullName }).ToListAsync();
            var subjectIds = cards.Select(c => c.SubjectId).Distinct().ToList();
            var subjects = await _db.Subjects.AsNoTracking().Where(s => subjectIds.Contains(s.Id))
                .Select(s => new TimetableSubjectDto { Id = s.Id, Name = s.Name, Abbr = s.Abbr }).ToListAsync();

            var dto = new TimetableDetailDto
            {
                Id = t.Id,
                Name = t.Name,
                SessionId = t.SessionId,
                SessionName = $"{t.Session!.AcademicYear?.Name} {t.Session.SessionName}",
                Status = t.Status,
                EffectiveFrom = t.EffectiveFrom,
                GeneratedAt = t.GeneratedAt,
                PublishedAt = t.PublishedAt,
                Notes = t.Notes,
                Modified = t.Modified,
                Lessons = cards.Count,
                Unplaced = cards.Count(c => c.Day == null),
                Report = string.IsNullOrEmpty(t.GenerationReport) ? null : JsonConvert.DeserializeObject<TimetableReportDto>(t.GenerationReport),
                Schedules = schedules.Select(b => new TimetableScheduleDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    Days = TimetableInputService.ParseDays(b.Days),
                    Slots = b.Slots.OrderBy(s => s.Rank).Select(s => new TimetableSlotDto
                    {
                        Id = s.Id, Rank = s.Rank, StartTime = s.StartTime.ToString("HH:mm"), EndTime = s.EndTime.ToString("HH:mm"),
                        Kind = s.Kind, Part = s.Part, Label = s.Label
                    }).ToList(),
                    Fixed = b.FixedSlots.Where(f => f.SubjectId == null).Select(f => new TimetableFixedDto
                    {
                        Day = f.Day, BellSlotId = f.BellSlotId, LearningLevelId = f.LearningLevelId, SubjectId = f.SubjectId, Label = f.Label
                    }).ToList()
                }).ToList(),
                Classes = classes.Where(c => scheduleByLevel.ContainsKey(c.LearningLevelId)).Select(c => new TimetableClassDto
                {
                    Id = c.Id, Name = c.Name, LearningLevelId = c.LearningLevelId, ScheduleId = scheduleByLevel[c.LearningLevelId]
                }).ToList(),
                Teachers = teachers.OrderBy(x => x.FullName).Select(x => new TimetablePersonDto
                {
                    Id = x.Id, Name = x.FullName, Initials = Initials(x.FullName)
                }).ToList(),
                Subjects = subjects.OrderBy(s => s.Name).ToList(),
                Cards = cards.Select(c => new TimetableLessonDto
                {
                    Id = c.Id, SchoolClassId = c.SchoolClassId, SubjectId = c.SubjectId, StaffDetailsId = c.StaffDetailsId,
                    Day = c.Day, BellSlotId = c.BellSlotId, Length = c.Length, LinkNo = c.LinkNo, IsLocked = c.IsLocked
                }).ToList()
            };
            return dto;
        }

        /// <summary>
        /// Generates the timetable afresh, keeping locked cards where they are.
        /// Returns why it could not run, or null.
        /// </summary>
        public async Task<string?> GenerateAsync(SchoolTimetable t, int seconds)
        {
            var input = await _input.LoadAsync(t.SessionId);
            if (input == null) return "Term not found.";
            var readiness = _input.Readiness(input);
            // Missing teachers and short weeks still generate (what cannot be placed
            // is reported); a timetable with no classes cannot.
            if (input.Classes.Count == 0) return readiness.Problems.FirstOrDefault() ?? "No classes to timetable.";

            var old = await _db.TimetableLessons.Where(l => l.SchoolTimetableId == t.Id).ToListAsync();
            // An option group's meeting is one locked card, however many subjects it has.
            var locked = old.Where(l => l.IsLocked && l.Day != null && l.BellSlotId != null)
                .GroupBy(l => l.LinkNo == null ? $"L{l.Id}" : $"G{l.SchoolClassId}-{l.LinkNo}")
                .Select(g => g.First())
                .Select(l => new TimetableGenerator.LockedLesson(l.SchoolClassId, l.SubjectId, l.Day!.Value, l.BellSlotId!.Value, l.Length, l.LinkNo))
                .ToList();

            var generator = new TimetableGenerator(input);
            var result = generator.Run(locked, TimeSpan.FromSeconds(Math.Clamp(seconds, 3, 60)));

            _db.TimetableLessons.RemoveRange(old);
            _db.TimetableLessons.AddRange(result.Lessons.Select(l => new TimetableLesson
            {
                SchoolTimetableId = t.Id,
                SchoolClassId = l.SchoolClassId,
                SubjectId = l.SubjectId,
                StaffDetailsId = l.StaffDetailsId,
                Day = l.Day,
                BellSlotId = l.BellSlotId,
                Length = l.Length,
                LinkNo = l.LinkNo,
                IsLocked = l.IsLocked
            }));
            t.GeneratedAt = DateTime.Now;
            var report = new TimetableReportDto
            {
                Placed = result.Placed,
                Total = result.Total,
                Attempts = result.Attempts,
                Seconds = result.Seconds,
                Score = result.Score,
                Unplaced = result.Unplaced.Select(u => new TimetableUnplacedDto
                {
                    ClassName = u.ClassName, Subjects = u.Subjects, Teachers = u.Teachers, Length = u.Length, Reason = u.Reason
                }).ToList(),
                Warnings = readiness.Problems.Concat(generator.Warnings).Concat(readiness.Warnings).ToList(),
                LeftOut = input.LeftOut
            };
            t.GenerationReport = JsonConvert.SerializeObject(report);
            await _db.SaveChangesAsync();
            return null;
        }

        /// <summary>"MR WAFULA SALIM" -> "WS": a teacher's initials for the master timetable.</summary>
        public static string Initials(string? name)
        {
            var words = Regex.Replace(name ?? "", @"[^A-Za-z\s'-]", " ")
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Where(w => !Regex.IsMatch(w, @"^(mr|mrs|ms|md|miss|dr|prof|rev|sir|madam|mdm|tr|teacher)$", RegexOptions.IgnoreCase))
                .ToList();
            if (words.Count == 0) return "";
            return string.Concat(words.Take(3).Select(w => char.ToUpperInvariant(w[0])));
        }
    }
}
