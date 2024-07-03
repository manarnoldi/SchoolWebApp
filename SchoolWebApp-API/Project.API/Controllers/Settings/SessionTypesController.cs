using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.Settings.SessionType;
using SchoolWebApp.Core.Entities.Settings;
using SchoolWebApp.Core.Entities.Staff;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.API.Controllers.Settings
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SessionTypesController : ControllerBase
    {
        private readonly ILogger<SessionTypesController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public SessionTypesController(ILogger<SessionTypesController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/sessionTypes
        /// <summary>
        /// A method for retrieving all session types
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SessionTypeDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(_mapper.Map<List<SessionTypeDto>>(await _unitOfWork.SessionTypes.GetAll()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all session types list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/sessionTypes/paginated
        /// <summary>
        /// A method for retrieving a list of paginated session types
        /// </summary>
        /// <param name="pageNumber">The page number to return</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns></returns>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SessionTypeDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _unitOfWork.SessionTypes.GetPaginatedData(pageNumber ?? 1, pageSize ?? 4);
                var mappedData = _mapper.Map<List<SessionTypeDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<SessionTypeDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated session types.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/sessionTypes/5
        /// <summary>
        /// A method for retrieving of session types record by Id.
        /// </summary>
        /// <param name="id">The session types Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SessionTypeDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _unitOfWork.SessionTypes.GetById(id);

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<SessionTypeDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the session types details by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/sessionType
        /// <summary>
        /// A method for creating a session type record.
        /// </summary>
        /// <param name="model">The session type record to be created</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SessionTypeDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateSessionTypeDto model)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.SessionTypes.IsExists("Name", model.Name))
                    return Conflict(new { message = $"The session type name - '{model.Name}' already exists" });
                try
                {
                    var _item = _mapper.Map<SessionType>(model);
                    _unitOfWork.SessionTypes.Create(_item);
                    await _unitOfWork.SaveChangesAsync();
                    var returnItem = _mapper.Map<SessionTypeDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the session type");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the session type - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/sessionTypes/5
        /// <summary>
        /// A method for updating a session type record.
        /// </summary>
        /// <param name="model">The session type record to be updated</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(SessionTypeDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _unitOfWork.SessionTypes.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The session type of Id- '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var _item = _mapper.Map<SessionType>(model);
                    _unitOfWork.SessionTypes.Update(_item);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the session type.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the session type.");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE api/sessionTypes/5
        /// <summary>
        /// A method for deleting the session type record by Id.
        /// </summary>
        /// <param name="id">The session type Id to be retrieved</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _unitOfWork.SessionTypes.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The session type of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _unitOfWork.SessionTypes.GetById(id);
                _unitOfWork.SessionTypes.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the session type.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the session type - " + ex.Message);
            }
        }
    }
}
