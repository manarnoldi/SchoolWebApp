using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.API.Controllers.School;
using SchoolWebApp.Core.DTOs.School.Department;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.Entities.School;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.DTOs.Settings.EmploymentType;
using SchoolWebApp.Core.Entities.Settings;
using SchoolWebApp.Core.Entities.Staff;

namespace SchoolWebApp.API.Controllers.Settings
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmploymentTypesController : ControllerBase
    {
        private readonly ILogger<EmploymentTypesController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public EmploymentTypesController(ILogger<EmploymentTypesController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/employmentTypes
        /// <summary>
        /// A method for retrieving all employment types
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<EmploymentTypeDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(_mapper.Map<List<EmploymentTypeDto>>(await _unitOfWork.EmploymentTypes.GetAll()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all employment types list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/employmentTypes/paginated
        /// <summary>
        /// A method for retrieving a list of paginated employment types
        /// </summary>
        /// <param name="pageNumber">The page number to return</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns></returns>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<EmploymentTypeDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _unitOfWork.EmploymentTypes.GetPaginatedData(pageNumber ?? 1, pageSize ?? 4);
                var mappedData = _mapper.Map<List<EmploymentTypeDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<EmploymentTypeDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated employment types.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/employmentTypes/5
        /// <summary>
        /// A method for retrieving employment type record by Id.
        /// </summary>
        /// <param name="id">The employment type Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EmploymentTypeDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _unitOfWork.EmploymentTypes.GetById(id);

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<EmploymentTypeDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the employment type details by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/employmentType
        /// <summary>
        /// A method for creating an employment type record.
        /// </summary>
        /// <param name="model">The employment type record to be created</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EmploymentTypeDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateEmploymentTypeDto model)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.EmploymentTypes.IsExists("Name", model.Name))
                    return Conflict(new { message = $"The employment type name - '{model.Name}' already exists" });
                try
                {
                    var _item = _mapper.Map<EmploymentType>(model);
                    _unitOfWork.EmploymentTypes.Create(_item);
                    await _unitOfWork.SaveChangesAsync();
                    var returnItem = _mapper.Map<EmploymentTypeDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the employment type");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the employment type - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/employmentTypes/5
        /// <summary>
        /// A method for updating a employment type record.
        /// </summary>
        /// <param name="model">The employment type record to be updated</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(EmploymentTypeDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _unitOfWork.EmploymentTypes.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The employment type of Id- '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var _item = _mapper.Map<EmploymentType>(model);
                    _unitOfWork.EmploymentTypes.Update(_item);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the employment type.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the employment type.");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE api/employmentTypes/5
        /// <summary>
        /// A method for deleting the employment type record by Id.
        /// </summary>
        /// <param name="id">The employment type Id to be retrieved</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _unitOfWork.EmploymentTypes.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The employment type of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _unitOfWork.EmploymentTypes.GetById(id);
                _unitOfWork.EmploymentTypes.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the employment type.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the employment type - " + ex.Message);
            }
        }
    }
}
