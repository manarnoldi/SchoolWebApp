using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.CBE.Cocurriculum.CoCurriculumScoreType;
using SchoolWebApp.Core.Entities.CBE.Cocurriculum;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Cocurriculum;

namespace SchoolWebApp.API.Controllers.CBE.Cocurriculum
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CoCurriculumScoreTypesController : ControllerBase
    {
        private readonly ILogger<CoCurriculumScoreTypesController> _logger;
        private readonly ICoCurriculumScoreTypeService _modelSvc;
        private readonly IMapper _mapper;
        public CoCurriculumScoreTypesController(ILogger<CoCurriculumScoreTypesController> logger, ICoCurriculumScoreTypeService service, IMapper mapper)
        {
            _logger = logger;
            _modelSvc = service;
            _mapper = mapper;
        }

        // GET: api/cocurriculumscoretypes
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CoCurriculumScoreTypeDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var items = await _modelSvc.GetAll();
                var itemDtos = _mapper.Map<List<CoCurriculumScoreTypeDto>>(items);
                return Ok(itemDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all co-curriculum score types list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/cocurriculumscoretypes/paginated
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CoCurriculumScoreTypeDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _modelSvc.GetPaginatedData(pageNumber ?? 1, pageSize ?? 10);
                var mappedData = _mapper.Map<List<CoCurriculumScoreTypeDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<CoCurriculumScoreTypeDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated co-curriculum score types.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //GET: api/cocurriculumscoretypes/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CoCurriculumScoreTypeDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _modelSvc.GetById(id);

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<CoCurriculumScoreTypeDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the co-curriculum score type by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST: api/cocurriculumscoretypes
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CoCurriculumScoreTypeDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateCoCurriculumScoreTypeDto model)
        {
            if (ModelState.IsValid)
            {
                if (await _modelSvc.IsExists("Name", model.Name))
                    return Conflict(new { message = $"The co-curriculum score type name - '{model.Name}' already exists" });
                try
                {
                    var _item = _mapper.Map<CoCurriculumScoreType>(model);
                    _modelSvc.Create(_item);
                    await _modelSvc.SaveChangesAsync();

                    var returnItem = _mapper.Map<CoCurriculumScoreTypeDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the co-curriculum score type");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the co-curriculum score type - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT: api/cocurriculumscoretypes
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(CoCurriculumScoreTypeDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _modelSvc.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The co-curriculum score type of Id- '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var _item = _mapper.Map<CoCurriculumScoreType>(model);
                    _modelSvc.Update(_item);
                    await _modelSvc.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the co-curriculum score type - {ex.Message}");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the co-curriculum score type");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE: api/cocurriculumscoretypes/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _modelSvc.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The co-curriculum score type of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _modelSvc.GetById(id);
                _modelSvc.Delete(entity);
                await _modelSvc.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the co-curriculum score type.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the co-curriculum score type - " + ex.Message);
            }
        }
    }
}
