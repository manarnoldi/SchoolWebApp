using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.CBE.Assessments.BroadOutcome;
using SchoolWebApp.Core.DTOs.Class.LearningLevel;
using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Assessments;

namespace SchoolWebApp.API.Controllers.CBE.Assessments
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BroadOutcomesController : ControllerBase
    {
        private readonly ILogger<BroadOutcomesController> _logger;
        private readonly IBroadOutcomeService _modelSvc;
        private readonly IMapper _mapper;
        public BroadOutcomesController(ILogger<BroadOutcomesController> logger, IBroadOutcomeService service, IMapper mapper)
        {
            _logger = logger;
            _modelSvc = service;
            _mapper = mapper;
        }

        // GET: api/broadoutcomes
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<BroadOutcomeDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var BroadOutcomes = await _modelSvc.GetAll();
                var BroadOutcomesDtos = _mapper.Map<List<BroadOutcomeDto>>(BroadOutcomes);
                return Ok(BroadOutcomesDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all  broad outcomes list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/broadoutcomes/paginated
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<BroadOutcomeDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _modelSvc.GetPaginatedData(pageNumber ?? 1, pageSize ?? 10);
                var mappedData = _mapper.Map<List<BroadOutcomeDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<BroadOutcomeDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated broad outcomes.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //GET: api/broadoutcomes/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BroadOutcomeDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _modelSvc.GetById(id);

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<BroadOutcomeDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the broad outcome by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST: api/broadoutcomes
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BroadOutcomeDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateBroadOutcomeDto model)
        {
            if (ModelState.IsValid)
            {
                if (await _modelSvc.IsExists("Name", model.Name))
                    return Conflict(new { message = $"The broad outcome name - '{model.Name}' already exists" });
                try
                {
                    var _item = _mapper.Map<BroadOutcome>(model);
                    _modelSvc.Create(_item);
                    await _modelSvc.SaveChangesAsync();

                    var returnItem = _mapper.Map<BroadOutcomeDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the broad outcome");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the broad outcome - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT: api/broadoutcomes
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(BroadOutcomeDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _modelSvc.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The broad outcome of Id- '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var _item = _mapper.Map<BroadOutcome>(model);
                    _modelSvc.Update(_item);
                    await _modelSvc.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the broad outcome - {ex.Message}");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the broad outcome");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE: api/broadoutcomes/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _modelSvc.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The broad outcome of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _modelSvc.GetById(id);
                _modelSvc.Delete(entity);
                await _modelSvc.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the broad outcome.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the broad outcome - " + ex.Message);
            }
        }

        // GET: api/broadoutcomes/byEducationLevelId/5
        [HttpGet("byEducationLevelId/{educationLevelId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BroadOutcomeDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByEducationLevelId(int educationLevelId)
        {
            try
            {
                var _item = await _modelSvc.GetByEducationLevelId(educationLevelId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<BroadOutcomeDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the broad outcomes by education level id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
