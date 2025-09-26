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
    public class SpecificOutcomesController : ControllerBase
    {
        private readonly ILogger<SpecificOutcomesController> _logger;
        private readonly ISpecificOutcomeService _modelSvc;
        private readonly ICompetencyService _modelCompetencySvc;
        private readonly IMapper _mapper;
        public SpecificOutcomesController(ILogger<SpecificOutcomesController> logger, ISpecificOutcomeService service,
            ICompetencyService modelCompetencySvc, IMapper mapper)
        {
            _logger = logger;
            _modelSvc = service;
            _modelCompetencySvc = modelCompetencySvc;
            _mapper = mapper;
        }

        // GET: api/specificoutcomes
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SpecificOutcomeDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var specificOutcomes = await _modelSvc.Find(includeProperties: "SubStrand,LearningLevel,BroadOutcome,GeneralOutcome");
                var specificOutcomesDtos = _mapper.Map<List<SpecificOutcomeDto>>(specificOutcomes);
                return Ok(specificOutcomesDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all specific outcomes list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/specificoutcomes/paginated
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SpecificOutcomeDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _modelSvc.GetPaginatedData(pageNumber ?? 1, pageSize ?? 10,
                    includeProperties: "SubStrand,LearningLevel,BroadOutcome,GeneralOutcome");
                var mappedData = _mapper.Map<List<SpecificOutcomeDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<SpecificOutcomeDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated specific outcomes.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //GET: api/specificoutcomes/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SpecificOutcomeDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _modelSvc.GetById(id, includeProperties: "SubStrand,LearningLevel,BroadOutcome,GeneralOutcome");

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<SpecificOutcomeDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the specific outcome by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST: api/specificoutcomes
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SpecificOutcomeDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateSpecificOutcomeDto model)
        {
            if (ModelState.IsValid)
            {
                if (await _modelSvc.IsExists("Name", model.Name))
                    return Conflict(new { message = $"The specific outcome name - '{model.Name}' already exists" });
                try
                {
                    var _item = _mapper.Map<SpecificOutcome>(model);
                    _modelSvc.Create(_item);
                    await _modelSvc.SaveChangesAsync();

                    var returnItem = _mapper.Map<SpecificOutcomeDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the specific outcome");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the specific outcome - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT: api/specificoutcomes
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(SpecificOutcomeDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _modelSvc.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The specific outcome of Id- '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var _item = _mapper.Map<SpecificOutcome>(model);
                    _modelSvc.Update(_item);
                    await _modelSvc.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the specific outcome - {ex.Message}");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the specific outcome");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE: api/specificoutcomes/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _modelSvc.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The specific outcome of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _modelSvc.GetById(id);
                _modelSvc.Delete(entity);
                await _modelSvc.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the specific outcome.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the specific outcome - " + ex.Message);
            }
        }

        // GET: api/specificoutcomes/byLearningLevelId/5
        [HttpGet("byLearningLevelId/{learningLevelId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SpecificOutcomeDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByLearningLevelId(int learningLevelId)
        {
            try
            {
                var _item = await _modelSvc.GetByLearningLevelId(learningLevelId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<SpecificOutcomeDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the specific outcomes by learning level id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/specificoutcomes/bySubStrandId/5?learningLevelId=3
        [HttpGet("bySubStrandId/{subStrandId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SpecificOutcomeDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBySubStrandId(int subStrandId, int? learningLevelId)
        {
            try
            {
                var _item = await _modelSvc.GetBySubStrandId(subStrandId, learningLevelId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<SpecificOutcomeDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the specific outcomes by sub strand id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        

        // GET: api/specificoutcomes/byBroadOutcomeId/5?learningLevelId=3
        [HttpGet("byBroadOutcomeId/{broadOutcomeId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SpecificOutcomeDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBroadOutcomeId(int broadOutcomeId, int? learningLevelId)
        {
            try
            {
                var _item = await _modelSvc.GetByBroadOutcomeId(broadOutcomeId, learningLevelId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<SpecificOutcomeDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the specific outcomes by broad outcome id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/specificoutcomes/byGeneralOutcomeId/5?learningLevelId=3
        [HttpGet("byGeneralOutcomeId/{generalOutcomeId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SpecificOutcomeDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGeneralOutcomeId(int generalOutcomeId, int? learningLevelId)
        {
            try
            {
                var _item = await _modelSvc.GetByGeneralOutcomeId(generalOutcomeId, learningLevelId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<SpecificOutcomeDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the specific outcomes by general outcome id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/specificoutcomes/5/competencies
        [HttpGet("{specificOutcomeId}/competencies")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CompetencyDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCompetenciesForSpecificOutcome(int specificOutcomeId)
        {
            try
            {
                var competencies = await _modelSvc.GetCompetenciesForSpecificOutcomeId(specificOutcomeId);
                var competenciesDtos = _mapper.Map<List<CompetencyDto>>(competencies);
                return Ok(competenciesDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving competencies list for a specific outcome.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST: api/specificoutcomes/5/competencies/5
        [HttpPost("{specificOutcomeId}/competencies/{competencyId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SpecificOutcomeDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddCompetencyToSpecificOutcome(int specificOutcomeId, int competencyId)
        {
            try
            {
                var savedOutcome = await _modelSvc.AddCompetencyToSpecificOutcome(specificOutcomeId, competencyId);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while adding the competency to the specific outcome.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // DELETE: api/specificoutcomes/5/competencies/5
        [HttpDelete("{specificOutcomeId}/competencies/{competencyId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCompetencyFromSpecificOutcome(int specificOutcomeId, int competencyId)
        {
            try
            {
                await _modelSvc.RemoveCompetencyFromSpecificOutcome(specificOutcomeId, competencyId);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while removing the specific outcome.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the specific outcome - " + ex.Message);
            }
        }

    }
}
