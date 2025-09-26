using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.CBE.Assessments.SubStrand;
using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Assessments;

namespace SchoolWebApp.API.Controllers.CBE.Assessments
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SubStrandsController : ControllerBase
    {
        private readonly ILogger<SubStrandsController> _logger;
        private readonly ISubStrandService _modelSvc;
        private readonly IMapper _mapper;
        public SubStrandsController(ILogger<SubStrandsController> logger, ISubStrandService service, IMapper mapper)
        {
            _logger = logger;
            _modelSvc = service;
            _mapper = mapper;
        }

        // GET: api/substrands
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SubStrandDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var subStrands = await _modelSvc.Find(includeProperties: "Strand");
                var subStrandsDtos = _mapper.Map<List<SubStrandDto>>(subStrands);
                return Ok(subStrandsDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all sub strands list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/substrands/paginated
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SubStrandDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _modelSvc.GetPaginatedData(pageNumber ?? 1, pageSize ?? 10, includeProperties: "Strand");
                var mappedData = _mapper.Map<List<SubStrandDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<SubStrandDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated sub strands.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //GET: api/substrands/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SubStrandDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _modelSvc.GetById(id, includeProperties: "Strand");

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<SubStrandDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the sub strand by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST: api/substrands
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SubStrandDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateSubStrandDto model)
        {
            if (ModelState.IsValid)
            {
                if (await _modelSvc.IsExists("Name", model.Name))
                    return Conflict(new { message = $"The sub strand name - '{model.Name}' already exists" });
                try
                {
                    var _item = _mapper.Map<SubStrand>(model);
                    _modelSvc.Create(_item);
                    await _modelSvc.SaveChangesAsync();

                    var returnItem = _mapper.Map<SubStrandDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the sub strand");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the sub strand - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT: api/substrands
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(SubStrandDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _modelSvc.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The sub strand of Id- '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var _item = _mapper.Map<SubStrand>(model);
                    _modelSvc.Update(_item);
                    await _modelSvc.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the sub strand - {ex.Message}");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the sub strand");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE: api/substrands/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _modelSvc.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The sub strand of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _modelSvc.GetById(id);
                _modelSvc.Delete(entity);
                await _modelSvc.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the sub strand.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the sub strand - " + ex.Message);
            }
        }

        // GET: api/substrands/byStrandId/5
        [HttpGet("byStrandId/{strandId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SubStrandDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByStrandId(int strandId)
        {
            try
            {
                var _item = await _modelSvc.GetByStrandId(strandId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<SubStrandDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the sub strands by strand id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
