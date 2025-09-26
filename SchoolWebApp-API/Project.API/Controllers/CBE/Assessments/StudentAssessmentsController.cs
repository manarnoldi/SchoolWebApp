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
                var studentAssessments = await _modelSvc.GetAll();
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
                var paginatedData = await _modelSvc.GetPaginatedData(pageNumber ?? 1, pageSize ?? 10);
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
                var _item = await _modelSvc.GetById(id);

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

        // GET: api/studentAssessments/byStudentId/5
        [HttpGet("byStudentId/{studentId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentAssessmentDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByStudentId(int studentId)
        {
            try
            {
                var _item = await _modelSvc.GetByStudentId(studentId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<StudentAssessmentDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the student assessments by student id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/studentAssessments/bySpecificOutcomeId/5
        [HttpGet("bySpecificOutcomeId/{specificOutcomeId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentAssessmentDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBySpecificOutcomeId(int specificOutcomeId)
        {
            try
            {
                var _item = await _modelSvc.GetBySpecificOutcomeId(specificOutcomeId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<StudentAssessmentDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the student assessments by specific outcome id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/studentAssessments/byGradeId/5
        [HttpGet("byGradeId/{gradeId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentAssessmentDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGradeId(int gradeId)
        {
            try
            {
                var _item = await _modelSvc.GetByGradeId(gradeId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<StudentAssessmentDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the student assessments by grade id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        // GET: api/studentAssessments/bySessionId/5
        [HttpGet("bySessionId/{sessionId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentAssessmentDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBySessionId(int sessionId)
        {
            try
            {
                var _item = await _modelSvc.GetBySessionId(sessionId);
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

        // GET: api/studentAssessments/byAssessmentTypeId/5
        [HttpGet("byAssessmentTypeId/{assessmentTypeId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentAssessmentDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByAssessmentTypeId(int assessmentTypeId)
        {
            try
            {
                var _item = await _modelSvc.GetByAssessmentTypeId(assessmentTypeId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<StudentAssessmentDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the student assessments by assessment type id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/studentAssessments/bySchoolClassId/5
        [HttpGet("bySchoolClassId/{schoolClassId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentAssessmentDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBySchoolClassId(int schoolClassId)
        {
            try
            {
                var _item = await _modelSvc.GetBySchoolClassId(schoolClassId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<StudentAssessmentDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the student assessments by school class id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
