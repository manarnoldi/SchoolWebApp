using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.Settings.Nationality;
using SchoolWebApp.Core.Entities.Settings;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.API.Controllers.Settings
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NationalitiesController : ControllerBase
    {
        private readonly ILogger<NationalitiesController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public NationalitiesController(ILogger<NationalitiesController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/nationalities
        /// <summary>
        /// A method for retrieving all nationalities
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<NationalityDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(_mapper.Map<List<NationalityDto>>(await _unitOfWork.Nationalities.GetAll()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all nationalities list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/nationalities/paginated
        /// <summary>
        /// A method for retrieving a list of paginated nationalities
        /// </summary>
        /// <param name="pageNumber">The page number to return</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns></returns>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<NationalityDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _unitOfWork.Nationalities.GetPaginatedData(pageNumber ?? 1, pageSize ?? 4);
                var mappedData = _mapper.Map<List<NationalityDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<NationalityDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated nationalities.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/nationalities/5
        /// <summary>
        /// A method for retrieving of nationalities record by Id.
        /// </summary>
        /// <param name="id">The nationalities Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NationalityDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _unitOfWork.Nationalities.GetById(id);

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<NationalityDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the nationalities details by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/designation
        /// <summary>
        /// A method for creating a natonality record.
        /// </summary>
        /// <param name="model">The nationality record to be created</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NationalityDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateNationalityDto model)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.Nationalities.IsExists("Name", model.Name))
                    return Conflict(new { message = $"The nationality name - '{model.Name}' already exists" });
                try
                {
                    var _item = _mapper.Map<Nationality>(model);
                    _unitOfWork.Nationalities.Create(_item);
                    await _unitOfWork.SaveChangesAsync();
                    var returnItem = _mapper.Map<NationalityDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the nationality");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the nationality - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/nationalities/5
        /// <summary>
        /// A method for updating a nationality record.
        /// </summary>
        /// <param name="model">The nationality record to be updated</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(NationalityDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _unitOfWork.Nationalities.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The nationality of Id- '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var existingItem = await _unitOfWork.Nationalities.GetById(model.Id);
                    //Manual mapping
                    existingItem.Name = model.Name;
                    existingItem.Description = model.Description;
                    _unitOfWork.Nationalities.Update(existingItem);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the nationality.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the nationality.");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE api/nationalities/5
        /// <summary>
        /// A method for deleting the nationality record by Id.
        /// </summary>
        /// <param name="id">The nationality Id to be retrieved</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _unitOfWork.Nationalities.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The nationality of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _unitOfWork.Nationalities.GetById(id);
                _unitOfWork.Nationalities.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the nationality.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the nationality - " + ex.Message);
            }
        }
    }
}
