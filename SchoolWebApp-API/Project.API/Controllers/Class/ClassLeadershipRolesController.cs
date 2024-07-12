using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs.Class.Session;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.DTOs.Class.ClassLeadershipRole;
using SchoolWebApp.Core.Entities.Class;

namespace SchoolWebApp.API.Controllers.Class
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ClassLeadershipRolesController : ControllerBase
    {
        private readonly ILogger<ClassLeadershipRolesController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ClassLeadershipRolesController(ILogger<ClassLeadershipRolesController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/classLeadershipRoles
        /// <summary>
        /// A method for retrieving all class leadership roles
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ClassLeadershipRoleDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var classLeadershipRoles = await _unitOfWork.ClassLeadershipRoles.GetAll();
                return Ok(_mapper.Map<List<ClassLeadershipRoleDto>>(classLeadershipRoles));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all class leadership roles list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/classLeadershipRoles/paginated
        /// <summary>
        /// A method for retrieving a list of paginated class leadership roles
        /// </summary>
        /// <param name="pageNumber">The page number to return</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns></returns>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ClassLeadershipRoleDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _unitOfWork.ClassLeadershipRoles.GetPaginatedData(pageNumber ?? 1, pageSize ?? 4);
                var mappedData = _mapper.Map<List<ClassLeadershipRoleDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<ClassLeadershipRoleDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated class leadership roles.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/classLeadershipRoles/5
        /// <summary>
        /// A method for retrieving a class leadership role record by Id.
        /// </summary>
        /// <param name="id">The class leadership role Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ClassLeadershipRoleDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _unitOfWork.ClassLeadershipRoles.GetById(id);

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<ClassLeadershipRoleDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the class leadership role details by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/classLeadershipRoles
        /// <summary>
        /// A method for creating a class leadership role record.
        /// </summary>
        /// <param name="model">The class leadership role record to be created</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ClassLeadershipRoleDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateClassLeadershipRoleDto model)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.ClassLeadershipRoles.ItemExistsAsync(s => s.Name == model.Name))
                    return Conflict(new { message = $"The class leadership role details submitted already exists" });
                try
                {
                    var _item = _mapper.Map<ClassLeadershipRole>(model);
                    _unitOfWork.ClassLeadershipRoles.Create(_item);
                    await _unitOfWork.SaveChangesAsync();
                    var returnItem = _mapper.Map<ClassLeadershipRoleDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the class leadership role.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the class leadership role - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/classLeadershipRoles/5
        /// <summary>
        /// A method for updating a class leadership role record.
        /// </summary>
        /// <param name="model">The class leadership role record to be updated</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(ClassLeadershipRoleDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _unitOfWork.ClassLeadershipRoles.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The class leadership role of Id - '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var _item = _mapper.Map<ClassLeadershipRole>(model);
                    _unitOfWork.ClassLeadershipRoles.Update(_item);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the class leadership role.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the class leadership role.");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE api/classLeadershipRoles/5
        /// <summary>
        /// A method for deleting the class leadership role record by Id.
        /// </summary>
        /// <param name="id">The class leadership role Id to be deleted</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _unitOfWork.ClassLeadershipRoles.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The class leadership role of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _unitOfWork.ClassLeadershipRoles.GetById(id);
                _unitOfWork.ClassLeadershipRoles.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the class leadership role.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the class leadership role - " + ex.Message);
            }
        }
    }
}
