using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.Settings.Relationship;
using SchoolWebApp.Core.Entities.Settings;
using SchoolWebApp.Core.Entities.Staff;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.API.Controllers.Settings
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RelationshipsController : ControllerBase
    {
        private readonly ILogger<RelationshipsController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public RelationshipsController(ILogger<RelationshipsController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/relationships
        /// <summary>
        /// A method for retrieving all relationships
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<RelationshipDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(_mapper.Map<List< RelationshipDto>>(await _unitOfWork.Relationships.GetAll()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all relationships list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/relationships/paginated
        /// <summary>
        /// A method for retrieving a list of paginated relationships
        /// </summary>
        /// <param name="pageNumber">The page number to return</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns></returns>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List< RelationshipDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _unitOfWork.Relationships.GetPaginatedData(pageNumber ?? 1, pageSize ?? 4);
                var mappedData = _mapper.Map<List< RelationshipDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<RelationshipDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated relationships.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/relationships/5
        /// <summary>
        /// A method for retrieving of relationships record by Id.
        /// </summary>
        /// <param name="id">The relationships Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof( RelationshipDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _unitOfWork.Relationships.GetById(id);

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map< RelationshipDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the relationships details by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/relationship
        /// <summary>
        /// A method for creating a relationship record.
        /// </summary>
        /// <param name="model">The relationship record to be created</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof( RelationshipDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(RelationshipDto model)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.Relationships.IsExists("Name", model.Name))
                    return Conflict(new { message = $"The relationship name - '{model.Name}' already exists" });
                try
                {
                    var _item = _mapper.Map<RelationShip>(model);
                    _unitOfWork.Relationships.Create(_item);
                    await _unitOfWork.SaveChangesAsync();
                    var returnItem = _mapper.Map< RelationshipDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the relationship");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the relationship - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/relationships/5
        /// <summary>
        /// A method for updating a relationship record.
        /// </summary>
        /// <param name="model">The relationship record to be updated</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit( RelationshipDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _unitOfWork.Relationships.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The relationship of Id- '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var _item = _mapper.Map<RelationShip>(model);
                    _unitOfWork.Relationships.Update(_item);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the relationship.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the relationship.");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE api/relationships/5
        /// <summary>
        /// A method for deleting the relationship record by Id.
        /// </summary>
        /// <param name="id">The relationship Id to be retrieved</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _unitOfWork.Relationships.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The relationship of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _unitOfWork.Relationships.GetById(id);
                _unitOfWork.Relationships.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the relationship.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the relationship - " + ex.Message);
            }
        }
    }
}
