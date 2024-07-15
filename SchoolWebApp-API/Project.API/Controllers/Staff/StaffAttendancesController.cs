using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.Staff.StaffAttendance;
using SchoolWebApp.Core.Entities.Staff;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.API.Controllers.Staff
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StaffAttendancesController : ControllerBase
    {
        private readonly ILogger<StaffAttendancesController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public StaffAttendancesController(ILogger<StaffAttendancesController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/staffAttendances
        /// <summary>
        /// A method for retrieving all staff attendances
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StaffAttendanceDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(_mapper.Map<List<StaffAttendanceDto>>(await _unitOfWork.StaffAttendances.Find(includeProperties: "StaffDetails")));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all staff attendances list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/staffAttendances/paginated
        /// <summary>
        /// A method for retrieving a list of paginated staff attendances
        /// </summary>
        /// <param name="pageNumber">The page number to return</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns></returns>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StaffAttendanceDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _unitOfWork.StaffAttendances.GetPaginatedData(pageNumber ?? 1, pageSize ?? 4);
                var mappedData = _mapper.Map<List<StaffAttendanceDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<StaffAttendanceDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated staff attendances.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        // GET api/staffAttendances/byStaffDetailsId/5
        /// <summary>
        /// A method for retrieving staff attendances by staff details Id.
        /// </summary>
        /// <param name="id">The staff details Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("byStaffDetailsId/{staffDetailsId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StaffAttendanceDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStaffAttendancesByStaffDetaildId(int staffDetailsId)
        {
            try
            {
                if (staffDetailsId <= 0) return BadRequest(staffDetailsId);
                var _item = await _unitOfWork.StaffAttendances.GetByStaffDetailsId(staffDetailsId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<StaffAttendanceDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the staff attendances by staff details id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/staffAttendances/5
        /// <summary>
        /// A method for retrieving of staff attendances record by Id.
        /// </summary>
        /// <param name="id">The staff attendances Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StaffAttendanceDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _unitOfWork.StaffAttendances.GetById(id);

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<StaffAttendanceDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the staff attendances details by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/staffAttendances
        /// <summary>
        /// A method for creating a staff attendance record.
        /// </summary>
        /// <param name="model">The staff attendance record to be created</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StaffAttendanceDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateStaffAttendanceDto model)
        {
            if (ModelState.IsValid)
            {
                if (!await _unitOfWork.StaffDetails.ItemExistsAsync(s => s.Id == model.StaffDetailsId))
                    return Conflict(new { message = $"The staff details submitted do not exist." });
                if (await _unitOfWork.StaffAttendances.ItemExistsAsync(s => s.StaffDetailsId == model.StaffDetailsId && s.Date == model.Date))
                    return Conflict(new { message = $"The staff attendance record for the date already exists" });
                try
                {
                    var _item = _mapper.Map<StaffAttendance>(model);
                    _unitOfWork.StaffAttendances.Create(_item);
                    await _unitOfWork.SaveChangesAsync();
                    var returnItem = _mapper.Map<StaffAttendanceDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the staff attendance");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the staff attendance - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/staffAttendances/5
        /// <summary>
        /// A method for updating a staff attendance record.
        /// </summary>
        /// <param name="model">The staff attendance record to be updated</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(StaffAttendanceDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _unitOfWork.StaffAttendances.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The staff attendance of Id- '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var _item = _mapper.Map<StaffAttendance>(model);
                    _unitOfWork.StaffAttendances.Update(_item);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the staff attendance.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the staff attendance.");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE api/staffAttendances/5
        /// <summary>
        /// A method for deleting the staff attendance record by Id.
        /// </summary>
        /// <param name="id">The staff attendance Id to be retrieved</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _unitOfWork.StaffAttendances.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The staff attendance of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _unitOfWork.StaffAttendances.GetById(id);
                _unitOfWork.StaffAttendances.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the staff attendance.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the staff attendance - " + ex.Message);
            }
        }
    }
}
