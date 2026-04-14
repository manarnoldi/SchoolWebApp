using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.CBE.Assessments.LessonAllocation;
using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Assessments;

namespace SchoolWebApp.API.Controllers.CBE.Assessments
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LessonAllocationsController : ControllerBase
    {
        private readonly ILogger<LessonAllocationsController> _logger;
        private readonly ILessonAllocationService _modelSvc;
        private readonly IMapper _mapper;
        public LessonAllocationsController(ILogger<LessonAllocationsController> logger, ILessonAllocationService service, IMapper mapper)
        {
            _logger = logger;
            _modelSvc = service;
            _mapper = mapper;
        }

        // GET: api/lessonAllocations
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<LessonAllocationDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var items = await _modelSvc.Find(includeProperties: "Subject,LearningLevel");
                var itemsDtos = _mapper.Map<List<LessonAllocationDto>>(items);
                return Ok(itemsDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all lesson allocations list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/lessonAllocations/paginated
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<LessonAllocationDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _modelSvc.GetPaginatedData(pageNumber ?? 1, pageSize ?? 10, includeProperties: "Subject,LearningLevel");
                var mappedData = _mapper.Map<List<LessonAllocationDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<LessonAllocationDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated lesson allocations.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //GET: api/lessonAllocations/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LessonAllocationDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _modelSvc.GetById(id, includeProperties: "Subject,LearningLevel");

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<LessonAllocationDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the lesson allocation by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST: api/lessonAllocations
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LessonAllocationDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateLessonAllocationDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var _item = _mapper.Map<SchoolWebApp.Core.Entities.CBE.Assessments.LessonAllocation>(model);
                    _modelSvc.Create(_item);
                    await _modelSvc.SaveChangesAsync();

                    var returnItem = _mapper.Map<LessonAllocationDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the lesson allocation");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the lesson allocation - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT: api/lessonAllocations
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(LessonAllocationDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _modelSvc.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The lesson allocation of Id- '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var _item = _mapper.Map<SchoolWebApp.Core.Entities.CBE.Assessments.LessonAllocation>(model);
                    _modelSvc.Update(_item);
                    await _modelSvc.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the lesson allocation - {ex.Message}");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the lesson allocation");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE: api/lessonAllocations/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _modelSvc.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The lesson allocation of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _modelSvc.GetById(id);
                _modelSvc.Delete(entity);
                await _modelSvc.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the lesson allocation.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the lesson allocation - " + ex.Message);
            }
        }

        // GET: api/lessonAllocations/bySubjectId/5
        [HttpGet("bySubjectId/{subjectId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<LessonAllocationDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBySubjectId(int subjectId)
        {
            try
            {
                var _item = await _modelSvc.GetBySubjectId(subjectId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<LessonAllocationDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the lesson allocations by subject id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/lessonAllocations/byLearningLevelId/5
        [HttpGet("byLearningLevelId/{learningLevelId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<LessonAllocationDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByLearningLevelId(int learningLevelId)
        {
            try
            {
                var _item = await _modelSvc.GetByLearningLevelId(learningLevelId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<LessonAllocationDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the lesson allocations by learning level id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
