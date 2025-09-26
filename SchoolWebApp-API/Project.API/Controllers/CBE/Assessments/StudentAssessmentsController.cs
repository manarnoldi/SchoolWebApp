using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.CBE.Assessments.Assessment;
using SchoolWebApp.Core.Entities.CBE.Assessments;
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
        public StudentAssessmentsController(ILogger<StudentAssessmentsController> logger, IStudentAssessmentService service, IMapper mapper)
        {
            _logger = logger;
            _modelSvc = service;
            _mapper = mapper;
        }

        // GET: api/studentAssessments
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentAssessmentDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var studentAssessments = await _modelSvc.Find(includeProperties: "AssessmentType,Grade,Session,SpecificOutcome,SchoolClass");
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
                    includeProperties: "AssessmentType,Grade,Session,SpecificOutcome,SchoolClass");
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
                var _item = await _modelSvc.GetById(id, includeProperties: "AssessmentType,Grade,Session,SpecificOutcome,SchoolClass");

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
                st.SpecificOutcomeId == model.SpecificOutcomeId && st.GradeId == model.GradeId &&
                st.SessionId == model.SessionId && st.AssessmentTypeId == model.AssessmentTypeId &&
                st.SchoolClassId == model.SchoolClassId))
                    return Conflict(new { message = $"The student assessment specified already exists" });
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
            int? specificOutcomeId)
        {
            try
            {
                var _item = await _modelSvc.GetBySessionIdAndParams(sessionId, studentId, schoolClassId, assessmentTypeid, specificOutcomeId);
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
