using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.CBE.CommunityService.CommunityServiceActivity;
using SchoolWebApp.Core.Entities.CBE.CommunityService;
using SchoolWebApp.Core.Interfaces.IServices.CBE.CommunityService;

namespace SchoolWebApp.API.Controllers.CBE.CommunityService
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommunityServiceActivitiesController : ControllerBase
    {
        private readonly ILogger<CommunityServiceActivitiesController> _logger;
        private readonly ICommunityServiceActivityService _modelSvc;
        private readonly IMapper _mapper;
        public CommunityServiceActivitiesController(ILogger<CommunityServiceActivitiesController> logger, ICommunityServiceActivityService service, IMapper mapper)
        {
            _logger = logger;
            _modelSvc = service;
            _mapper = mapper;
        }

        // GET: api/communityServiceActivities
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CommunityServiceActivityDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var items = await _modelSvc.Find();
                var itemsDtos = _mapper.Map<List<CommunityServiceActivityDto>>(items);
                return Ok(itemsDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all community service activities list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/communityServiceActivities/paginated
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CommunityServiceActivityDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _modelSvc.GetPaginatedData(pageNumber ?? 1, pageSize ?? 10);
                var mappedData = _mapper.Map<List<CommunityServiceActivityDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<CommunityServiceActivityDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated community service activities.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //GET: api/communityServiceActivities/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CommunityServiceActivityDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _modelSvc.GetById(id);

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<CommunityServiceActivityDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the community service activity by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST: api/communityServiceActivities
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CommunityServiceActivityDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateCommunityServiceActivityDto model)
        {
            if (ModelState.IsValid)
            {
                if (await _modelSvc.IsExists("Name", model.Name))
                    return Conflict(new { message = $"The community service activity name - '{model.Name}' already exists" });
                try
                {
                    var _item = _mapper.Map<SchoolWebApp.Core.Entities.CBE.CommunityService.CommunityServiceActivity>(model);
                    _modelSvc.Create(_item);
                    await _modelSvc.SaveChangesAsync();

                    var returnItem = _mapper.Map<CommunityServiceActivityDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the community service activity");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the community service activity - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT: api/communityServiceActivities
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(CommunityServiceActivityDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _modelSvc.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The community service activity of Id- '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var _item = _mapper.Map<SchoolWebApp.Core.Entities.CBE.CommunityService.CommunityServiceActivity>(model);
                    _modelSvc.Update(_item);
                    await _modelSvc.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the community service activity - {ex.Message}");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the community service activity");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE: api/communityServiceActivities/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _modelSvc.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The community service activity of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _modelSvc.GetById(id);
                _modelSvc.Delete(entity);
                await _modelSvc.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the community service activity.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the community service activity - " + ex.Message);
            }
        }
    }
}
