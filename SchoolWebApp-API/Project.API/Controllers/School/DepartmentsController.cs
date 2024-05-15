using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.School.Department;
using SchoolWebApp.Core.Entities.School;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.API.Controllers.School
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly ILogger<DepartmentsController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public DepartmentsController(ILogger<DepartmentsController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/departments
        /// <summary>
        /// A method for retrieving all departments
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<DepartmentDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(_mapper.Map<List<DepartmentDto>>(await _unitOfWork.Departments.GetAll()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all departments list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/departments/paginated
        /// <summary>
        /// A method for retrieving a list of paginated departments
        /// </summary>
        /// <param name="pageNumber">The page number to return</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns></returns>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<DepartmentDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _unitOfWork.Departments.GetPaginatedData(pageSize ?? 4, pageNumber ?? 1);
                var mappedData = _mapper.Map<List<DepartmentDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<DepartmentDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated departments.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/departments/5
        /// <summary>
        /// A method for retrieving of department record by Id.
        /// </summary>
        /// <param name="id">The department Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DepartmentDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _unitOfWork.Departments.GetById(id);

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<DepartmentDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the department details by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/departments
        /// <summary>
        /// A method for creating school a department record.
        /// </summary>
        /// <param name="model">The department record to be created</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DepartmentDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateDepartmentDto model)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.Departments.IsExists("Name", model.Name))
                    return Conflict(new { message = $"The department name - '{model.Name}' already exists" });
                try
                {
                    var _item = _mapper.Map<Department>(model);
                    _unitOfWork.Departments.Create(_item);
                    await _unitOfWork.SaveChangesAsync();
                    var returnItem = _mapper.Map<DepartmentDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the department");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the department - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/departments/5
        /// <summary>
        /// A method for updating a department record.
        /// </summary>
        /// <param name="model">The department record to be updated</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(DepartmentDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _unitOfWork.Departments.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The department of Id- '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var existingItem = await _unitOfWork.Departments.GetById(model.Id);
                    //Manual mapping
                    existingItem.Name = model.Name;
                    existingItem.Code = model.Code;
                    existingItem.StaffDetailsId = model.StaffDetailsId;
                    existingItem.Description = model.Description;
                    _unitOfWork.Departments.Update(existingItem);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the department.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the department.");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE api/departments/5
        /// <summary>
        /// A method for deleting the department record by Id.
        /// </summary>
        /// <param name="id">The department Id to be retrieved</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _unitOfWork.Departments.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The department of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _unitOfWork.Departments.GetById(id);
                _unitOfWork.Departments.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the department.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the department - " + ex.Message);
            }
        }
    }
}
