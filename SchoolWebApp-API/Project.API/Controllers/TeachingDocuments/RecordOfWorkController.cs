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
    /// The record of work: the scheme's lessons with what was actually taught,
    /// and the head of department's weekly check. Kept by the teacher whatever
    /// the scheme's approval state; a checked week is locked until the check is
    /// removed.
    /// </summary>
    [Route("api/recordOfWork")]
    [ApiController]
    [Authorize]
    public class RecordOfWorkController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly TeachingDocumentService _svc;

        public RecordOfWorkController(ApplicationDbContext db, TeachingDocumentService svc)
        {
            _db = db;
            _svc = svc;
        }

        // GET api/recordOfWork?schemeOfWorkId=5
        [HttpGet]
        public async Task<IActionResult> Get(int schemeOfWorkId)
        {
            var me = await _svc.CurrentAsync();
            if (me == null) return Unauthorized();
            var s = await _svc.LoadSchemeAsync(schemeOfWorkId);
            if (s == null) return NotFound();
            if (!await _svc.CanViewAsync(me, s)) return Forbid();

            var lessons = await _db.SchemeOfWorkLessons.AsNoTracking().Include(l => l.RecordOfWorkEntry)
                .Where(l => l.SchemeOfWorkId == s.Id)
                .OrderBy(l => l.WeekNo).ThenBy(l => l.LessonNo).ThenBy(l => l.Id)
                .ToListAsync();
            var checks = await _db.RecordOfWorkChecks.AsNoTracking().Include(c => c.CheckedBy)
                .Where(c => c.SchemeOfWorkId == s.Id).OrderBy(c => c.WeekNo).ToListAsync();

            var scheme = await _svc.ToDtoAsync(s, me, withLessons: false);
            // Record keeping is not held up by the scheme's approval lock.
            scheme.CanEdit = me.CanEdit(s);
            var planned = lessons.Where(l => !string.IsNullOrWhiteSpace(l.SubStrandName)).ToList();
            return Ok(new RecordOfWorkDto
            {
                Scheme = scheme,
                CanCheck = me.CanCheck(s),
                Rows = lessons.Select(l => new RecordOfWorkRowDto
                {
                    SchemeOfWorkLessonId = l.Id,
                    WeekNo = l.WeekNo,
                    LessonNo = l.LessonNo,
                    StrandName = l.StrandName,
                    SubStrandName = l.SubStrandName,
                    LearningOutcomes = l.LearningOutcomes,
                    Status = l.RecordOfWorkEntry?.Status ?? LessonTaughtStatus.NotRecorded,
                    TaughtOn = l.RecordOfWorkEntry?.TaughtOn,
                    WorkCovered = l.RecordOfWorkEntry?.WorkCovered,
                    Remarks = l.RecordOfWorkEntry?.Remarks,
                    Modified = l.RecordOfWorkEntry?.Modified
                }).ToList(),
                Checks = checks.Select(ToDto).ToList(),
                Planned = planned.Count,
                Taught = planned.Count(l => l.RecordOfWorkEntry?.Status == LessonTaughtStatus.Taught),
                PartlyTaught = planned.Count(l => l.RecordOfWorkEntry?.Status == LessonTaughtStatus.PartlyTaught),
                NotTaught = planned.Count(l => l.RecordOfWorkEntry?.Status == LessonTaughtStatus.NotTaught)
            });
        }

        private static RecordOfWorkCheckDto ToDto(RecordOfWorkCheck c) => new()
        {
            Id = c.Id,
            WeekNo = c.WeekNo,
            CheckedByUserId = c.CheckedByUserId,
            CheckedByName = c.CheckedBy == null ? "" : $"{c.CheckedBy.FirstName} {c.CheckedBy.LastName}".Trim(),
            CheckedAt = c.CheckedAt,
            Comment = c.Comment
        };

        // PUT api/recordOfWork/entries?schemeOfWorkId=5
        [HttpPut("entries")]
        public async Task<IActionResult> SaveEntries(int schemeOfWorkId, List<SaveRecordOfWorkEntryDto> model)
        {
            var me = await _svc.CurrentAsync();
            if (me == null) return Unauthorized();
            var s = await _svc.LoadSchemeAsync(schemeOfWorkId);
            if (s == null) return NotFound();
            if (!me.CanEdit(s)) return StatusCode(403, new { message = "Only the teacher who owns this scheme keeps its record of work." });

            var ids = model.Select(m => m.SchemeOfWorkLessonId).ToList();
            var lessons = await _db.SchemeOfWorkLessons.Include(l => l.RecordOfWorkEntry)
                .Where(l => l.SchemeOfWorkId == s.Id && ids.Contains(l.Id)).ToListAsync();
            if (lessons.Count != ids.Distinct().Count()) return BadRequest(new { message = "Some lessons do not belong to this scheme." });
            var checkedWeeks = await _db.RecordOfWorkChecks.Where(c => c.SchemeOfWorkId == s.Id).Select(c => c.WeekNo).ToListAsync();

            foreach (var m in model)
            {
                var lesson = lessons.First(l => l.Id == m.SchemeOfWorkLessonId);
                var entry = lesson.RecordOfWorkEntry;
                var changed = entry == null
                    ? m.Status != LessonTaughtStatus.NotRecorded || !string.IsNullOrWhiteSpace(m.WorkCovered) || !string.IsNullOrWhiteSpace(m.Remarks)
                    : entry.Status != m.Status || entry.TaughtOn?.Date != m.TaughtOn?.Date || entry.WorkCovered != m.WorkCovered || entry.Remarks != m.Remarks;
                if (!changed) continue;
                if (checkedWeeks.Contains(lesson.WeekNo))
                    return BadRequest(new { message = $"Week {lesson.WeekNo} has been checked by the HOD; ask them to remove the check to change it." });
                if (m.Status is LessonTaughtStatus.Taught or LessonTaughtStatus.PartlyTaught && m.TaughtOn == null)
                    return BadRequest(new { message = $"Enter the date week {lesson.WeekNo} lesson {lesson.LessonNo} was taught." });
                if (entry == null)
                {
                    entry = new RecordOfWorkEntry { SchemeOfWorkLessonId = lesson.Id };
                    _db.RecordOfWorkEntries.Add(entry);
                }
                entry.Status = m.Status;
                entry.TaughtOn = m.TaughtOn?.Date;
                entry.WorkCovered = m.WorkCovered;
                entry.Remarks = m.Remarks;
            }
            await _db.SaveChangesAsync();
            return await Get(s.Id);
        }

        // POST api/recordOfWork/checks
        [HttpPost("checks")]
        public async Task<IActionResult> Check(CheckRecordOfWorkDto model)
        {
            var me = await _svc.CurrentAsync();
            if (me == null) return Unauthorized();
            var s = await _svc.LoadSchemeAsync(model.SchemeOfWorkId);
            if (s == null) return NotFound();
            if (!me.CanCheck(s))
                return StatusCode(403, new { message = "Only the head of the subject's department (or whole-school staff) can check another teacher's record of work." });
            if (model.WeekNo < 1 || model.WeekNo > s.NumberOfWeeks) return BadRequest(new { message = $"The scheme has {s.NumberOfWeeks} weeks." });

            var check = await _db.RecordOfWorkChecks.FirstOrDefaultAsync(c => c.SchemeOfWorkId == s.Id && c.WeekNo == model.WeekNo);
            if (check == null)
            {
                check = new RecordOfWorkCheck { SchemeOfWorkId = s.Id, WeekNo = model.WeekNo };
                _db.RecordOfWorkChecks.Add(check);
            }
            check.CheckedByUserId = me.UserId;
            check.CheckedAt = DateTime.Now;
            check.Comment = string.IsNullOrWhiteSpace(model.Comment) ? null : model.Comment.Trim();
            await _db.SaveChangesAsync();
            return await Get(s.Id);
        }

        // DELETE api/recordOfWork/checks/{id} - by whoever checked it, or an administrator.
        [HttpDelete("checks/{id}")]
        public async Task<IActionResult> RemoveCheck(int id)
        {
            var me = await _svc.CurrentAsync();
            if (me == null) return Unauthorized();
            var check = await _db.RecordOfWorkChecks.FirstOrDefaultAsync(c => c.Id == id);
            if (check == null) return NotFound();
            if (check.CheckedByUserId != me.UserId && !me.IsAdmin)
                return StatusCode(403, new { message = "Only the person who checked this week, or an administrator, can remove the check." });
            _db.RecordOfWorkChecks.Remove(check);
            await _db.SaveChangesAsync();
            return await Get(check.SchemeOfWorkId);
        }
    }
}
