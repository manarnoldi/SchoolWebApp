using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.CBE.Cocurriculum.CoCurriculumScore;
using SchoolWebApp.Core.Entities.CBE.Cocurriculum;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Cocurriculum;

namespace SchoolWebApp.API.Controllers.CBE.Cocurriculum
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CoCurriculumScoresController : ControllerBase
    {
        private readonly ILogger<CoCurriculumScoresController> _logger;
        private readonly ICoCurriculumScoreService _modelSvc;
        private readonly IMapper _mapper;
        public CoCurriculumScoresController(ILogger<CoCurriculumScoresController> logger, ICoCurriculumScoreService service, IMapper mapper)
        {
            _logger = logger;
            _modelSvc = service;
            _mapper = mapper;
        }

        // GET: api/cocurriculumscores
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CoCurriculumScoreDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var items = await _modelSvc.Find(includeProperties: "CoCurriculumScoreType");
                var itemDtos = _mapper.Map<List<CoCurriculumScoreDto>>(items);
                return Ok(itemDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all co-curriculum scores list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/cocurriculumscores/paginated
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CoCurriculumScoreDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _modelSvc.GetPaginatedData(pageNumber ?? 1, pageSize ?? 10,
                    includeProperties: "CoCurriculumScoreType");
                var mappedData = _mapper.Map<List<CoCurriculumScoreDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<CoCurriculumScoreDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated co-curriculum scores.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //GET: api/cocurriculumscores/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CoCurriculumScoreDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _modelSvc.GetById(id, includeProperties: "CoCurriculumScoreType");

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<CoCurriculumScoreDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the co-curriculum score by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //GET: api/cocurriculumscores/byScoreTypeId/5
        [HttpGet("byScoreTypeId/{scoreTypeId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CoCurriculumScoreDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByScoreTypeId(int scoreTypeId)
        {
            try
            {
                var _item = await _modelSvc.GetByScoreTypeId(scoreTypeId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<CoCurriculumScoreDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the co-curriculum scores by score type id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST: api/cocurriculumscores
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CoCurriculumScoreDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateCoCurriculumScoreDto model)
        {
            if (ModelState.IsValid)
            {
                if (await _modelSvc.IsExists("Name", model.Name))
                    return Conflict(new { message = $"The co-curriculum score name - '{model.Name}' already exists" });
                try
                {
                    var _item = _mapper.Map<CoCurriculumScore>(model);
                    _modelSvc.Create(_item);
                    await _modelSvc.SaveChangesAsync();

                    var returnItem = _mapper.Map<CoCurriculumScoreDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the co-curriculum score");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the co-curriculum score - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT: api/cocurriculumscores
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(CoCurriculumScoreDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _modelSvc.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The co-curriculum score of Id- '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var _item = _mapper.Map<CoCurriculumScore>(model);
                    _modelSvc.Update(_item);
                    await _modelSvc.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the co-curriculum score - {ex.Message}");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the co-curriculum score");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE: api/cocurriculumscores/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _modelSvc.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The co-curriculum score of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _modelSvc.GetById(id);
                _modelSvc.Delete(entity);
                await _modelSvc.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the co-curriculum score.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the co-curriculum score - " + ex.Message);
            }
        }
    }
}
