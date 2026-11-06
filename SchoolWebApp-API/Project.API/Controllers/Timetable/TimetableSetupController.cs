using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.API.Licensing;
using SchoolWebApp.API.Security;
using SchoolWebApp.API.Services.Timetable;
using SchoolWebApp.Core.DTOs.Timetable;
using SchoolWebApp.Core.Entities.Timetable;

namespace SchoolWebApp.API.Controllers.Timetable
{
    /// <summary>
    /// Timetable set-up: bell schedules, subject rules, option groups, teacher
    /// limits, and the readiness check before generating. Reading is open to
    /// signed-in staff; changes need the Timetable menus.
    /// </summary>
    [Route("api/timetableSetup")]
    [ApiController]
    [Authorize]
    [RequiresModule(AppModules.Academics)]
    [RequiresMenu(MenuGroup.Timetable, WritesOnly = true)]
    public class TimetableSetupController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly TimetableInputService _input;

        public TimetableSetupController(ApplicationDbContext db, TimetableInputService input)
        {
            _db = db;
            _input = input;
        }

        // GET api/timetableSetup/lookups - grades, subjects, teachers and terms for the pickers
        [HttpGet("lookups")]
        public async Task<IActionResult> Lookups()
        {
            var levels = await _db.LearningLevels.AsNoTracking().Include(l => l.EducationLevel)
                .OrderBy(l => l.EducationLevel!.Rank).ThenBy(l => l.Rank)
                .Select(l => new { l.Id, l.Name, EducationLevel = l.EducationLevel!.Name, l.Rank })
                .ToListAsync();
            var subjects = await _db.Subjects.AsNoTracking().OrderBy(s => s.Rank).ThenBy(s => s.Name)
                .Select(s => new { s.Id, s.Name, s.Abbr }).ToListAsync();
            var teachers = await _db.StaffDetails.AsNoTracking().Where(s => s.CurrentlyEmployed)
                .OrderBy(s => s.FullName).Select(s => new { s.Id, Name = s.FullName }).ToListAsync();
            var sessions = await _db.Sessions.AsNoTracking().Include(s => s.AcademicYear)
                .OrderByDescending(s => s.StartDate)
                .Select(s => new { s.Id, Name = s.AcademicYear!.Name + " " + s.SessionName, s.StartDate, s.EndDate })
                .ToListAsync();
            var today = DateTime.Today;
            var current = sessions.FirstOrDefault(s => s.StartDate.Date <= today && today <= s.EndDate.Date)
                ?? sessions.FirstOrDefault(s => s.StartDate.Date > today)
                ?? sessions.FirstOrDefault();
            return Ok(new { levels, subjects, teachers, sessions, currentSessionId = current?.Id });
        }

        // ---------------------------------------------------------------- bell schedules

        // GET api/timetableSetup/bellSchedules
        [HttpGet("bellSchedules")]
        public async Task<IActionResult> BellSchedules()
        {
            var schedules = await _db.BellSchedules.AsNoTracking()
                .Include(b => b.Slots).Include(b => b.Levels).Include(b => b.FixedSlots)
                .OrderBy(b => b.Name).ToListAsync();
            return Ok(schedules.Select(ToDto));
        }

        // POST api/timetableSetup/bellSchedules
        [HttpPost("bellSchedules")]
        public async Task<IActionResult> CreateBellSchedule([FromBody] BellScheduleDto dto)
        {
            var schedule = new BellSchedule { Name = dto.Name.Trim() };
            _db.BellSchedules.Add(schedule);
            var error = await ApplyAsync(schedule, dto);
            if (error != null) return BadRequest(new { message = error });
            await _db.SaveChangesAsync();
            await SaveFixedSlotsAsync(schedule, dto);
            return Ok(new { id = schedule.Id });
        }

        // PUT api/timetableSetup/bellSchedules/5 - the whole schedule: days, slots, grades, fixed slots
        [HttpPut("bellSchedules/{id:int}")]
        public async Task<IActionResult> UpdateBellSchedule(int id, [FromBody] BellScheduleDto dto)
        {
            var schedule = await _db.BellSchedules
                .Include(b => b.Slots).Include(b => b.Levels).Include(b => b.FixedSlots)
                .FirstOrDefaultAsync(b => b.Id == id);
            if (schedule == null) return NotFound(new { message = "Bell schedule not found." });
            var error = await ApplyAsync(schedule, dto);
            if (error != null) return BadRequest(new { message = error });
            await _db.SaveChangesAsync();
            await SaveFixedSlotsAsync(schedule, dto);
            return Ok(new { id = schedule.Id });
        }

        // DELETE api/timetableSetup/bellSchedules/5
        [HttpDelete("bellSchedules/{id:int}")]
        public async Task<IActionResult> DeleteBellSchedule(int id)
        {
            var schedule = await _db.BellSchedules.Include(b => b.Slots).FirstOrDefaultAsync(b => b.Id == id);
            if (schedule == null) return NotFound(new { message = "Bell schedule not found." });
            var slotIds = schedule.Slots.Select(s => s.Id).ToList();
            if (await _db.TimetableLessons.AnyAsync(l => l.BellSlotId != null && slotIds.Contains(l.BellSlotId.Value)))
                return BadRequest(new { message = "A timetable has lessons on this schedule. Delete or regenerate that timetable first." });
            _db.TimetableFixedSlots.RemoveRange(_db.TimetableFixedSlots.Where(f => f.BellScheduleId == id));
            _db.BellSchedules.Remove(schedule);
            await _db.SaveChangesAsync();
            return Ok();
        }

        /// <summary>Copies the DTO onto the schedule; returns why it cannot be saved, or null.</summary>
        private async Task<string?> ApplyAsync(BellSchedule schedule, BellScheduleDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name)) return "Give the schedule a name.";
            var days = dto.Days.Where(d => d >= 1 && d <= 7).Distinct().OrderBy(d => d).ToList();
            if (days.Count == 0) return "Pick at least one teaching day.";
            if (dto.Slots.Count == 0) return "Add the slots of the day.";
            if (!dto.Slots.Any(s => s.Kind == BellSlotKind.Lesson)) return "The day needs at least one lesson slot.";

            var parsed = new List<(BellSlotDto Dto, TimeOnly Start, TimeOnly End)>();
            foreach (var s in dto.Slots)
            {
                if (!TryTime(s.StartTime, out var start) || !TryTime(s.EndTime, out var end))
                    return $"Slot {parsed.Count + 1}: enter start and end times as HH:mm.";
                if (end <= start) return $"Slot {parsed.Count + 1} ({s.StartTime}): ends before it starts.";
                parsed.Add((s, start, end));
            }
            // In time order and not overlapping. Fixed slots refer to slots by their
            // position in this list, so it is never re-sorted here.
            for (var i = 1; i < parsed.Count; i++)
                if (parsed[i].Start < parsed[i - 1].End)
                    return $"Slots {parsed[i - 1].Dto.StartTime} and {parsed[i].Dto.StartTime} overlap or are out of order.";

            var levelIds = dto.LearningLevelIds.Distinct().ToList();
            var taken = await _db.BellScheduleLevels.AsNoTracking()
                .Where(l => levelIds.Contains(l.LearningLevelId) && l.BellScheduleId != schedule.Id)
                .Select(l => new { Level = l.LearningLevel!.Name, Schedule = l.BellSchedule!.Name })
                .FirstOrDefaultAsync();
            if (taken != null) return $"{taken.Level} already follows the {taken.Schedule} schedule.";

            // Slots kept by id; removed ones must not hold timetable lessons.
            var keepIds = parsed.Where(p => p.Dto.Id > 0).Select(p => p.Dto.Id).ToHashSet();
            var removed = schedule.Slots.Where(s => !keepIds.Contains(s.Id)).ToList();
            if (removed.Count > 0)
            {
                var removedIds = removed.Select(s => s.Id).ToList();
                if (await _db.TimetableLessons.AnyAsync(l => l.BellSlotId != null && removedIds.Contains(l.BellSlotId.Value)))
                    return "A timetable has lessons in a slot you removed. Delete or regenerate that timetable first, or keep the slot.";
                _db.TimetableFixedSlots.RemoveRange(schedule.FixedSlots.Where(f => removedIds.Contains(f.BellSlotId)));
                _db.BellSlots.RemoveRange(removed);
            }

            schedule.Name = dto.Name.Trim();
            schedule.Description = dto.Description?.Trim();
            schedule.Days = string.Join(",", days);
            for (var i = 0; i < parsed.Count; i++)
            {
                var (s, start, end) = parsed[i];
                var slot = s.Id > 0 ? schedule.Slots.FirstOrDefault(x => x.Id == s.Id) : null;
                if (slot == null)
                {
                    slot = new BellSlot();
                    schedule.Slots.Add(slot);
                }
                slot.Rank = i + 1;
                slot.StartTime = start;
                slot.EndTime = end;
                slot.Kind = s.Kind;
                slot.Part = s.Part;
                slot.Label = string.IsNullOrWhiteSpace(s.Label) ? null : s.Label.Trim();
            }

            _db.BellScheduleLevels.RemoveRange(schedule.Levels.Where(l => !levelIds.Contains(l.LearningLevelId)));
            foreach (var levelId in levelIds.Where(l => !schedule.Levels.Any(x => x.LearningLevelId == l)))
                schedule.Levels.Add(new BellScheduleLevel { LearningLevelId = levelId });
            return null;
        }

        /// <summary>Fixed slots point at slots by position in the day, so they are saved once slots have ids.</summary>
        private async Task SaveFixedSlotsAsync(BellSchedule schedule, BellScheduleDto dto)
        {
            var slots = schedule.Slots.OrderBy(s => s.Rank).ToList();
            var existing = await _db.TimetableFixedSlots.Where(f => f.BellScheduleId == schedule.Id).ToListAsync();
            _db.TimetableFixedSlots.RemoveRange(existing);
            var days = TimetableInputService.ParseDays(schedule.Days);
            foreach (var f in dto.FixedSlots)
            {
                if (f.SlotIndex < 0 || f.SlotIndex >= slots.Count || !days.Contains(f.Day)) continue;
                if (f.SubjectId == null && string.IsNullOrWhiteSpace(f.Label)) continue;
                _db.TimetableFixedSlots.Add(new TimetableFixedSlot
                {
                    BellScheduleId = schedule.Id,
                    BellSlotId = slots[f.SlotIndex].Id,
                    Day = f.Day,
                    LearningLevelId = f.LearningLevelId,
                    SubjectId = f.SubjectId,
                    Label = string.IsNullOrWhiteSpace(f.Label) ? null : f.Label.Trim()
                });
            }
            await _db.SaveChangesAsync();
        }

        private static BellScheduleDto ToDto(BellSchedule b)
        {
            var slots = b.Slots.OrderBy(s => s.Rank).ToList();
            return new BellScheduleDto
            {
                Id = b.Id,
                Name = b.Name,
                Description = b.Description,
                Days = TimetableInputService.ParseDays(b.Days),
                Slots = slots.Select(s => new BellSlotDto
                {
                    Id = s.Id,
                    StartTime = s.StartTime.ToString("HH:mm"),
                    EndTime = s.EndTime.ToString("HH:mm"),
                    Kind = s.Kind,
                    Part = s.Part,
                    Label = s.Label
                }).ToList(),
                LearningLevelIds = b.Levels.Select(l => l.LearningLevelId).ToList(),
                FixedSlots = b.FixedSlots.Select(f => new FixedSlotDto
                {
                    Id = f.Id,
                    Day = f.Day,
                    SlotIndex = slots.FindIndex(s => s.Id == f.BellSlotId),
                    LearningLevelId = f.LearningLevelId,
                    SubjectId = f.SubjectId,
                    Label = f.Label
                }).Where(f => f.SlotIndex >= 0).ToList()
            };
        }

        private static bool TryTime(string? value, out TimeOnly time) =>
            TimeOnly.TryParseExact(value?.Trim(), new[] { "HH:mm", "H:mm" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out time);

        // ---------------------------------------------------------------- subject rules and option groups

        // GET api/timetableSetup/subjectRules
        [HttpGet("subjectRules")]
        public async Task<IActionResult> SubjectRules() =>
            Ok(await _db.TimetableSubjectRules.AsNoTracking().Select(r => new TimetableSubjectRuleDto
            {
                Id = r.Id,
                SubjectId = r.SubjectId,
                LearningLevelId = r.LearningLevelId,
                Excluded = r.Excluded,
                Placement = r.Placement,
                DoubleLessons = r.DoubleLessons,
                MaxPerDay = r.MaxPerDay,
                AvoidLastLesson = r.AvoidLastLesson
            }).ToListAsync());

        // PUT api/timetableSetup/subjectRules - the full list; rules left out are removed
        [HttpPut("subjectRules")]
        public async Task<IActionResult> SaveSubjectRules([FromBody] List<TimetableSubjectRuleDto> rules)
        {
            var dupe = rules.GroupBy(r => (r.SubjectId, r.LearningLevelId)).FirstOrDefault(g => g.Count() > 1);
            if (dupe != null) return BadRequest(new { message = "A subject has two rules for the same grade." });
            foreach (var r in rules)
            {
                if (r.DoubleLessons < 0 || r.MaxPerDay < 1 || r.MaxPerDay > 4)
                    return BadRequest(new { message = "Doubles cannot be negative and lessons a day must be 1 to 4." });
            }
            var existing = await _db.TimetableSubjectRules.ToListAsync();
            _db.TimetableSubjectRules.RemoveRange(existing.Where(e => !rules.Any(r => r.SubjectId == e.SubjectId && r.LearningLevelId == e.LearningLevelId)));
            foreach (var r in rules)
            {
                var row = existing.FirstOrDefault(e => e.SubjectId == r.SubjectId && e.LearningLevelId == r.LearningLevelId);
                if (row == null) _db.TimetableSubjectRules.Add(row = new TimetableSubjectRule { SubjectId = r.SubjectId, LearningLevelId = r.LearningLevelId });
                row.Excluded = r.Excluded;
                row.Placement = r.Placement;
                row.DoubleLessons = r.DoubleLessons;
                row.MaxPerDay = r.MaxPerDay;
                row.AvoidLastLesson = r.AvoidLastLesson;
            }
            await _db.SaveChangesAsync();
            return Ok();
        }

        // GET api/timetableSetup/optionGroups
        [HttpGet("optionGroups")]
        public async Task<IActionResult> OptionGroups() =>
            Ok(await _db.TimetableOptionGroups.AsNoTracking().OrderBy(g => g.Name).Select(g => new TimetableOptionGroupDto
            {
                Id = g.Id,
                Name = g.Name,
                LearningLevelId = g.LearningLevelId,
                SubjectIds = g.Subjects.Select(s => s.SubjectId).ToList()
            }).ToListAsync());

        // PUT api/timetableSetup/optionGroups - the full list; groups left out are removed
        [HttpPut("optionGroups")]
        public async Task<IActionResult> SaveOptionGroups([FromBody] List<TimetableOptionGroupDto> groups)
        {
            foreach (var g in groups)
            {
                if (string.IsNullOrWhiteSpace(g.Name)) return BadRequest(new { message = "Give each option group a name." });
                if (g.SubjectIds.Distinct().Count() < 2) return BadRequest(new { message = $"{g.Name}: an option group needs at least two subjects." });
            }
            var existing = await _db.TimetableOptionGroups.Include(g => g.Subjects).ToListAsync();
            _db.TimetableOptionGroups.RemoveRange(existing.Where(e => !groups.Any(g => g.Id == e.Id)));
            foreach (var g in groups)
            {
                var row = existing.FirstOrDefault(e => e.Id == g.Id && g.Id > 0);
                if (row == null) _db.TimetableOptionGroups.Add(row = new TimetableOptionGroup { Name = g.Name.Trim() });
                row.Name = g.Name.Trim();
                row.LearningLevelId = g.LearningLevelId;
                var ids = g.SubjectIds.Distinct().ToList();
                _db.TimetableOptionGroupSubjects.RemoveRange(row.Subjects.Where(s => !ids.Contains(s.SubjectId)));
                foreach (var id in ids.Where(id => !row.Subjects.Any(s => s.SubjectId == id)))
                    row.Subjects.Add(new TimetableOptionGroupSubject { SubjectId = id });
            }
            await _db.SaveChangesAsync();
            return Ok();
        }

        // ---------------------------------------------------------------- teachers

        // GET api/timetableSetup/teacherRules - teachers with limits or times off
        [HttpGet("teacherRules")]
        public async Task<IActionResult> TeacherRules()
        {
            var rules = await _db.TeacherTimetableRules.AsNoTracking().Include(r => r.StaffDetails).ToListAsync();
            var off = await _db.TeacherUnavailabilities.AsNoTracking().Include(u => u.StaffDetails).ToListAsync();
            var staffIds = rules.Select(r => r.StaffDetailsId).Union(off.Select(u => u.StaffDetailsId)).Distinct();
            var rows = staffIds.Select(id =>
            {
                var rule = rules.FirstOrDefault(r => r.StaffDetailsId == id);
                var mine = off.Where(u => u.StaffDetailsId == id).OrderBy(u => u.Day).ThenBy(u => u.StartTime).ToList();
                return new TeacherTimetableRuleDto
                {
                    StaffDetailsId = id,
                    StaffName = rule?.StaffDetails?.FullName ?? mine.FirstOrDefault()?.StaffDetails?.FullName,
                    MaxLessonsPerDay = rule?.MaxLessonsPerDay,
                    MaxConsecutiveLessons = rule?.MaxConsecutiveLessons,
                    Notes = rule?.Notes,
                    Unavailable = mine.Select(u => new TeacherUnavailabilityDto
                    {
                        Id = u.Id,
                        Day = u.Day,
                        StartTime = u.StartTime.ToString("HH:mm"),
                        EndTime = u.EndTime.ToString("HH:mm"),
                        Reason = u.Reason
                    }).ToList()
                };
            }).OrderBy(r => r.StaffName).ToList();
            return Ok(rows);
        }

        // PUT api/timetableSetup/teacherRules/12 - one teacher's limits and times off (empty clears them)
        [HttpPut("teacherRules/{staffId:int}")]
        public async Task<IActionResult> SaveTeacherRule(int staffId, [FromBody] TeacherTimetableRuleDto dto)
        {
            if (!await _db.StaffDetails.AnyAsync(s => s.Id == staffId)) return NotFound(new { message = "Teacher not found." });
            if (dto.MaxLessonsPerDay is < 1 || dto.MaxConsecutiveLessons is < 1)
                return BadRequest(new { message = "Limits must be at least 1, or left blank." });
            var times = new List<TeacherUnavailability>();
            foreach (var u in dto.Unavailable)
            {
                if (u.Day < 1 || u.Day > 7 || !TryTime(u.StartTime, out var start) || !TryTime(u.EndTime, out var end) || end <= start)
                    return BadRequest(new { message = "Each time off needs a day and a start before its end (HH:mm)." });
                times.Add(new TeacherUnavailability { StaffDetailsId = staffId, Day = u.Day, StartTime = start, EndTime = end, Reason = u.Reason?.Trim() });
            }

            var rule = await _db.TeacherTimetableRules.FirstOrDefaultAsync(r => r.StaffDetailsId == staffId);
            var empty = dto.MaxLessonsPerDay == null && dto.MaxConsecutiveLessons == null && string.IsNullOrWhiteSpace(dto.Notes);
            if (empty && rule != null) _db.TeacherTimetableRules.Remove(rule);
            else if (!empty)
            {
                if (rule == null) _db.TeacherTimetableRules.Add(rule = new TeacherTimetableRule { StaffDetailsId = staffId });
                rule.MaxLessonsPerDay = dto.MaxLessonsPerDay;
                rule.MaxConsecutiveLessons = dto.MaxConsecutiveLessons;
                rule.Notes = dto.Notes?.Trim();
            }
            _db.TeacherUnavailabilities.RemoveRange(_db.TeacherUnavailabilities.Where(u => u.StaffDetailsId == staffId));
            _db.TeacherUnavailabilities.AddRange(times);
            await _db.SaveChangesAsync();
            return Ok();
        }

        // ---------------------------------------------------------------- readiness

        // GET api/timetableSetup/readiness?sessionId=6
        [HttpGet("readiness")]
        public async Task<IActionResult> Readiness(int sessionId)
        {
            var input = await _input.LoadAsync(sessionId);
            if (input == null) return NotFound(new { message = "Term not found." });
            return Ok(_input.Readiness(input));
        }
    }
}
