using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs.School.Department;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.DTOs.School.EducationLevelType;
using SchoolWebApp.Core.Entities.School;

namespace SchoolWebApp.API.Controllers.School
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EducationLevelTypesController : ControllerBase
    {
        private readonly ILogger<EducationLevelTypesController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public EducationLevelTypesController(ILogger<EducationLevelTypesController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/educationLevelTypes
        /// <summary>
        /// A method for retrieving all education level types
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<EducationLevelTypeDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(_mapper.Map<List<EducationLevelTypeDto>>(await _unitOfWork.EducationLevelTypes.GetAll()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all education level types list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/educationLevelTypes/paginated
        /// <summary>
        /// A method for retrieving a list of paginated education level types
        /// </summary>
        /// <param name="pageNumber">The page number to return</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns></returns>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<EducationLevelTypeDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _unitOfWork.EducationLevelTypes.GetPaginatedData(pageNumber ?? 1, pageSize ?? 4);
                var mappedData = _mapper.Map<List<EducationLevelTypeDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<EducationLevelTypeDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated education level types.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/educationLevelTypes/5
        /// <summary>
        /// A method for retrieving of education level type record by Id.
        /// </summary>
        /// <param name="id">The education level type Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EducationLevelTypeDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _unitOfWork.EducationLevelTypes.GetById(id);

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<EducationLevelTypeDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the education level type details by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/educationLevelTypes
        /// <summary>
        /// A method for creating school an education level type record.
        /// </summary>
        /// <param name="model">The education level type record to be created</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EducationLevelTypeDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateEducationLevelTypeDto model)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.EducationLevelTypes.IsExists("Name", model.Name))
                    return Conflict(new { message = $"The education level type name - '{model.Name}' already exists" });
                try
                {
                    var _item = _mapper.Map<EducationLevelType>(model);
                    _unitOfWork.EducationLevelTypes.Create(_item);
                    await _unitOfWork.SaveChangesAsync();
                    var returnItem = _mapper.Map<EducationLevelTypeDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the education level type");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the education level type - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/educationLevelTypes/5
        /// <summary>
        /// A method for updating a education level type record.
        /// </summary>
        /// <param name="model">The education level type record to be updated</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(EducationLevelTypeDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _unitOfWork.EducationLevelTypes.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The education level type of Id- '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var existingItem = await _unitOfWork.EducationLevelTypes.GetById(model.Id);
                    //Manual mapping
                    existingItem.Name = model.Name;
                    existingItem.Abbr = model.Abbr;
                    existingItem.Description = model.Description;
                    _unitOfWork.EducationLevelTypes.Update(existingItem);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the education level type.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the education level type.");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE api/educationLevelTypes/5
        /// <summary>
        /// A method for deleting the education level type record by Id.
        /// </summary>
        /// <param name="id">The education level type Id to be retrieved</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _unitOfWork.EducationLevelTypes.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The education level type of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _unitOfWork.EducationLevelTypes.GetById(id);
                _unitOfWork.EducationLevelTypes.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the education level type.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the education level type - " + ex.Message);
            }
        }
    }
}
