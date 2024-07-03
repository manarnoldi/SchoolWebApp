using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.School.Event;
using SchoolWebApp.Core.Entities.School;
using SchoolWebApp.Core.Entities.Staff;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.API.Controllers.School
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly ILogger<EventsController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public EventsController(ILogger<EventsController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/events
        /// <summary>
        /// A method for retrieving all events
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<EventDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(_mapper.Map<List<EventDto>>(await _unitOfWork.Events.GetAll()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all events list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/events/paginated
        /// <summary>
        /// A method for retrieving a list of paginated events
        /// </summary>
        /// <param name="pageNumber">The page number to return</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns></returns>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<EventDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _unitOfWork.Events.GetPaginatedData(pageNumber ?? 1, pageSize ?? 4);
                var mappedData = _mapper.Map<List<EventDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<EventDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated events.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/events/bySessionId/5
        /// <summary>
        /// A method for retrieving an events by session Id.
        /// </summary>
        /// <param name="id">The event Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("bySessionId/{sessionId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EventDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEventsBySessionId(int sessionId)
        {
            try
            {
                if (sessionId <= 0) return BadRequest(sessionId);
                var _item = await _unitOfWork.Events.GetBySessionId(sessionId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<EventDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the events by session id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/events/5
        /// <summary>
        /// A method for retrieving an event record by Id.
        /// </summary>
        /// <param name="id">The event Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EventDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _unitOfWork.Events.GetById(id);

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<EventDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the event details by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/events
        /// <summary>
        /// A method for creating an event record.
        /// </summary>
        /// <param name="model">The event record to be created</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EventDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateEventDto model)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.Events.ItemExistsAsync(s => s.EventName == model.EventName && s.SessionId == model.SessionId
                && s.EventLocation == model.EventLocation && s.StartDate == model.StartDate && s.Status == model.Status && s.Description == model.Description
                && s.EndDate == model.EndDate && s.EventYear == model.EventYear))
                    return Conflict(new { message = $"The event details submitted already exist." });
                try
                {
                    var _item = _mapper.Map<Event>(model);
                    _unitOfWork.Events.Create(_item);
                    await _unitOfWork.SaveChangesAsync();
                    var returnItem = _mapper.Map<EventDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the event.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the event - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/events/5
        /// <summary>
        /// A method for updating an event record.
        /// </summary>
        /// <param name="model">The event record to be updated</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(EventDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _unitOfWork.Events.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The event of Id - '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var _item = _mapper.Map<Event>(model);
                    _unitOfWork.Events.Update(_item);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the event.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the event.");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE api/events/5
        /// <summary>
        /// A method for deleting the event record by Id.
        /// </summary>
        /// <param name="id">The event Id to be retrieved</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _unitOfWork.Events.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The event of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _unitOfWork.Events.GetById(id);
                _unitOfWork.Events.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the event.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the event - " + ex.Message);
            }
        }
    }
}
