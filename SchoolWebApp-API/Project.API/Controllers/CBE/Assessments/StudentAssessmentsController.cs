using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.CBE.Assessments.Assessment;
using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.Core.Entities.Identity;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Assessments;

namespace SchoolWebApp.API.Controllers.CBE.Assessments
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StudentAssessmentsController : ControllerBase
    {
        private readonly ILogger<StudentAssessmentsController> _logger;
        private readonly IStudentAssessmentService _modelSvc;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;

        // Sentinel path that, when assigned to a role via MenuPermissions, grants
        // admin rights on student assessments (full teacher list, no per-(class,
        // subject) restriction). Matches the menu entry under CBE Curriculum.
        private const string ADMIN_PERMISSION_PATH = "/cbe/assessments/admin";

        public StudentAssessmentsController(
            ILogger<StudentAssessmentsController> logger,
            IStudentAssessmentService service,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            UserManager<AppUser> userManager)
        {
            _logger = logger;
            _modelSvc = service;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        /// <summary>
        /// True if the caller bypasses the per-allocation restriction: either
        /// the Administrator/SuperAdministrator role, OR a role that has the
        /// <c>/cbe/assessments/admin</c> path assigned via MenuPermissions.
        /// </summary>
        private async Task<bool> HasAdminAccessAsync()
        {
            var roles = User.Claims.Where(c => c.Type == "roles").Select(c => c.Value).ToList();
            if (roles.Contains("Administrator") || roles.Contains("SuperAdministrator"))
                return true;
            if (roles.Count == 0) return false;

            var perms = await _unitOfWork.Repository<MenuPermission>()
                .Find(p => p.MenuPath == ADMIN_PERMISSION_PATH && roles.Contains(p.RoleId));
            return perms.Any();
        }

        /// <summary>
        /// Gates a write to student assessments. Admins / permission-holders
        /// pass through. Other authenticated users (typically Teachers) must
        /// have a StaffSubject row linking their staff record to the
        /// (SchoolClassId, derived SubjectId) of the assessment being saved.
        /// Returns null when allowed, or an IActionResult the caller should
        /// return immediately when blocked.
        /// </summary>
        private async Task<IActionResult> CheckCanWriteAsync(int schoolClassId, int? strandId, int? subStrandId)
        {
            if (await HasAdminAccessAsync()) return null;

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userid")?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return StatusCode(StatusCodes.Status403Forbidden,
                    new { message = "Unable to identify the current user." });

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user?.PersonId == null)
                return StatusCode(StatusCodes.Status403Forbidden,
                    new { message = "Your account is not linked to a staff record." });

            // Resolve the assessment's SubjectId from its Strand (or its
            // SubStrand's parent Strand). Uses the generic Repository<T>
            // accessor since Strands/SubStrands aren't in the unit of work's
            // named property list.
            int? subjectId = null;
            if (strandId.HasValue && strandId.Value > 0)
            {
                var strand = await _unitOfWork.Repository<Strand>().GetById(strandId.Value);
                if (strand != null) subjectId = strand.SubjectId;
            }
            if (subjectId == null && subStrandId.HasValue && subStrandId.Value > 0)
            {
                var subStrand = await _unitOfWork.Repository<SubStrand>().GetById(subStrandId.Value, includeProperties: "Strand");
                if (subStrand?.Strand != null) subjectId = subStrand.Strand.SubjectId;
            }
            if (subjectId == null)
            {
                // No strand/subject context — can't enforce; fall back to deny
                // for non-admins (safer than allowing an un-checkable write).
                return StatusCode(StatusCodes.Status403Forbidden,
                    new { message = "Cannot verify subject allocation for this assessment." });
            }

            var allocations = await _unitOfWork.StaffSubjects.GetByStaffDetailsId(user.PersonId.Value);
            var ok = allocations.Any(a => a.SchoolClassId == schoolClassId && a.SubjectId == subjectId.Value);
            if (!ok)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    message = "You are not allocated to this subject for the selected class."
                });
            }
            return null;
        }

        // GET: api/studentAssessments
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentAssessmentDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var studentAssessments = await _modelSvc.Find(includeProperties: "AssessmentType,Grade,Session,SpecificOutcome,SubStrand,Strand,SchoolClass");
                var studentAssessmentsDtos = _mapper.Map<List<StudentAssessmentDto>>(studentAssessments);
                return Ok(studentAssessmentsDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all student assessments list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/studentAssessments/paginated
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentAssessmentDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _modelSvc.GetPaginatedData(pageNumber ?? 1, pageSize ?? 10,
                    includeProperties: "AssessmentType,Grade,Session,SpecificOutcome,SubStrand,Strand,SchoolClass");
                var mappedData = _mapper.Map<List<StudentAssessmentDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<StudentAssessmentDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated student assessments.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //GET: api/studentAssessments/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentAssessmentDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _modelSvc.GetById(id, includeProperties: "AssessmentType,Grade,Session,SpecificOutcome,SubStrand,Strand,SchoolClass");

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<StudentAssessmentDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the student assessment by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST: api/studentAssessments
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentAssessmentDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateStudentAssessmentDto model)
        {
            if (ModelState.IsValid)
            {
                if (await _modelSvc.ItemExistsAsync(st => st.StudentId == model.StudentId &&
                st.SpecificOutcomeId == model.SpecificOutcomeId && st.SubStrandId == model.SubStrandId &&
                st.StrandId == model.StrandId && st.SessionId == model.SessionId &&
                st.AssessmentTypeId == model.AssessmentTypeId && st.SchoolClassId == model.SchoolClassId))
                    return Conflict(new { message = $"The student assessment specified already exists" });

                var authBlock = await CheckCanWriteAsync(model.SchoolClassId, model.StrandId, model.SubStrandId);
                if (authBlock != null) return authBlock;

                try
                {
                    var _item = _mapper.Map<StudentAssessment>(model);
                    _modelSvc.Create(_item);
                    await _modelSvc.SaveChangesAsync();

                    var returnItem = _mapper.Map<StudentAssessmentDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the student assessment");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the student assessment - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT: api/studentAssessments
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(StudentAssessmentDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _modelSvc.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The student assessment of Id- '{model.Id}' does not exist hence cannot be updated.");

                var authBlock = await CheckCanWriteAsync(model.SchoolClassId, model.StrandId, model.SubStrandId);
                if (authBlock != null) return authBlock;

                try
                {
                    var _item = _mapper.Map<StudentAssessment>(model);
                    _modelSvc.Update(_item);
                    await _modelSvc.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the student assessment - {ex.Message}");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the student assessment");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE: api/studentAssessments/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _modelSvc.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The student assessment of Id- '{id}' does not exist hence cannot be deleted.");

                var authBlock = await CheckCanWriteAsync(itemExists.SchoolClassId, itemExists.StrandId, itemExists.SubStrandId);
                if (authBlock != null) return authBlock;

                var entity = await _modelSvc.GetById(id);
                _modelSvc.Delete(entity);
                await _modelSvc.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the student assessment.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the student assessment - " + ex.Message);
            }
        }

        //GET api/studentAssessments/bySessionIdAndParams/5?studentId=1&schoolClassId=1&assessmentTypeid=1&specificOutcomeId=1
        [HttpGet("bySessionIdAndParams/{sessionId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentAssessmentDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBySessionIdAndParams(int sessionId, int? studentId, int? schoolClassId, int? assessmentTypeid,
            int? specificOutcomeId, int? subStrandId, int? strandId)
        {
            try
            {
                var _item = await _modelSvc.GetBySessionIdAndParams(sessionId, studentId, schoolClassId, assessmentTypeid, specificOutcomeId, subStrandId, strandId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<StudentAssessmentDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the student assessments by session id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
