using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.Settings.Occupation;
using SchoolWebApp.Core.Entities.Settings;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.API.Controllers.Settings
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OccupationsController : ControllerBase
    {
        private readonly ILogger<OccupationsController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public OccupationsController(ILogger<OccupationsController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/occupations
        /// <summary>
        /// A method for retrieving all occupations
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<OccupationDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(_mapper.Map<List<OccupationDto>>(await _unitOfWork.Occupations.GetAll()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all occupations list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/occupations/paginated
        /// <summary>
        /// A method for retrieving a list of paginated occupations
        /// </summary>
        /// <param name="pageNumber">The page number to return</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns></returns>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<OccupationDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _unitOfWork.Occupations.GetPaginatedData(pageNumber ?? 1, pageSize ?? 4);
                var mappedData = _mapper.Map<List<OccupationDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<OccupationDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated occupations.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/occupations/5
        /// <summary>
        /// A method for retrieving of occupations record by Id.
        /// </summary>
        /// <param name="id">The occupations Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OccupationDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _unitOfWork.Occupations.GetById(id);

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<OccupationDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the occupations details by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/occupation
        /// <summary>
        /// A method for creating a occupation record.
        /// </summary>
        /// <param name="model">The occupation record to be created</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OccupationDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateOccupationDto model)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.Occupations.IsExists("Name", model.Name))
                    return Conflict(new { message = $"The occupation name - '{model.Name}' already exists" });
                try
                {
                    var _item = _mapper.Map<Occupation>(model);
                    _unitOfWork.Occupations.Create(_item);
                    await _unitOfWork.SaveChangesAsync();
                    var returnItem = _mapper.Map<OccupationDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the occupation");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the occupation - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/occupations/5
        /// <summary>
        /// A method for updating a occupation record.
        /// </summary>
        /// <param name="model">The occupation record to be updated</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(OccupationDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _unitOfWork.Occupations.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The occupation of Id- '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var existingItem = await _unitOfWork.Occupations.GetById(model.Id);
                    //Manual mapping
                    existingItem.Name = model.Name;
                    existingItem.Description = model.Description;
                    _unitOfWork.Occupations.Update(existingItem);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the occupation.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the occupation.");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE api/occupations/5
        /// <summary>
        /// A method for deleting the occupation record by Id.
        /// </summary>
        /// <param name="id">The occupation Id to be retrieved</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _unitOfWork.Occupations.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The occupation of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _unitOfWork.Occupations.GetById(id);
                _unitOfWork.Occupations.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the occupation.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the occupation - " + ex.Message);
            }
        }
    }
}
