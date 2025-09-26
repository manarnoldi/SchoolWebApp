using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.CBE.Assessments.Competency;
using SchoolWebApp.Core.DTOs.CBE.Assessments.SpecificOutcome;
using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Assessments;

namespace SchoolWebApp.API.Controllers.CBE.Assessments
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CompetenciesController : ControllerBase
    {
        private readonly ILogger<CompetenciesController> _logger;
        private readonly ICompetencyService _modelSvc;
        private readonly IMapper _mapper;
        public CompetenciesController(ILogger<CompetenciesController> logger, ICompetencyService service, IMapper mapper)
        {
            _logger = logger;
            _modelSvc = service;
            _mapper = mapper;
        }

        // GET: api/competencies
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CompetencyDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var competencies = await _modelSvc.GetAll();
                var competenciesDtos = _mapper.Map<List<CompetencyDto>>(competencies);
                return Ok(competenciesDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all competencies list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/competencies/paginated
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CompetencyDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _modelSvc.GetPaginatedData(pageNumber ?? 1, pageSize ?? 10);
                var mappedData = _mapper.Map<List<CompetencyDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<CompetencyDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated competencies.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //GET: api/competencies/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CompetencyDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _modelSvc.GetById(id);

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<CompetencyDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the competency by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST: api/competencies
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CompetencyDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateCompetencyDto model)
        {
            if (ModelState.IsValid)
            {
                if (await _modelSvc.IsExists("Name", model.Name))
                    return Conflict(new { message = $"The competency name - '{model.Name}' already exists" });
                try
                {
                    var _item = _mapper.Map<Competency>(model);
                    _modelSvc.Create(_item);
                    await _modelSvc.SaveChangesAsync();

                    var returnItem = _mapper.Map<CompetencyDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the competency");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the competency - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT: api/competencies
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(CompetencyDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _modelSvc.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The competency of Id- '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var _item = _mapper.Map<Competency>(model);
                    _modelSvc.Update(_item);
                    await _modelSvc.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the competency - {ex.Message}");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the competency");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE: api/competencies/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _modelSvc.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The competency of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _modelSvc.GetById(id);
                _modelSvc.Delete(entity);
                await _modelSvc.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the competency.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the competency - " + ex.Message);
            }
        }

        // GET: api/competencies/5/specificOutcomes
        [HttpGet("{competencyId}/specificOutcomes")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SpecificOutcomeDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSpecificOutcomesForCompetency(int competencyId)
        {
            try
            {
                var specificOutcomes = await _modelSvc.GetSpecificOutcomesForCompetencyId(competencyId);
                var specificOutcomesDtos = _mapper.Map<List<SpecificOutcomeDto>>(specificOutcomes);
                return Ok(specificOutcomesDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving specific outcomes list for a specific competency.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
