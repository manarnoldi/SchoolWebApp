using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.API.Services.TeachingDocuments;
using SchoolWebApp.Core.DTOs.TeachingDocuments;
using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.TeachingDocuments;

namespace SchoolWebApp.API.Controllers.TeachingDocuments
{
    /// <summary>
    /// What the teaching document screens share: the caller's context, their
    /// subject allocations, the curriculum for the pickers, and the tracker.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TeachingDocumentsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly TeachingDocumentService _svc;

        public TeachingDocumentsController(ApplicationDbContext db, TeachingDocumentService svc)
        {
            _db = db;
            _svc = svc;
        }

        // GET api/teachingDocuments/context
        [HttpGet("context")]
        public async Task<IActionResult> Context()
        {
            var me = await _svc.CurrentAsync();
            if (me == null) return Unauthorized();
            var sessions = await _db.Sessions.AsNoTracking().Include(s => s.AcademicYear)
                .OrderByDescending(s => s.StartDate).ToListAsync();
            var current = sessions.FirstOrDefault(TeachingDocumentService.IsCurrent)
                ?? sessions.Where(s => s.StartDate.Date <= DateTime.Today).OrderByDescending(s => s.StartDate).FirstOrDefault()
                ?? sessions.LastOrDefault();
            string? staffName = me.StaffDetailsId == null ? null
                : await _db.StaffDetails.Where(s => s.Id == me.StaffDetailsId).Select(s => s.FullName).FirstOrDefaultAsync();
            return Ok(new TeachingContextDto
            {
                UserId = me.UserId,
                StaffDetailsId = me.StaffDetailsId,
                StaffName = staffName ?? me.Name,
                IsAdmin = me.IsAdmin,
                WholeSchool = me.WholeSchool,
                HodDepartmentIds = me.HodDepartmentIds.ToList(),
                Sessions = sessions.Select(TeachingDocumentService.ToSessionDto).ToList(),
                CurrentSessionId = current?.Id
            });
        }

        // GET api/teachingDocuments/assignments?sessionId=6&scope=mine|oversee
        // "mine": the caller's own subject allocations. "oversee": everything the
        // caller can see - the whole school, or their departments plus their own.
        [HttpGet("assignments")]
        public async Task<IActionResult> Assignments(int sessionId, string scope = "mine")
        {
            var me = await _svc.CurrentAsync();
            if (me == null) return Unauthorized();
            var session = await _db.Sessions.AsNoTracking().FirstOrDefaultAsync(s => s.Id == sessionId);
            if (session == null) return NotFound(new { message = "Term not found." });
            var rows = await AssignmentsAsync(session, me, scope == "oversee");
            return Ok(rows);
        }

        private async Task<List<TeachingAssignmentDto>> AssignmentsAsync(Session session, TeachingUser me, bool oversee)
        {
            var mine = me.StaffDetailsId ?? -1;
            var allocations = _db.StaffSubjects.AsNoTracking()
                .Where(a => a.SchoolClass!.AcademicYearId == session.AcademicYearId);
            var schemes = _db.SchemesOfWork.AsNoTracking().Where(s => s.SessionId == session.Id);
            if (!oversee)
            {
                allocations = allocations.Where(a => a.StaffDetailsId == mine);
                schemes = schemes.Where(s => s.StaffDetailsId == mine);
            }
            else if (!me.WholeSchool)
            {
                var depts = me.HodDepartmentIds.ToList();
                allocations = allocations.Where(a => a.StaffDetailsId == mine || depts.Contains(a.Subject!.DepartmentId));
                schemes = schemes.Where(s => s.StaffDetailsId == mine || depts.Contains(s.Subject!.DepartmentId));
            }

            var fromAllocations = await allocations.Select(a => new { a.StaffDetailsId, a.SubjectId, a.SchoolClassId }).ToListAsync();
            var schemeList = await schemes.Select(s => new
            {
                s.Id, s.StaffDetailsId, s.SubjectId, s.SchoolClassId,
                Lessons = s.Lessons.Count(l => l.SubStrandName != null && l.SubStrandName != "")
            }).ToListAsync();

            // A scheme whose allocation has since been removed still shows.
            var keys = fromAllocations.Select(a => (a.StaffDetailsId, a.SubjectId, a.SchoolClassId))
                .Concat(schemeList.Select(s => (s.StaffDetailsId, s.SubjectId, s.SchoolClassId)))
                .Distinct().ToList();
            if (keys.Count == 0) return new();

            var staffIds = keys.Select(k => k.Item1).Distinct().ToList();
            var subjectIds = keys.Select(k => k.Item2).Distinct().ToList();
            var classIds = keys.Select(k => k.Item3).Distinct().ToList();
            var staff = await _db.StaffDetails.AsNoTracking().Where(s => staffIds.Contains(s.Id))
                .ToDictionaryAsync(s => s.Id, s => s.FullName);
            var subjects = await _db.Subjects.AsNoTracking().Include(s => s.Department).Where(s => subjectIds.Contains(s.Id))
                .ToDictionaryAsync(s => s.Id);
            var classes = await _db.SchoolClasses.AsNoTracking().Include(c => c.LearningLevel).Where(c => classIds.Contains(c.Id))
                .ToDictionaryAsync(c => c.Id);
            var states = await _svc.ApprovalStatesAsync(TeachingDocumentService.EntityScheme, schemeList.Select(s => s.Id));

            // Allocations left pointing at a deleted staff record have no one to show.
            return keys.Where(k => staff.ContainsKey(k.Item1)).Select(k =>
            {
                var scheme = schemeList.FirstOrDefault(s => s.StaffDetailsId == k.Item1 && s.SubjectId == k.Item2 && s.SchoolClassId == k.Item3);
                var subject = subjects.GetValueOrDefault(k.Item2);
                var cls = classes.GetValueOrDefault(k.Item3);
                return new TeachingAssignmentDto
                {
                    StaffDetailsId = k.Item1,
                    StaffName = staff.GetValueOrDefault(k.Item1) ?? "",
                    SubjectId = k.Item2,
                    SubjectName = subject?.Name ?? "",
                    SchoolClassId = k.Item3,
                    ClassName = cls?.Name ?? "",
                    LearningLevelId = cls?.LearningLevelId ?? 0,
                    LearningLevelName = cls?.LearningLevel?.Name ?? "",
                    DepartmentId = subject?.DepartmentId ?? 0,
                    DepartmentName = subject?.Department?.Name,
                    SchemeOfWorkId = scheme?.Id,
                    SchemeStatus = scheme == null ? DocumentStatus.NotStarted : TeachingDocumentService.StatusOf(states.GetValueOrDefault(scheme.Id)),
                    SchemeLessons = scheme?.Lessons ?? 0
                };
            })
            .OrderBy(a => a.StaffName).ThenBy(a => ClassRank(a)).ThenBy(a => a.SubjectName)
            .ToList();

            int ClassRank(TeachingAssignmentDto a) => classes.GetValueOrDefault(a.SchoolClassId)?.Rank ?? 0;
        }

        // GET api/teachingDocuments/curriculum?subjectId=1&learningLevelId=2&sessionId=6
        // Strands and sub-strands with their outcomes, experiences and key
        // questions, for the scheme and lesson plan pickers. With a term, each
        // sub-strand says which terms of that year already have it in a scheme.
        [HttpGet("curriculum")]
        public async Task<IActionResult> Curriculum(int subjectId, int learningLevelId, int? sessionId) =>
            Ok(await _svc.CurriculumAsync(subjectId, learningLevelId, sessionId));

        // GET api/teachingDocuments/preferredApprovers?subjectId=1
        [HttpGet("preferredApprovers")]
        public async Task<IActionResult> PreferredApprovers(int subjectId) =>
            Ok(await _svc.PreferredApproverUserIdsAsync(subjectId));

        // GET api/teachingDocuments/tracker?sessionId=6&departmentId=&learningLevelId=&subjectId=
        // One row per teacher x subject x class: the scheme, each week's lesson
        // plans, and record-of-work progress, coloured by where each stands.
        [HttpGet("tracker")]
        public async Task<IActionResult> Tracker(int sessionId, int? departmentId, int? learningLevelId, int? subjectId, int? staffDetailsId)
        {
            var me = await _svc.CurrentAsync();
            if (me == null) return Unauthorized();
            var session = await _db.Sessions.AsNoTracking().Include(s => s.AcademicYear).FirstOrDefaultAsync(s => s.Id == sessionId);
            if (session == null) return NotFound(new { message = "Term not found." });
            var due = await _svc.DueAsync();
            var termWeeks = TeachingDocumentService.TermWeeks(session);

            var rows = (await AssignmentsAsync(session, me, oversee: true))
                .Where(a => departmentId == null || a.DepartmentId == departmentId)
                .Where(a => learningLevelId == null || a.LearningLevelId == learningLevelId)
                .Where(a => subjectId == null || a.SubjectId == subjectId)
                .Where(a => staffDetailsId == null || a.StaffDetailsId == staffDetailsId)
                .ToList();

            var schemeIds = rows.Where(r => r.SchemeOfWorkId.HasValue).Select(r => r.SchemeOfWorkId!.Value).ToList();
            var schemes = await _db.SchemesOfWork.AsNoTracking().Where(s => schemeIds.Contains(s.Id))
                .Select(s => new { s.Id, s.NumberOfWeeks }).ToDictionaryAsync(s => s.Id, s => s.NumberOfWeeks);
            var weeks = await _db.LessonPlanWeeks.AsNoTracking().Where(w => schemeIds.Contains(w.SchemeOfWorkId))
                .Select(w => new { w.Id, w.SchemeOfWorkId, w.WeekNo }).ToListAsync();
            var lessons = await _db.SchemeOfWorkLessons.AsNoTracking()
                .Where(l => schemeIds.Contains(l.SchemeOfWorkId) && l.SubStrandName != null && l.SubStrandName != "")
                .Select(l => new
                {
                    l.SchemeOfWorkId,
                    Taught = l.RecordOfWorkEntry != null
                        && (l.RecordOfWorkEntry.Status == LessonTaughtStatus.Taught || l.RecordOfWorkEntry.Status == LessonTaughtStatus.PartlyTaught)
                })
                .ToListAsync();
            var checks = await _db.RecordOfWorkChecks.AsNoTracking().Where(c => schemeIds.Contains(c.SchemeOfWorkId))
                .GroupBy(c => c.SchemeOfWorkId).Select(g => new { g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.Key, g => g.Count);

            var schemeStates = await _svc.ApprovalStatesAsync(TeachingDocumentService.EntityScheme, schemeIds);
            var weekStates = await _svc.ApprovalStatesAsync(TeachingDocumentService.EntityLessonPlanWeek, weeks.Select(w => w.Id));
            var awaitingIds = schemeStates.Values.Concat(weekStates.Values)
                .Where(s => s.AwaitingUserId.HasValue).Select(s => s.AwaitingUserId!.Value).Distinct().ToList();
            var userNames = await _db.Users.AsNoTracking().Where(u => awaitingIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => (u.FirstName + " " + u.LastName).Trim());
            string? Awaiting(TeachingDocumentService.ApprovalState? s) =>
                s?.AwaitingUserId is int id ? userNames.GetValueOrDefault(id) : null;

            var schemeDue = TeachingDocumentService.SchemeDueAt(session, due);
            var result = new TrackerDto
            {
                Session = TeachingDocumentService.ToSessionDto(session),
                Weeks = TeachingDocumentService.Weeks(session, termWeeks, due),
                SchemeDueAt = schemeDue,
                Scope = me.WholeSchool ? "school" : me.HodDepartmentIds.Count > 0 ? "department" : "own"
            };

            foreach (var a in rows)
            {
                var schemeId = a.SchemeOfWorkId;
                var row = new TrackerRowDto
                {
                    StaffDetailsId = a.StaffDetailsId,
                    StaffName = a.StaffName,
                    SubjectId = a.SubjectId,
                    SubjectName = a.SubjectName,
                    SchoolClassId = a.SchoolClassId,
                    ClassName = a.ClassName,
                    LearningLevelId = a.LearningLevelId,
                    LearningLevelName = a.LearningLevelName,
                    DepartmentId = a.DepartmentId,
                    DepartmentName = a.DepartmentName
                };
                if (schemeId == null)
                {
                    row.Scheme = new TrackerCellDto { Overdue = DateTime.Now > schemeDue };
                }
                else
                {
                    var st = schemeStates.GetValueOrDefault(schemeId.Value);
                    var status = TeachingDocumentService.StatusOf(st);
                    row.Scheme = new TrackerCellDto
                    {
                        Id = schemeId,
                        Status = status,
                        Overdue = TeachingDocumentService.IsOverdue(status, schemeDue),
                        AwaitingName = Awaiting(st)
                    };
                    var schemeLessons = lessons.Where(l => l.SchemeOfWorkId == schemeId).ToList();
                    row.PlannedLessons = schemeLessons.Count;
                    row.TaughtLessons = schemeLessons.Count(l => l.Taught);
                    row.CheckedWeeks = checks.GetValueOrDefault(schemeId.Value);
                }

                var schemeWeeks = schemeId == null ? termWeeks : schemes.GetValueOrDefault(schemeId.Value, termWeeks);
                foreach (var w in result.Weeks)
                {
                    if (w.WeekNo > schemeWeeks)
                    {
                        row.Weeks.Add(new TrackerCellDto { Status = DocumentStatus.NotApplicable });
                        continue;
                    }
                    var week = schemeId == null ? null : weeks.FirstOrDefault(x => x.SchemeOfWorkId == schemeId && x.WeekNo == w.WeekNo);
                    if (week == null)
                    {
                        row.Weeks.Add(new TrackerCellDto { Overdue = DateTime.Now > w.DueAt });
                        continue;
                    }
                    var st = weekStates.GetValueOrDefault(week.Id);
                    var status = TeachingDocumentService.StatusOf(st);
                    row.Weeks.Add(new TrackerCellDto
                    {
                        Id = week.Id,
                        Status = status,
                        Overdue = TeachingDocumentService.IsOverdue(status, w.DueAt),
                        AwaitingName = Awaiting(st)
                    });
                }
                result.Rows.Add(row);
            }
            return Ok(result);
        }
    }
}
