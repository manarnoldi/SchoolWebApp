using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.CBE.CommunityService.StudentCommunityServiceActivity;
using SchoolWebApp.Core.Entities.CBE.CommunityService;
using SchoolWebApp.Core.Interfaces.IServices.CBE.CommunityService;

namespace SchoolWebApp.API.Controllers.CBE.CommunityService
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StudentCommunityServiceActivitiesController : ControllerBase
    {
        private readonly ILogger<StudentCommunityServiceActivitiesController> _logger;
        private readonly IStudentCommunityServiceActivityService _modelSvc;
        private readonly IMapper _mapper;
        public StudentCommunityServiceActivitiesController(ILogger<StudentCommunityServiceActivitiesController> logger, IStudentCommunityServiceActivityService service, IMapper mapper)
        {
            _logger = logger;
            _modelSvc = service;
            _mapper = mapper;
        }

        // GET: api/studentCommunityServiceActivities
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentCommunityServiceActivityDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var items = await _modelSvc.Find(includeProperties: "Student,CommunityServiceActivity,Session,AcademicYear");
                var itemsDtos = _mapper.Map<List<StudentCommunityServiceActivityDto>>(items);
                return Ok(itemsDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all student community service activities list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/studentCommunityServiceActivities/paginated
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentCommunityServiceActivityDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _modelSvc.GetPaginatedData(pageNumber ?? 1, pageSize ?? 10,
                    includeProperties: "Student,CommunityServiceActivity,Session,AcademicYear");
                var mappedData = _mapper.Map<List<StudentCommunityServiceActivityDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<StudentCommunityServiceActivityDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated student community service activities.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //GET: api/studentCommunityServiceActivities/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentCommunityServiceActivityDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _modelSvc.GetById(id, includeProperties: "Student,CommunityServiceActivity,Session,AcademicYear");

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<StudentCommunityServiceActivityDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the student community service activity by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST: api/studentCommunityServiceActivities
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentCommunityServiceActivityDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateStudentCommunityServiceActivityDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var _item = _mapper.Map<SchoolWebApp.Core.Entities.CBE.CommunityService.StudentCommunityServiceActivity>(model);
                    _modelSvc.Create(_item);
                    await _modelSvc.SaveChangesAsync();

                    var returnItem = _mapper.Map<StudentCommunityServiceActivityDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the student community service activity");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the student community service activity - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT: api/studentCommunityServiceActivities
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(StudentCommunityServiceActivityDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _modelSvc.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The student community service activity of Id- '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var _item = _mapper.Map<SchoolWebApp.Core.Entities.CBE.CommunityService.StudentCommunityServiceActivity>(model);
                    _modelSvc.Update(_item);
                    await _modelSvc.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the student community service activity - {ex.Message}");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the student community service activity");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE: api/studentCommunityServiceActivities/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _modelSvc.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The student community service activity of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _modelSvc.GetById(id);
                _modelSvc.Delete(entity);
                await _modelSvc.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the student community service activity.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the student community service activity - " + ex.Message);
            }
        }

        // GET: api/studentCommunityServiceActivities/byStudentId/5
        [HttpGet("byStudentId/{studentId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentCommunityServiceActivityDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByStudentId(int studentId)
        {
            try
            {
                var _item = await _modelSvc.GetByStudentId(studentId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<StudentCommunityServiceActivityDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the student community service activities by student id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/studentCommunityServiceActivities/byActivityId/5
        [HttpGet("byActivityId/{activityId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentCommunityServiceActivityDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByActivityId(int activityId)
        {
            try
            {
                var _item = await _modelSvc.GetByActivityId(activityId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<StudentCommunityServiceActivityDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the student community service activities by activity id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
