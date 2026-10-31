using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.API.Services.TeachingDocuments;
using SchoolWebApp.Core.DTOs.TeachingDocuments;
using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.Core.Entities.TeachingDocuments;
using SchoolWebApp.API.Licensing;

namespace SchoolWebApp.API.Controllers.TeachingDocuments
{
    /// <summary>
    /// Schemes of work: one per teacher, subject, class and term. Edits stop
    /// while the scheme is with its approvers (per the global edit-lock policy)
    /// and once it is approved.
    /// </summary>
    [Route("api/schemesOfWork")]
    [ApiController]
    [Authorize]
    [RequiresModule(AppModules.Academics)]
    public class SchemesOfWorkController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly TeachingDocumentService _svc;

        public SchemesOfWorkController(ApplicationDbContext db, TeachingDocumentService svc)
        {
            _db = db;
            _svc = svc;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var me = await _svc.CurrentAsync();
            if (me == null) return Unauthorized();
            var s = await _svc.LoadSchemeAsync(id, withLessons: true);
            if (s == null) return NotFound();
            if (!await _svc.CanViewAsync(me, s)) return Forbid();
            return Ok(await _svc.ToDtoAsync(s, me, withLessons: true));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSchemeOfWorkDto model)
        {
            var me = await _svc.CurrentAsync();
            if (me == null) return Unauthorized();
            var staffId = me.IsAdmin && model.StaffDetailsId is > 0 ? model.StaffDetailsId.Value : me.StaffDetailsId;
            if (staffId == null) return BadRequest(new { message = "Your account is not linked to a staff record." });

            var session = await _db.Sessions.FirstOrDefaultAsync(s => s.Id == model.SessionId);
            var cls = await _db.SchoolClasses.FirstOrDefaultAsync(c => c.Id == model.SchoolClassId);
            var subject = await _db.Subjects.FirstOrDefaultAsync(s => s.Id == model.SubjectId);
            if (session == null || cls == null || subject == null) return BadRequest(new { message = "Choose the term, class and subject." });
            if (cls.AcademicYearId != session.AcademicYearId)
                return BadRequest(new { message = "The class belongs to a different academic year from the term." });
            var level = await _db.LearningLevels.AsNoTracking().FirstOrDefaultAsync(l => l.Id == cls.LearningLevelId);
            if (!subject.RequiresTeachingDocuments || level?.RequiresTeachingDocuments == false)
                return BadRequest(new { message = $"{subject.Name} for {cls.Name} does not need teaching documents (see Subjects and Learning Levels)." });
            if (!me.IsAdmin && !await _db.StaffSubjects.AnyAsync(a =>
                    a.StaffDetailsId == staffId && a.SubjectId == model.SubjectId && a.SchoolClassId == model.SchoolClassId))
                return BadRequest(new { message = "You are not allocated to this subject for the selected class." });
            var existing = await _db.SchemesOfWork.FirstOrDefaultAsync(s => s.StaffDetailsId == staffId
                && s.SubjectId == model.SubjectId && s.SchoolClassId == model.SchoolClassId && s.SessionId == model.SessionId);
            if (existing != null) return Ok(new { id = existing.Id });

            var scheme = new SchemeOfWork
            {
                StaffDetailsId = staffId.Value,
                SubjectId = subject.Id,
                SchoolClassId = cls.Id,
                SessionId = session.Id,
                NumberOfWeeks = TeachingDocumentService.TermWeeks(session),
                LessonsPerWeek = await _svc.DefaultLessonsPerWeekAsync(subject.Id, cls.LearningLevelId)
            };
            scheme.Lessons = BlankGrid(scheme.NumberOfWeeks, scheme.LessonsPerWeek);
            _db.SchemesOfWork.Add(scheme);
            await _db.SaveChangesAsync();
            return Ok(new { id = scheme.Id });
        }

        private static List<SchemeOfWorkLesson> BlankGrid(int weeks, int perWeek) => TeachingDocumentService.BlankGrid(weeks, perWeek);

        private async Task<(SchemeOfWork? Scheme, IActionResult? Error)> LoadForEditAsync(int id)
        {
            var me = await _svc.CurrentAsync();
            if (me == null) return (null, Unauthorized());
            var s = await _svc.LoadSchemeAsync(id, withLessons: true);
            if (s == null) return (null, NotFound());
            if (!me.CanEdit(s)) return (null, StatusCode(403, new { message = "Only the teacher who owns this scheme can change it." }));
            if (await _svc.IsLockedAsync(TeachingDocumentService.EntityScheme, s.Id))
                return (null, BadRequest(new { message = "This scheme has been submitted or approved and can no longer be edited." }));
            return (s, null);
        }

        [HttpPut]
        public async Task<IActionResult> Save(SaveSchemeOfWorkDto model)
        {
            var (s, error) = await LoadForEditAsync(model.Id);
            if (error != null) return error;
            if (model.NumberOfWeeks < 1 || model.NumberOfWeeks > 20) return BadRequest(new { message = "Number of weeks must be between 1 and 20." });
            if (model.LessonsPerWeek < 1 || model.LessonsPerWeek > 20) return BadRequest(new { message = "Lessons per week must be between 1 and 20." });
            var bad = model.Lessons.FirstOrDefault(l => l.WeekNo < 1 || l.WeekNo > model.NumberOfWeeks || l.LessonNo < 1);
            if (bad != null) return BadRequest(new { message = $"Week {bad.WeekNo} lesson {bad.LessonNo} is outside the scheme's {model.NumberOfWeeks} weeks." });
            var dup = model.Lessons.GroupBy(l => (l.WeekNo, l.LessonNo)).FirstOrDefault(g => g.Count() > 1);
            if (dup != null) return BadRequest(new { message = $"Week {dup.Key.WeekNo} has lesson {dup.Key.LessonNo} more than once." });

            s!.References = model.References?.Trim();
            s.NumberOfWeeks = model.NumberOfWeeks;
            s.LessonsPerWeek = model.LessonsPerWeek;

            var incomingIds = model.Lessons.Where(l => l.Id > 0).Select(l => l.Id).ToHashSet();
            _db.SchemeOfWorkLessons.RemoveRange(s.Lessons.Where(l => !incomingIds.Contains(l.Id)));
            foreach (var dto in model.Lessons)
            {
                var row = dto.Id > 0 ? s.Lessons.FirstOrDefault(l => l.Id == dto.Id) : null;
                if (row == null)
                {
                    row = new SchemeOfWorkLesson();
                    s.Lessons.Add(row);
                }
                Apply(row, dto);
            }
            await _db.SaveChangesAsync();
            var me = await _svc.CurrentAsync();
            return Ok(await _svc.ToDtoAsync((await _svc.LoadSchemeAsync(s.Id, true))!, me!, withLessons: true));
        }

        private static void Apply(SchemeOfWorkLesson row, SchemeOfWorkLessonDto dto)
        {
            row.WeekNo = dto.WeekNo;
            row.LessonNo = dto.LessonNo;
            row.StrandId = dto.StrandId > 0 ? dto.StrandId : null;
            row.SubStrandId = dto.SubStrandId > 0 ? dto.SubStrandId : null;
            row.StrandName = Trim(dto.StrandName, 255);
            row.SubStrandName = Trim(dto.SubStrandName, 255);
            row.LearningOutcomes = dto.LearningOutcomes;
            row.LearningExperiences = dto.LearningExperiences;
            row.KeyInquiryQuestions = dto.KeyInquiryQuestions;
            row.LearningResources = dto.LearningResources;
            row.AssessmentMethods = dto.AssessmentMethods;
        }

        private static string? Trim(string? s, int max)
        {
            s = s?.Trim();
            if (string.IsNullOrEmpty(s)) return null;
            return s.Length > max ? s[..max] : s;
        }

        private static bool HasContent(SchemeOfWorkLesson l) =>
            !string.IsNullOrWhiteSpace(l.SubStrandName) || !string.IsNullOrWhiteSpace(l.StrandName)
            || !string.IsNullOrWhiteSpace(l.LearningOutcomes) || !string.IsNullOrWhiteSpace(l.LearningExperiences);

        /// <summary>
        /// Lays the curriculum out across the scheme's weeks: sub-strands in
        /// curriculum order, each taking as many lessons as the curriculum gives
        /// it, skipping sub-strands an earlier term's scheme (any teacher, same
        /// subject and grade) already covers. The teacher then edits the result.
        /// </summary>
        [HttpPost("{id}/generate")]
        public async Task<IActionResult> Generate(int id, GenerateSchemeDto model)
        {
            var (s, error) = await LoadForEditAsync(id);
            if (error != null) return error;
            if (!model.Replace && s!.Lessons.Any(HasContent))
                return Conflict(new { message = "The scheme already has lessons filled in. Replace them?" });

            var levelId = s!.SchoolClass!.LearningLevelId;
            var strands = await _db.Strands.AsNoTracking()
                .Where(x => x.SubjectId == s.SubjectId && x.LearningLevelId == levelId)
                .OrderBy(x => x.Rank).ThenBy(x => x.Code).ThenBy(x => x.Id)
                .Select(x => new { x.Id, x.Name }).ToListAsync();
            var strandIds = strands.Select(x => x.Id).ToList();
            var subStrands = (await _db.SubStrands.AsNoTracking()
                    .Where(x => strandIds.Contains(x.StrandId))
                    .Select(x => new { x.Id, x.StrandId, x.Name, x.LessonNo, x.Rank, x.Code })
                    .ToListAsync())
                .OrderBy(x => strandIds.IndexOf(x.StrandId)).ThenBy(x => x.Rank).ThenBy(x => x.Code).ThenBy(x => x.Id)
                .ToList();

            var covered = await _db.SchemeOfWorkLessons.AsNoTracking()
                .Where(l => l.SubStrandId != null && l.SchemeOfWorkId != s.Id
                    && l.SchemeOfWork!.SubjectId == s.SubjectId
                    && l.SchemeOfWork.SchoolClass!.LearningLevelId == levelId
                    && l.SchemeOfWork.Session!.AcademicYearId == s.Session!.AcademicYearId
                    && l.SchemeOfWork.Session.StartDate < s.Session.StartDate)
                .Select(l => l.SubStrandId!.Value).Distinct().ToListAsync();
            subStrands = subStrands.Where(x => !covered.Contains(x.Id)).ToList();
            if (subStrands.Count == 0)
                return BadRequest(new { message = "There are no curriculum sub-strands left for this subject and grade - add them under CBE Curriculum, or fill the scheme in by hand." });

            var subIds = subStrands.Select(x => x.Id).ToList();
            var outcomes = await _db.SpecificOutcomes.AsNoTracking().Where(o => subIds.Contains(o.SubStrandId))
                .OrderBy(o => o.Rank).ThenBy(o => o.Id).Select(o => new { o.SubStrandId, o.Name }).ToListAsync();
            var experiences = await _db.Set<LearningExperience>().AsNoTracking().Where(o => subIds.Contains(o.SubStrandId))
                .OrderBy(o => o.Rank).ThenBy(o => o.Id).Select(o => new { o.SubStrandId, o.Name }).ToListAsync();
            var questions = await _db.Set<KeyQuestion>().AsNoTracking().Where(o => subIds.Contains(o.SubStrandId))
                .OrderBy(o => o.Rank).ThenBy(o => o.Id).Select(o => new { o.SubStrandId, o.Name }).ToListAsync();

            var slots = BlankGrid(s.NumberOfWeeks, s.LessonsPerWeek);
            var slot = 0;
            foreach (var sub in subStrands)
            {
                var strandName = strands.First(x => x.Id == sub.StrandId).Name;
                for (var i = 0; i < Math.Max(sub.LessonNo, 1) && slot < slots.Count; i++, slot++)
                {
                    var row = slots[slot];
                    row.StrandId = sub.StrandId;
                    row.SubStrandId = sub.Id;
                    row.StrandName = strandName;
                    row.SubStrandName = sub.Name;
                    row.LearningOutcomes = TeachingDocumentService.Lines(
                        outcomes.Where(o => o.SubStrandId == sub.Id).Select(o => o.Name),
                        "By the end of the lesson, the learner should be able to:");
                    row.LearningExperiences = TeachingDocumentService.Lines(
                        experiences.Where(o => o.SubStrandId == sub.Id).Select(o => o.Name),
                        "The learner is guided to:");
                    row.KeyInquiryQuestions = string.Join("\n", questions.Where(o => o.SubStrandId == sub.Id).Select(o => o.Name));
                    row.LearningResources = s.References;
                }
                if (slot >= slots.Count) break;
            }

            _db.SchemeOfWorkLessons.RemoveRange(s.Lessons);
            foreach (var row in slots) s.Lessons.Add(row);
            await _db.SaveChangesAsync();
            var me = await _svc.CurrentAsync();
            return Ok(await _svc.ToDtoAsync((await _svc.LoadSchemeAsync(s.Id, true))!, me!, withLessons: true));
        }

        // GET api/schemesOfWork/{id}/copySources - other schemes for the same
        // subject and grade (another stream, or an earlier year) to start from.
        [HttpGet("{id}/copySources")]
        public async Task<IActionResult> CopySources(int id)
        {
            var me = await _svc.CurrentAsync();
            if (me == null) return Unauthorized();
            var s = await _svc.LoadSchemeAsync(id);
            if (s == null) return NotFound();
            var sources = await _db.SchemesOfWork.AsNoTracking()
                .Where(x => x.Id != id && x.SubjectId == s.SubjectId && x.SchoolClass!.LearningLevelId == s.SchoolClass!.LearningLevelId)
                .OrderByDescending(x => x.Session!.StartDate).ThenBy(x => x.SchoolClass!.Name)
                .Select(x => new
                {
                    x.Id,
                    StaffName = x.StaffDetails!.FullName,
                    ClassName = x.SchoolClass!.Name,
                    SessionName = x.Session!.SessionName,
                    AcademicYearName = x.Session.AcademicYear!.Name,
                    Lessons = x.Lessons.Count(l => l.SubStrandName != null && l.SubStrandName != "")
                })
                .Take(50).ToListAsync();
            var states = await _svc.ApprovalStatesAsync(TeachingDocumentService.EntityScheme, sources.Select(x => x.Id));
            return Ok(sources.Where(x => x.Lessons > 0).Select(x => new SchemeCopySourceDto
            {
                Id = x.Id,
                StaffName = x.StaffName,
                ClassName = x.ClassName,
                SessionName = x.SessionName,
                AcademicYearName = x.AcademicYearName,
                Lessons = x.Lessons,
                Status = TeachingDocumentService.StatusOf(states.GetValueOrDefault(x.Id))
            }));
        }

        // POST api/schemesOfWork/{id}/copyFrom/{sourceId} - replaces this scheme's lessons.
        [HttpPost("{id}/copyFrom/{sourceId}")]
        public async Task<IActionResult> CopyFrom(int id, int sourceId)
        {
            var (s, error) = await LoadForEditAsync(id);
            if (error != null) return error;
            var source = await _db.SchemesOfWork.AsNoTracking().Include(x => x.Lessons).Include(x => x.SchoolClass)
                .FirstOrDefaultAsync(x => x.Id == sourceId);
            if (source == null) return NotFound(new { message = "Scheme to copy from not found." });
            if (source.SubjectId != s!.SubjectId || source.SchoolClass!.LearningLevelId != s.SchoolClass!.LearningLevelId)
                return BadRequest(new { message = "Only a scheme for the same subject and grade can be copied." });

            _db.SchemeOfWorkLessons.RemoveRange(s.Lessons);
            if (string.IsNullOrWhiteSpace(s.References)) s.References = source.References;
            s.LessonsPerWeek = source.LessonsPerWeek;
            s.NumberOfWeeks = Math.Clamp(Math.Max(s.NumberOfWeeks, source.Lessons.Select(l => l.WeekNo).DefaultIfEmpty(1).Max()), 1, 20);
            foreach (var l in source.Lessons.Where(l => l.WeekNo <= s.NumberOfWeeks).OrderBy(l => l.WeekNo).ThenBy(l => l.LessonNo))
            {
                var row = new SchemeOfWorkLesson();
                Apply(row, TeachingDocumentService.ToDto(l));
                s.Lessons.Add(row);
            }
            await _db.SaveChangesAsync();
            var me = await _svc.CurrentAsync();
            return Ok(await _svc.ToDtoAsync((await _svc.LoadSchemeAsync(s.Id, true))!, me!, withLessons: true));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var (s, error) = await LoadForEditAsync(id);
            if (error != null) return error;
            var weekIds = await _db.LessonPlanWeeks.Where(w => w.SchemeOfWorkId == id).Select(w => w.Id).ToListAsync();
            if (await _db.ApprovalRequests.AnyAsync(r => r.EntityType == TeachingDocumentService.EntityLessonPlanWeek
                    && weekIds.Contains(r.EntityId)
                    && (r.Status == Core.Entities.Approvals.ApprovalRequestStatus.Submitted || r.Status == Core.Entities.Approvals.ApprovalRequestStatus.Approved)))
                return BadRequest(new { message = "Lesson plans from this scheme have been submitted or approved; the scheme cannot be deleted." });

            // Approval history of the scheme and its weeks goes with it.
            var requests = await _db.ApprovalRequests.Include(r => r.Actions)
                .Where(r => (r.EntityType == TeachingDocumentService.EntityScheme && r.EntityId == id)
                    || (r.EntityType == TeachingDocumentService.EntityLessonPlanWeek && weekIds.Contains(r.EntityId)))
                .ToListAsync();
            _db.ApprovalStepActions.RemoveRange(requests.SelectMany(r => r.Actions));
            _db.ApprovalRequests.RemoveRange(requests);
            _db.SchemesOfWork.Remove(s!);
            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}
