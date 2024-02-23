using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.Settings.Designation;
using SchoolWebApp.Core.Entities.Settings;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.API.Controllers.Settings
{    
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DesignationsController : ControllerBase
    {
        private readonly ILogger<DesignationsController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public DesignationsController(ILogger<DesignationsController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/designations
        /// <summary>
        /// A method for retrieving all designations
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<DesignationDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(_mapper.Map<List<DesignationDto>>(await _unitOfWork.Designations.GetAll()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all designations list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/designations/paginated
        /// <summary>
        /// A method for retrieving a list of paginated designations
        /// </summary>
        /// <param name="pageNumber">The page number to return</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns></returns>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<DesignationDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _unitOfWork.Designations.GetPaginatedData(pageNumber ?? 1, pageSize ?? 4);
                var mappedData = _mapper.Map<List<DesignationDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<DesignationDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated designations.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/designations/5
        /// <summary>
        /// A method for retrieving of designations record by Id.
        /// </summary>
        /// <param name="id">The designations Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DesignationDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _unitOfWork.Designations.GetById(id);

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<DesignationDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the designations details by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/designations
        /// <summary>
        /// A method for creating a designation record.
        /// </summary>
        /// <param name="model">The designation record to be created</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DesignationDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateDesignationDto model)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.Designations.IsExists("Name", model.Name))
                    return Conflict(new { message = $"The designation name - '{model.Name}' already exists" });
                try
                {
                    var _item = _mapper.Map<Designation>(model);
                    _unitOfWork.Designations.Create(_item);
                    await _unitOfWork.SaveChangesAsync();
                    var returnItem = _mapper.Map<DesignationDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the designation");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the designation - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/designations/5
        /// <summary>
        /// A method for updating a designation record.
        /// </summary>
        /// <param name="model">The designation record to be updated</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(DesignationDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _unitOfWork.Designations.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The designation of Id- '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var existingItem = await _unitOfWork.Designations.GetById(model.Id);
                    //Manual mapping
                    existingItem.Name = model.Name;
                    existingItem.Description = model.Description;
                    _unitOfWork.Designations.Update(existingItem);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the designation.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the designation.");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE api/designations/5
        /// <summary>
        /// A method for deleting the designation record by Id.
        /// </summary>
        /// <param name="id">The designation Id to be retrieved</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _unitOfWork.Designations.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The designation of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _unitOfWork.Designations.GetById(id);
                _unitOfWork.Designations.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the designation.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the designation - " + ex.Message);
            }
        }
    }
}
