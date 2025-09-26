using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.CBE.Assessments.GeneralOutcome;
using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Assessments;

namespace SchoolWebApp.API.Controllers.CBE.Assessments
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GeneralOutcomesController : ControllerBase
    {
        private readonly ILogger<GeneralOutcomesController> _logger;
        private readonly IGeneralOutcomeService _modelSvc;
        private readonly IMapper _mapper;
        public GeneralOutcomesController(ILogger<GeneralOutcomesController> logger, IGeneralOutcomeService service, IMapper mapper)
        {
            _logger = logger;
            _modelSvc = service;
            _mapper = mapper;
        }

        // GET: api/generaloutcomes
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GeneralOutcomeDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var generalOutcomes = await _modelSvc.Find(includeProperties: "EducationLevelType");
                var generalOutcomesDtos = _mapper.Map<List<GeneralOutcomeDto>>(generalOutcomes);
                return Ok(generalOutcomesDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all general outcomes list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/generaloutcomes/paginated
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GeneralOutcomeDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _modelSvc.GetPaginatedData(pageNumber ?? 1, pageSize ?? 10, includeProperties: "EducationLevelType");
                var mappedData = _mapper.Map<List<GeneralOutcomeDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<GeneralOutcomeDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated general outcomes.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //GET: api/generaloutcomes/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GeneralOutcomeDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _modelSvc.GetById(id, includeProperties: "EducationLevelType");

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<GeneralOutcomeDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the broad outcome by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST: api/generaloutcomes
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GeneralOutcomeDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateGeneralOutcomeDto model)
        {
            if (ModelState.IsValid)
            {
                if (await _modelSvc.IsExists("Name", model.Name))
                    return Conflict(new { message = $"The broad outcome name - '{model.Name}' already exists" });
                try
                {
                    var _item = _mapper.Map<GeneralOutcome>(model);
                    _modelSvc.Create(_item);
                    await _modelSvc.SaveChangesAsync();

                    var returnItem = _mapper.Map<GeneralOutcomeDto>(_item);
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

        // PUT: api/generaloutcomes
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(GeneralOutcomeDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _modelSvc.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The broad outcome of Id- '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var _item = _mapper.Map<GeneralOutcome>(model);
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

        // DELETE: api/generaloutcomes/5
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

        // GET: api/generaloutcomes/byEducationLevelTypeId/5
        [HttpGet("byEducationLevelTypeId/{educationLevelTypeId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GeneralOutcomeDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByEducationLevelTypeId(int educationLevelTypeId)
        {
            try
            {
                var _item = await _modelSvc.GetByEducationLevelTypeId(educationLevelTypeId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<GeneralOutcomeDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the general outcomes by education level type id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}
