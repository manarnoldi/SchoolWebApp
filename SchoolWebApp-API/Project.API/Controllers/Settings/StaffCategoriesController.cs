using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.Settings.StaffCategory;
using SchoolWebApp.Core.Entities.Settings;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.API.Controllers.Settings
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StaffCategoriesController : ControllerBase
    {
        private readonly ILogger<StaffCategoriesController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public StaffCategoriesController(ILogger<StaffCategoriesController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/staffCategories
        /// <summary>
        /// A method for retrieving all staff categories
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StaffCategoryDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(_mapper.Map<List<StaffCategoryDto>>(await _unitOfWork.StaffCategories.GetAll()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all staff categories list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/staffCategories/paginated
        /// <summary>
        /// A method for retrieving a list of paginated staff categories
        /// </summary>
        /// <param name="pageNumber">The page number to return</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns></returns>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StaffCategoryDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _unitOfWork.StaffCategories.GetPaginatedData(pageNumber ?? 1, pageSize ?? 4);
                var mappedData = _mapper.Map<List<StaffCategoryDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<StaffCategoryDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated staff categories.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/staffCategories/5
        /// <summary>
        /// A method for retrieving of staff categories record by Id.
        /// </summary>
        /// <param name="id">The staff categories Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StaffCategoryDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _unitOfWork.StaffCategories.GetById(id);

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<StaffCategoryDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the staff categories details by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/staffCategories
        /// <summary>
        /// A method for creating a staff category record.
        /// </summary>
        /// <param name="model">The staff category record to be created</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StaffCategoryDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateStaffCategoryDto model)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.StaffCategories.IsExists("Name", model.Name))
                    return Conflict(new { message = $"The staff category name - '{model.Name}' already exists" });
                try
                {
                    var _item = _mapper.Map<StaffCategory>(model);
                    _unitOfWork.StaffCategories.Create(_item);
                    await _unitOfWork.SaveChangesAsync();
                    var returnItem = _mapper.Map<StaffCategoryDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the staff category");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the staff category - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/staffCategories/5
        /// <summary>
        /// A method for updating a staff category record.
        /// </summary>
        /// <param name="model">The staff category record to be updated</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(StaffCategoryDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _unitOfWork.StaffCategories.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The staff category of Id- '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var existingItem = await _unitOfWork.StaffCategories.GetById(model.Id);
                    //Manual mapping
                    existingItem.Name = model.Name;
                    existingItem.Description = model.Description;
                    _unitOfWork.StaffCategories.Update(existingItem);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the staff category.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the staff category.");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE api/staffCategories/5
        /// <summary>
        /// A method for deleting the staff category record by Id.
        /// </summary>
        /// <param name="id">The staff category Id to be retrieved</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _unitOfWork.StaffCategories.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The staff category of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _unitOfWork.StaffCategories.GetById(id);
                _unitOfWork.StaffCategories.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the staff category.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the staff category - " + ex.Message);
            }
        }
    }
}
