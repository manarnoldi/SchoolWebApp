using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.Academics.SubjectGroup;
using SchoolWebApp.Core.DTOs.Class.Session;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.API.Controllers.Class
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SessionsController : ControllerBase
    {
        private readonly ILogger<SessionsController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public SessionsController(ILogger<SessionsController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/sessions
        /// <summary>
        /// A method for retrieving all sessions
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SessionDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var sessions = await _unitOfWork.Sessions.Find(includeProperties: "AcademicYear,Curriculum,SessionType");
                return Ok(_mapper.Map<List<SessionDto>>(sessions));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all sessions list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/sessions/paginated
        /// <summary>
        /// A method for retrieving a list of paginated sessions
        /// </summary>
        /// <param name="pageNumber">The page number to return</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns></returns>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SessionDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _unitOfWork.Sessions.GetPaginatedData(pageNumber ?? 1, pageSize ?? 4);
                var mappedData = _mapper.Map<List<SessionDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<SessionDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated sessions.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/sessions/byCurriculumId/5
        /// <summary>
        /// A method for retrieving sessions by curriculum Id.
        /// </summary>
        /// <param name="id">The curriculum Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("byCurriculumId/{curriculumId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SessionDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSessionsByCurriculumId(int curriculumId)
        {
            try
            {
                if (curriculumId <= 0) return BadRequest(curriculumId);
                var _item = await _unitOfWork.Sessions.GetByCurriculumId(curriculumId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<SessionDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the sessions by curriculum id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/sessions/5
        /// <summary>
        /// A method for retrieving a session record by Id.
        /// </summary>
        /// <param name="id">The session Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SessionDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _unitOfWork.Sessions.GetById(id);

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<SessionDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the session details by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/sessions
        /// <summary>
        /// A method for creating a session record.
        /// </summary>
        /// <param name="model">The session record to be created</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SessionDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateSessionDto model)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.Sessions.ItemExistsAsync(s => s.SessionName == model.SessionName && s.SessionTypeId == model.SessionTypeId
                && s.CurriculumId == model.CurriculumId && s.AcademicYearId == model.AcademicYearId && s.Status == model.Status))
                    return Conflict(new { message = $"The session details submitted already exists" });
                try
                {
                    var _item = _mapper.Map<Session>(model);
                    _unitOfWork.Sessions.Create(_item);
                    await _unitOfWork.SaveChangesAsync();
                    var returnItem = _mapper.Map<SessionDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the session.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the session - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/sessions/5
        /// <summary>
        /// A method for updating a session record.
        /// </summary>
        /// <param name="model">The session record to be updated</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(SessionDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _unitOfWork.Sessions.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The session of Id - '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var existingItem = await _unitOfWork.Sessions.GetById(model.Id);
                    //Manual mapping
                    existingItem.SessionName = model.SessionName;
                    existingItem.Abbreviation = model.Abbreviation;
                    existingItem.StartDate = model.StartDate;
                    existingItem.EndDate = model.EndDate;
                    existingItem.Status = model.Status;
                    existingItem.AcademicYearId = model.AcademicYearId;
                    existingItem.CurriculumId = model.CurriculumId;
                    existingItem.SessionTypeId = model.SessionTypeId;
                    _unitOfWork.Sessions.Update(existingItem);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the session.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the session.");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE api/sessions/5
        /// <summary>
        /// A method for deleting the session record by Id.
        /// </summary>
        /// <param name="id">The session Id to be deleted</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _unitOfWork.Sessions.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The session of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _unitOfWork.Sessions.GetById(id);
                _unitOfWork.Sessions.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the session.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the session - " + ex.Message);
            }
        }
    }
}
