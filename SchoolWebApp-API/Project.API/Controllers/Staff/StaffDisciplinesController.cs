using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.Staff.StaffDiscipline;
using SchoolWebApp.Core.Entities.Staff;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.API.Controllers.Staff
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StaffDisciplinesController : ControllerBase
    {
        private readonly ILogger<StaffDisciplinesController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public StaffDisciplinesController(ILogger<StaffDisciplinesController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/staffDisciplines
        /// <summary>
        /// A method for retrieving all staff disciplines
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StaffDisciplineDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(_mapper.Map<List<StaffDisciplineDto>>(await _unitOfWork.StaffDisciplines.GetAll()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all staff disciplines list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/staffDisciplines/paginated
        /// <summary>
        /// A method for retrieving a list of paginated staff disciplines
        /// </summary>
        /// <param name="pageNumber">The page number to return</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns></returns>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StaffDisciplineDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _unitOfWork.StaffDisciplines.GetPaginatedData(pageNumber ?? 1, pageSize ?? 4);
                var mappedData = _mapper.Map<List<StaffDisciplineDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<StaffDisciplineDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated staff disciplines.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        // GET api/staffDisciplines/byStaffDetailsId/5
        /// <summary>
        /// A method for retrieving staff disciplines by staff details Id.
        /// </summary>
        /// <param name="id">The staff details Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("byStaffDetailsId/{staffDetailsId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StaffDisciplineDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStaffDisciplinesByStaffDetaildId(int staffDetailsId)
        {
            try
            {
                if (staffDetailsId <= 0) return BadRequest(staffDetailsId);
                var _item = await _unitOfWork.StaffDisciplines.GetByStaffDetailsId(staffDetailsId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<StaffDisciplineDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the staff disciplines by staff details id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/staffDisciplines/5
        /// <summary>
        /// A method for retrieving of staff disciplines record by Id.
        /// </summary>
        /// <param name="id">The staff disciplines Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StaffDisciplineDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _unitOfWork.StaffDisciplines.GetById(id);

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<StaffDisciplineDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the staff disciplines details by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/staffDisciplines
        /// <summary>
        /// A method for creating a staff discipline record.
        /// </summary>
        /// <param name="model">The staff discipline record to be created</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StaffDisciplineDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateStaffDisciplineDto model)
        {
            if (ModelState.IsValid)
            {
                if (!await _unitOfWork.StaffDetails.ItemExistsAsync(s => s.Id == model.StaffDetailsId))
                    return Conflict(new { message = $"The staff details submitted do not exist." });
                if (await _unitOfWork.StaffDisciplines.ItemExistsAsync(s => s.StaffDetailsId == model.StaffDetailsId && s.OccurenceDetails == model.OccurenceDetails &&
                s.OccurenceStartDate==model.OccurenceStartDate && s.OccurenceEndDate == model.OccurenceEndDate && s.OccurenceTypeId == model.OccurenceTypeId))
                    return Conflict(new { message = $"The staff discipline record already exists" });
                try
                {
                    var _item = _mapper.Map<StaffDiscipline>(model);
                    _unitOfWork.StaffDisciplines.Create(_item);
                    await _unitOfWork.SaveChangesAsync();
                    var returnItem = _mapper.Map<StaffDisciplineDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the staff discipline");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the staff discipline - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/staffDisciplines/5
        /// <summary>
        /// A method for updating a staff discipline record.
        /// </summary>
        /// <param name="model">The staff discipline record to be updated</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(StaffDisciplineDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _unitOfWork.StaffDisciplines.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The staff discipline of Id- '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var existingItem = await _unitOfWork.StaffDisciplines.GetById(model.Id);
                    //Manual mapping
                    existingItem.StaffDetailsId = model.StaffDetailsId;
                    existingItem.OccurenceTypeId = model.OccurenceTypeId;
                    existingItem.OccurenceDetails = model.OccurenceDetails;
                    existingItem.OccurenceStartDate = model.OccurenceStartDate;
                    existingItem.OccurenceEndDate = model.OccurenceEndDate;
                    existingItem.OutcomeId = model.OutcomeId;
                    _unitOfWork.StaffDisciplines.Update(existingItem);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the staff discipline.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the staff discipline.");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE api/staffDisciplines/5
        /// <summary>
        /// A method for deleting the staff discipline record by Id.
        /// </summary>
        /// <param name="id">The staff discipline Id to be retrieved</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _unitOfWork.StaffDisciplines.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The staff discipline of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _unitOfWork.StaffDisciplines.GetById(id);
                _unitOfWork.StaffDisciplines.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the staff discipline.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the staff discipline - " + ex.Message);
            }
        }
    }
}
