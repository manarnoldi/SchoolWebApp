using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.API.Services.TeachingDocuments;
using SchoolWebApp.Core.DTOs.TeachingDocuments;
using SchoolWebApp.Core.Entities.TeachingDocuments;

namespace SchoolWebApp.API.Controllers.TeachingDocuments
{
    /// <summary>
    /// Lesson plans, a week at a time. A week is prepared from the scheme's
    /// lessons for that week and submitted for approval as one. Summary,
    /// reflection and whether the lesson was taught are written afterwards and
    /// stay editable once the week is approved; they feed the record of work.
    /// </summary>
    [Route("api/lessonPlans")]
    [ApiController]
    [Authorize]
    public class LessonPlansController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly TeachingDocumentService _svc;

        public LessonPlansController(ApplicationDbContext db, TeachingDocumentService svc)
        {
            _db = db;
            _svc = svc;
        }

        // GET api/lessonPlans/weeks?schemeOfWorkId=5
        [HttpGet("weeks")]
        public async Task<IActionResult> Weeks(int schemeOfWorkId)
        {
            var me = await _svc.CurrentAsync();
            if (me == null) return Unauthorized();
            var s = await _svc.LoadSchemeAsync(schemeOfWorkId, withLessons: true);
            if (s == null) return NotFound();
            if (!await _svc.CanViewAsync(me, s)) return Forbid();

            var due = await _svc.DueAsync();
            var weeks = await _db.LessonPlanWeeks.AsNoTracking().Where(w => w.SchemeOfWorkId == s.Id)
                .Select(w => new { w.Id, w.WeekNo, Plans = w.Plans.Count }).ToListAsync();
            var states = await _svc.ApprovalStatesAsync(TeachingDocumentService.EntityLessonPlanWeek, weeks.Select(w => w.Id));
            return Ok(TeachingDocumentService.Weeks(s.Session!, s.NumberOfWeeks, due).Select(w =>
            {
                var week = weeks.FirstOrDefault(x => x.WeekNo == w.WeekNo);
                var status = week == null ? DocumentStatus.NotStarted : TeachingDocumentService.StatusOf(states.GetValueOrDefault(week.Id));
                return new LessonPlanWeekSummaryDto
                {
                    WeekNo = w.WeekNo,
                    StartDate = w.StartDate,
                    EndDate = w.EndDate,
                    DueAt = w.DueAt,
                    LessonPlanWeekId = week?.Id,
                    Plans = week?.Plans ?? 0,
                    SchemeLessons = s.Lessons.Count(l => l.WeekNo == w.WeekNo),
                    Status = status,
                    Overdue = TeachingDocumentService.IsOverdue(status, w.DueAt)
                };
            }));
        }

        // POST api/lessonPlans/weeks - prepares the week's plans from the scheme.
        [HttpPost("weeks")]
        public async Task<IActionResult> CreateWeek(CreateLessonPlanWeekDto model)
        {
            var me = await _svc.CurrentAsync();
            if (me == null) return Unauthorized();
            var s = await _svc.LoadSchemeAsync(model.SchemeOfWorkId, withLessons: true);
            if (s == null) return NotFound();
            if (!me.CanEdit(s)) return StatusCode(403, new { message = "Only the teacher who owns this scheme can prepare its lesson plans." });
            if (model.WeekNo < 1 || model.WeekNo > s.NumberOfWeeks)
                return BadRequest(new { message = $"The scheme has {s.NumberOfWeeks} weeks." });
            var existing = await _db.LessonPlanWeeks.FirstOrDefaultAsync(w => w.SchemeOfWorkId == s.Id && w.WeekNo == model.WeekNo);
            if (existing != null) return Ok(new { id = existing.Id });

            var (weekStart, _) = TeachingDocumentService.WeekRange(s.Session!, model.WeekNo);
            var lessons = s.Lessons.Where(l => l.WeekNo == model.WeekNo).OrderBy(l => l.LessonNo).ToList();
            var week = new LessonPlanWeek { SchemeOfWorkId = s.Id, WeekNo = model.WeekNo };
            foreach (var l in lessons)
            {
                week.Plans.Add(FromScheme(l, SuggestedDate(weekStart, l.LessonNo, s.LessonsPerWeek)));
            }
            if (week.Plans.Count == 0) week.Plans.Add(new LessonPlan { LessonNo = 1, LessonDate = weekStart });
            _db.LessonPlanWeeks.Add(week);
            await _db.SaveChangesAsync();
            return Ok(new { id = week.Id });
        }

        // Spreads the week's lessons over Monday to Friday; the teacher corrects it to the timetable.
        private static DateTime SuggestedDate(DateTime weekStart, int lessonNo, int perWeek) =>
            weekStart.AddDays(Math.Min(4, (lessonNo - 1) * 5 / Math.Max(perWeek, 1)));

        private static LessonPlan FromScheme(SchemeOfWorkLesson l, DateTime? date)
        {
            var plan = new LessonPlan
            {
                SchemeOfWorkLessonId = l.Id,
                LessonNo = l.LessonNo,
                LessonDate = date,
                StrandId = l.StrandId,
                SubStrandId = l.SubStrandId,
                StrandName = l.StrandName,
                SubStrandName = l.SubStrandName,
                LearningOutcomes = l.LearningOutcomes,
                KeyInquiryQuestions = l.KeyInquiryQuestions,
                LearningResources = l.LearningResources
            };
            // The scheme's learning experiences become the lesson development steps.
            var steps = (l.LearningExperiences ?? "").Split('\n')
                .Select(x => x.Trim().TrimStart('-', '•', '●', '*').Trim())
                .Where(x => x.Length > 0 && !x.EndsWith(":"))
                .ToList();
            for (var i = 0; i < steps.Count; i++) plan.Steps.Add(new LessonPlanStep { StepNo = i + 1, Activity = steps[i] });
            return plan;
        }

        [HttpGet("weeks/{id}")]
        public async Task<IActionResult> GetWeek(int id)
        {
            var me = await _svc.CurrentAsync();
            if (me == null) return Unauthorized();
            var week = await _db.LessonPlanWeeks.AsNoTracking()
                .Include(w => w.Plans).ThenInclude(p => p.Steps)
                .Include(w => w.Plans).ThenInclude(p => p.SchemeOfWorkLesson).ThenInclude(l => l!.RecordOfWorkEntry)
                .FirstOrDefaultAsync(w => w.Id == id);
            if (week == null) return NotFound();
            var s = await _svc.LoadSchemeAsync(week.SchemeOfWorkId);
            if (s == null) return NotFound();
            if (!await _svc.CanViewAsync(me, s)) return Forbid();
            return Ok(await ToDtoAsync(week, s, me));
        }

        private async Task<LessonPlanWeekDto> ToDtoAsync(LessonPlanWeek week, SchemeOfWork s, TeachingUser me)
        {
            var due = await _svc.DueAsync();
            var (start, end) = TeachingDocumentService.WeekRange(s.Session!, week.WeekNo);
            var state = (await _svc.ApprovalStatesAsync(TeachingDocumentService.EntityLessonPlanWeek, new[] { week.Id })).GetValueOrDefault(week.Id);
            var locked = await _svc.IsLockedAsync(TeachingDocumentService.EntityLessonPlanWeek, week.Id);
            return new LessonPlanWeekDto
            {
                Id = week.Id,
                WeekNo = week.WeekNo,
                StartDate = start,
                EndDate = end,
                DueAt = TeachingDocumentService.LessonPlanDueAt(s.Session!, week.WeekNo, due),
                Status = TeachingDocumentService.StatusOf(state),
                IsLocked = locked,
                CanEdit = me.CanEdit(s),
                Scheme = await _svc.ToDtoAsync(s, me, withLessons: false),
                Plans = week.Plans.OrderBy(p => p.LessonNo).ThenBy(p => p.Id).Select(ToDto).ToList()
            };
        }

        private static LessonPlanDto ToDto(LessonPlan p) => new()
        {
            Id = p.Id,
            LessonPlanWeekId = p.LessonPlanWeekId,
            SchemeOfWorkLessonId = p.SchemeOfWorkLessonId,
            LessonNo = p.LessonNo,
            LessonDate = p.LessonDate,
            StartTime = p.StartTime,
            EndTime = p.EndTime,
            Roll = p.Roll,
            StrandId = p.StrandId,
            SubStrandId = p.SubStrandId,
            StrandName = p.StrandName,
            SubStrandName = p.SubStrandName,
            LearningOutcomes = p.LearningOutcomes,
            KeyInquiryQuestions = p.KeyInquiryQuestions,
            LearningResources = p.LearningResources,
            OrganisationOfLearning = p.OrganisationOfLearning,
            Introduction = p.Introduction,
            IntroductionMinutes = p.IntroductionMinutes,
            DevelopmentMinutes = p.DevelopmentMinutes,
            Conclusion = p.Conclusion,
            ConclusionMinutes = p.ConclusionMinutes,
            ExtendedActivities = p.ExtendedActivities,
            Summary = p.Summary,
            Reflection = p.Reflection,
            Steps = p.Steps.OrderBy(x => x.StepNo).Select(x => new LessonPlanStepDto { Id = x.Id, StepNo = x.StepNo, Activity = x.Activity }).ToList(),
            TaughtStatus = p.SchemeOfWorkLesson?.RecordOfWorkEntry?.Status ?? LessonTaughtStatus.NotRecorded,
            TaughtOn = p.SchemeOfWorkLesson?.RecordOfWorkEntry?.TaughtOn
        };

        /// <summary>Loads a week for a change, enforcing ownership and (unless afterLesson) the approval lock.</summary>
        private async Task<(LessonPlanWeek? Week, SchemeOfWork? Scheme, IActionResult? Error)> LoadWeekForEditAsync(int weekId, bool afterLesson = false)
        {
            var me = await _svc.CurrentAsync();
            if (me == null) return (null, null, Unauthorized());
            var week = await _db.LessonPlanWeeks.Include(w => w.Plans).ThenInclude(p => p.Steps)
                .FirstOrDefaultAsync(w => w.Id == weekId);
            if (week == null) return (null, null, NotFound());
            var s = await _svc.LoadSchemeAsync(week.SchemeOfWorkId);
            if (s == null) return (null, null, NotFound());
            if (!me.CanEdit(s)) return (null, null, StatusCode(403, new { message = "Only the teacher who owns these lesson plans can change them." }));
            if (!afterLesson && await _svc.IsLockedAsync(TeachingDocumentService.EntityLessonPlanWeek, week.Id))
                return (null, null, BadRequest(new { message = "This week's plans have been submitted or approved and can no longer be edited." }));
            return (week, s, null);
        }

        private async Task<int?> WeekIdOfPlanAsync(int planId) =>
            await _db.LessonPlans.Where(p => p.Id == planId).Select(p => (int?)p.LessonPlanWeekId).FirstOrDefaultAsync();

        [HttpPut("plans/{id}")]
        public async Task<IActionResult> SavePlan(int id, LessonPlanDto model)
        {
            var weekId = await WeekIdOfPlanAsync(id);
            if (weekId == null) return NotFound();
            var (week, s, error) = await LoadWeekForEditAsync(weekId.Value);
            if (error != null) return error;
            var plan = week!.Plans.First(p => p.Id == id);
            if (!ValidTime(model.StartTime) || !ValidTime(model.EndTime)) return BadRequest(new { message = "Times must be HH:mm." });

            plan.LessonNo = Math.Max(1, model.LessonNo);
            plan.LessonDate = model.LessonDate?.Date;
            plan.StartTime = Blank(model.StartTime);
            plan.EndTime = Blank(model.EndTime);
            plan.Roll = model.Roll is >= 0 ? model.Roll : null;
            plan.StrandId = model.StrandId > 0 ? model.StrandId : null;
            plan.SubStrandId = model.SubStrandId > 0 ? model.SubStrandId : null;
            plan.StrandName = Cut(model.StrandName, 255);
            plan.SubStrandName = Cut(model.SubStrandName, 255);
            plan.LearningOutcomes = model.LearningOutcomes;
            plan.KeyInquiryQuestions = model.KeyInquiryQuestions;
            plan.LearningResources = model.LearningResources;
            plan.OrganisationOfLearning = model.OrganisationOfLearning;
            plan.Introduction = model.Introduction;
            plan.IntroductionMinutes = Minutes(model.IntroductionMinutes);
            plan.DevelopmentMinutes = Minutes(model.DevelopmentMinutes);
            plan.Conclusion = model.Conclusion;
            plan.ConclusionMinutes = Minutes(model.ConclusionMinutes);
            plan.ExtendedActivities = model.ExtendedActivities;
            plan.Summary = model.Summary;
            plan.Reflection = model.Reflection;

            _db.LessonPlanSteps.RemoveRange(plan.Steps);
            var steps = model.Steps.Where(x => !string.IsNullOrWhiteSpace(x.Activity)).OrderBy(x => x.StepNo).ToList();
            for (var i = 0; i < steps.Count; i++) plan.Steps.Add(new LessonPlanStep { StepNo = i + 1, Activity = steps[i].Activity!.Trim() });
            await _db.SaveChangesAsync();
            return await GetWeek(week.Id);
        }

        private static bool ValidTime(string? t) =>
            string.IsNullOrWhiteSpace(t) || TimeSpan.TryParseExact(t, @"hh\:mm", null, out _);
        private static string? Blank(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();
        private static string? Cut(string? s, int max) => Blank(s) is string v && v.Length > max ? v[..max] : Blank(s);
        private static int? Minutes(int? m) => m is > 0 and < 600 ? m : null;

        // POST api/lessonPlans/weeks/{id}/plans - an extra lesson in the week.
        [HttpPost("weeks/{id}/plans")]
        public async Task<IActionResult> AddPlan(int id)
        {
            var (week, s, error) = await LoadWeekForEditAsync(id);
            if (error != null) return error;
            var next = week!.Plans.Select(p => p.LessonNo).DefaultIfEmpty(0).Max() + 1;
            var (start, _) = TeachingDocumentService.WeekRange(s!.Session!, week.WeekNo);
            week.Plans.Add(new LessonPlan { LessonNo = next, LessonDate = start });
            await _db.SaveChangesAsync();
            return await GetWeek(week.Id);
        }

        [HttpDelete("plans/{id}")]
        public async Task<IActionResult> DeletePlan(int id)
        {
            var weekId = await WeekIdOfPlanAsync(id);
            if (weekId == null) return NotFound();
            var (week, _, error) = await LoadWeekForEditAsync(weekId.Value);
            if (error != null) return error;
            _db.LessonPlans.Remove(week!.Plans.First(p => p.Id == id));
            await _db.SaveChangesAsync();
            return await GetWeek(week.Id);
        }

        /// <summary>
        /// After the lesson: summary, reflection and whether it was taught. The
        /// taught status and date go to the record of work of the scheme lesson
        /// the plan came from, with the summary as work covered and the
        /// reflection as remarks.
        /// </summary>
        [HttpPut("plans/{id}/afterLesson")]
        public async Task<IActionResult> AfterLesson(int id, LessonPlanAfterLessonDto model)
        {
            var weekId = await WeekIdOfPlanAsync(id);
            if (weekId == null) return NotFound();
            var (week, s, error) = await LoadWeekForEditAsync(weekId.Value, afterLesson: true);
            if (error != null) return error;
            var plan = week!.Plans.First(p => p.Id == id);
            plan.Summary = model.Summary;
            plan.Reflection = model.Reflection;

            if (plan.SchemeOfWorkLessonId is int lessonId && model.TaughtStatus != LessonTaughtStatus.NotRecorded)
            {
                var lesson = await _db.SchemeOfWorkLessons.Include(l => l.RecordOfWorkEntry).FirstAsync(l => l.Id == lessonId);
                if (await _db.RecordOfWorkChecks.AnyAsync(c => c.SchemeOfWorkId == s!.Id && c.WeekNo == lesson.WeekNo))
                    return BadRequest(new { message = $"Week {lesson.WeekNo}'s record of work has been checked; ask your HOD to remove the check to change it." });
                var entry = lesson.RecordOfWorkEntry ?? new RecordOfWorkEntry { SchemeOfWorkLessonId = lessonId };
                if (entry.Id == 0) _db.RecordOfWorkEntries.Add(entry);
                entry.Status = model.TaughtStatus;
                entry.TaughtOn = model.TaughtOn?.Date ?? plan.LessonDate;
                if (!string.IsNullOrWhiteSpace(model.Summary)) entry.WorkCovered = model.Summary;
                if (!string.IsNullOrWhiteSpace(model.Reflection)) entry.Remarks = model.Reflection;
            }
            await _db.SaveChangesAsync();
            return await GetWeek(week.Id);
        }

        [HttpDelete("weeks/{id}")]
        public async Task<IActionResult> DeleteWeek(int id)
        {
            var (week, _, error) = await LoadWeekForEditAsync(id);
            if (error != null) return error;
            var requests = await _db.ApprovalRequests.Include(r => r.Actions)
                .Where(r => r.EntityType == TeachingDocumentService.EntityLessonPlanWeek && r.EntityId == id).ToListAsync();
            _db.ApprovalStepActions.RemoveRange(requests.SelectMany(r => r.Actions));
            _db.ApprovalRequests.RemoveRange(requests);
            _db.LessonPlanWeeks.Remove(week!);
            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}
