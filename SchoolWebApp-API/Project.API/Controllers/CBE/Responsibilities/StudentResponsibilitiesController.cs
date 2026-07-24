using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.CBE.Responsibilities.StudentResponsibility;
using SchoolWebApp.Core.Entities.CBE.Responsibilities;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Responsibilities;

namespace SchoolWebApp.API.Controllers.CBE.Responsibilities
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StudentResponsibilitiesController : ControllerBase
    {
        private readonly ILogger<StudentResponsibilitiesController> _logger;
        private readonly IStudentResponsibilityService _modelSvc;
        private readonly IMapper _mapper;
        public StudentResponsibilitiesController(ILogger<StudentResponsibilitiesController> logger, IStudentResponsibilityService service, IMapper mapper)
        {
            _logger = logger;
            _modelSvc = service;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentResponsibilityDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var items = await _modelSvc.Find(includeProperties: "Student,AcademicYear");
                var itemDtos = _mapper.Map<List<StudentResponsibilityDto>>(items);
                return Ok(itemDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all student responsibilities list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentResponsibilityDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _modelSvc.GetPaginatedData(pageNumber ?? 1, pageSize ?? 10,
                    includeProperties: "Student,AcademicYear");
                var mappedData = _mapper.Map<List<StudentResponsibilityDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<StudentResponsibilityDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated student responsibilities.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentResponsibilityDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _modelSvc.GetById(id, includeProperties: "Student,AcademicYear");
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<StudentResponsibilityDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the student responsibility by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("byStudentId/{studentId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentResponsibilityDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByStudentId(int studentId)
        {
            try
            {
                var _item = await _modelSvc.GetByStudentId(studentId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<StudentResponsibilityDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the student responsibilities by student id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("byAcademicYearId/{academicYearId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentResponsibilityDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByAcademicYearId(int academicYearId)
        {
            try
            {
                var _item = await _modelSvc.GetByAcademicYearId(academicYearId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<StudentResponsibilityDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the student responsibilities by academic year id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentResponsibilityDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateStudentResponsibilityDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var _item = _mapper.Map<StudentResponsibility>(model);
                    _modelSvc.Create(_item);
                    await _modelSvc.SaveChangesAsync();
                    var returnItem = _mapper.Map<StudentResponsibilityDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the student responsibility");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the student responsibility - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // POST api/studentResponsibilities/batch
        /// <summary>
        /// Upserts a batch of student responsibilities in a single request.
        /// Rows with Id &gt; 0 are updated; rows with Id &lt;= 0 are created.
        /// Rows omitted from the payload are left untouched (NOT a full-set
        /// replace), so the client can send only the records that were added or
        /// changed.
        /// </summary>
        [HttpPost("batch")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateMany(List<StudentResponsibilityDto> model)
        {
            if (model == null || !model.Any())
                return BadRequest("No student responsibilities provided.");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                foreach (var item in model)
                {
                    var _item = _mapper.Map<StudentResponsibility>(item);
                    if (item.Id > 0)
                        _modelSvc.Update(_item);
                    else
                        _modelSvc.Create(_item);
                }
                await _modelSvc.SaveChangesAsync();
                return Ok("Student responsibilities saved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the batch of student responsibilities.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(StudentResponsibilityDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _modelSvc.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The student responsibility of Id- '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var _item = _mapper.Map<StudentResponsibility>(model);
                    _modelSvc.Update(_item);
                    await _modelSvc.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the student responsibility - {ex.Message}");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the student responsibility");
                }
            }
            return BadRequest(ModelState);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _modelSvc.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The student responsibility of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _modelSvc.GetById(id);
                _modelSvc.Delete(entity);
                await _modelSvc.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the student responsibility.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the student responsibility - " + ex.Message);
            }
        }
    }
}
