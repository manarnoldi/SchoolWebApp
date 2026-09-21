using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.Academics.SchoolExam;
using SchoolWebApp.Core.Entities.CBE.Exams;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Services;

namespace SchoolWebApp.API.Controllers.CBE.Exams
{
    /// <summary>
    /// Manages the exam "event" header (type, term, schedule) and the
    /// release workflow. Reading is open to any authenticated user (the
    /// register / results screens need it); creating, editing, deleting
    /// and releasing are restricted to administrators - releasing
    /// publishes results to the dashboard and (later) parents.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolExamsController : ControllerBase
    {
        private readonly ILogger<SchoolExamsController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditService _audit;

        public SchoolExamsController(ILogger<SchoolExamsController> logger, IUnitOfWork unitOfWork,
            IMapper mapper, IAuditService audit)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _audit = audit;
        }

        // GET: api/schoolExams
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SchoolExamDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(_mapper.Map<List<SchoolExamDto>>(await _unitOfWork.SchoolExams
                    .Find(includeProperties: "ExamType,Session")));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all school exams.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/schoolExams/examSearch?academicYearId=5&curriculumId=5&sessionId=5&examTypeId=5
        [HttpGet("examSearch")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SchoolExamDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Search(int academicYearId, int curriculumId,
            int? sessionId = null, int? examTypeId = null)
        {
            try
            {
                if (academicYearId <= 0) return BadRequest(academicYearId);
                if (curriculumId <= 0) return BadRequest(curriculumId);
                var _items = await _unitOfWork.SchoolExams.SearchForSchoolExams(academicYearId, curriculumId, sessionId, examTypeId);
                return Ok(_mapper.Map<List<SchoolExamDto>>(_items));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while searching school exams.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/schoolExams/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SchoolExamDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _unitOfWork.SchoolExams.GetById(id, includeProperties: "ExamType,Session");
                if (_item == null) return NotFound();
                return Ok(_mapper.Map<SchoolExamDto>(_item));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the school exam by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST: api/schoolExams
        [HttpPost]
        [Authorize(Roles = "Administrator,SuperAdministrator")]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SchoolExamDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateSchoolExamDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // One exam per type per term, unless the type allows several - a weekly
            // marathon, say. The single-exam rule keeps one report-form column per
            // exam type unambiguous; types that allow several are the ones kept off
            // the report form.
            var examType = await _unitOfWork.ExamTypes.GetById(model.ExamTypeId);
            if (examType == null)
                return BadRequest(new { message = "The selected exam type no longer exists." });

            if (!examType.AllowMultiplePerTerm
                && await _unitOfWork.SchoolExams.ItemExistsAsync(s => s.SessionId == model.SessionId
                    && s.ExamTypeId == model.ExamTypeId))
                return Conflict(new { message = $"A school exam for this term and '{examType.Name}' already exists. To register several {examType.Name} exams in a term, tick 'Allow several exams per term' on the exam type." });
            try
            {
                var _item = _mapper.Map<SchoolExam>(model);
                _unitOfWork.SchoolExams.Create(_item);
                await _unitOfWork.SaveChangesAsync();
                return Ok(_mapper.Map<SchoolExamDto>(_item));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the school exam.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the school exam - {ex.Message}");
            }
        }

        // PUT: api/schoolExams
        [HttpPut]
        [Authorize(Roles = "Administrator,SuperAdministrator")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(SchoolExamDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _unitOfWork.SchoolExams.GetById(model.Id);
            if (existing == null)
                return BadRequest($"The school exam of Id - '{model.Id}' does not exist hence cannot be updated.");

            // Same rule as Create: moving an exam to another term or type must not
            // leave two exams of a type that allows only one a term.
            var examType = await _unitOfWork.ExamTypes.GetById(model.ExamTypeId);
            if (examType == null)
                return BadRequest(new { message = "The selected exam type no longer exists." });

            if (!examType.AllowMultiplePerTerm
                && await _unitOfWork.SchoolExams.ItemExistsAsync(s => s.SessionId == model.SessionId
                    && s.ExamTypeId == model.ExamTypeId && s.Id != model.Id))
                return Conflict(new { message = $"Another school exam for this term and '{examType.Name}' already exists." });

            try
            {
                // Editing the schedule must not silently change release state;
                // preserve the release/notification fields from the stored row.
                existing.ExamStartDate = model.ExamStartDate;
                existing.ExamEndDate = model.ExamEndDate;
                existing.ExamMarkEntryEndDate = model.ExamMarkEntryEndDate;
                existing.Description = model.Description;
                existing.ExamTypeId = model.ExamTypeId;
                existing.SessionId = model.SessionId;
                _unitOfWork.SchoolExams.Update(existing);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the school exam.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the school exam.");
            }
        }

        // POST: api/schoolExams/5/release
        // Releasing publishes the exam's results (e.g. to the dashboard
        // summary) and is the hook for future parent notifications.
        [HttpPost("{id}/release")]
        [Authorize(Roles = "Administrator,SuperAdministrator")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SchoolExamDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Release(int id, [FromQuery] bool release = true)
        {
            try
            {
                var _item = await _unitOfWork.SchoolExams.GetById(id, includeProperties: "ExamType,Session");
                if (_item == null) return NotFound();

                var actor = User?.FindFirstValue("username") ?? User?.Identity?.Name;
                _item.IsReleased = release;
                _item.ReleasedBy = release ? actor : null;
                _item.ReleasedDate = release ? DateTime.UtcNow : null;
                _unitOfWork.SchoolExams.Update(_item);
                await _unitOfWork.SaveChangesAsync();

                await _audit.LogAsync(
                    action: release ? "Release" : "Unrelease",
                    entityType: nameof(SchoolExam),
                    entityId: id.ToString(),
                    notes: release ? "Exam results released." : "Exam release reverted.");

                return Ok(_mapper.Map<SchoolExamDto>(_item));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while releasing the school exam.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // DELETE: api/schoolExams/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator,SuperAdministrator")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var entity = await _unitOfWork.SchoolExams.GetById(id);
                if (entity == null)
                    return BadRequest($"The school exam of Id - '{id}' does not exist hence cannot be deleted.");

                // Don't orphan detail rows / results - block deletion while
                // any Exam still hangs off this header.
                if (await _unitOfWork.Exams.ItemExistsAsync(e => e.SchoolExamId == id))
                    return Conflict(new { message = "Remove the registered class/subject exams under this school exam before deleting it." });

                _unitOfWork.SchoolExams.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the school exam.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the school exam - " + ex.Message);
            }
        }
    }
}
