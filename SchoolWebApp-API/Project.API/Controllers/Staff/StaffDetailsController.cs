using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.API.Utils;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.Academics.Exam;
using SchoolWebApp.Core.DTOs.Class.Session;
using SchoolWebApp.Core.DTOs.Staff.StaffDetails;
using SchoolWebApp.Core.Entities.Enums;
using SchoolWebApp.Core.Entities.Settings;
using SchoolWebApp.Core.Entities.Staff;
using SchoolWebApp.Core.Interfaces.IRepositories;
using System;
using System.Linq.Expressions;

namespace SchoolWebApp.API.Controllers.Staff
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StaffDetailsController : ControllerBase
    {
        private readonly ILogger<StaffDetailsController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public StaffDetailsController(ILogger<StaffDetailsController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/staffDetails?active=true
        /// <summary>
        /// A method for retrieving all staff details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StaffDetailDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(bool active, int? staffCategoryId, int? employmentTypeId)
        {
            try
            {
                var staffs = await _unitOfWork.StaffDetails
                    .SearchForStaff(staffCategoryId, employmentTypeId, active ? Status.Active : null);
                return Ok(_mapper.Map<List<StaffDetailDto>>(staffs));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all staff details list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/staffDetails/GetCount
        /// <summary>
        /// A method for getting the total number of staff per category and employment type in the school
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetCount")]
        public async Task<ActionResult<int>> GetCount(bool active, int? staffCategoryId, int? employmentTypeId)
        {
            Expression<Func<StaffDetails, bool>> query = s => true; // Default to always true

            if (active)
            {
                query = Utility.CombineExpressions(query, s => s.Status == Status.Active);
            }

            if (staffCategoryId != null && staffCategoryId > 0)
            {
                query = Utility.CombineExpressions(query, s => s.StaffCategoryId == staffCategoryId);
            }

            if (employmentTypeId != null && employmentTypeId > 0)
            {
                query = Utility.CombineExpressions(query, s => s.EmploymentTypeId == employmentTypeId);
            }

            var returnStaffReq = _unitOfWork.StaffDetails.RecordCount(query);
            return Ok(await returnStaffReq);
        }


        // GET: api/staffDetails/paginated
        /// <summary>
        /// A method for retrieving a list of paginated staff details
        /// </summary>
        /// <param name="pageNumber">The page number to return</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns></returns>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StaffDetailDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _unitOfWork.StaffDetails.GetPaginatedData(pageNumber ?? 1, pageSize ?? 4);
                var mappedData = _mapper.Map<List<StaffDetailDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<StaffDetailDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated staff details.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/staffDetails/byEmploymentTypeId/5
        /// <summary>
        /// A method for retrieving an staff details by employment type Id.
        /// </summary>
        /// <param name="id">The employment type Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("byEmploymentTypeId/{employmentTypeId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StaffDetailDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStaffDetailsByEmploymentTypeId(int employmentTypeId)
        {
            try
            {
                if (employmentTypeId <= 0) return BadRequest(employmentTypeId);
                var _item = await _unitOfWork.StaffDetails.GetByEmploymentTypeId(employmentTypeId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<StaffDetailDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the staff details by employement type id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/staffDetails/byStaffCategoryId/5
        /// <summary>
        /// A method for retrieving an staff details by staff category Id type Id.
        /// </summary>
        /// <param name="id">The staff category Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("byStaffCategoryId/{staffCategoryId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StaffDetailDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStaffDetailsByStaffCategoryId(int staffCategoryId)
        {
            try
            {
                if (staffCategoryId <= 0) return BadRequest(staffCategoryId);
                var _item = await _unitOfWork.StaffDetails.GetByStaffCategoryId(staffCategoryId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<StaffDetailDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the staff details by employment type id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/staffDetails/5
        /// <summary>
        /// A method for retrieving staff details record by Id.
        /// </summary>
        /// <param name="id">The staff details Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StaffDetailDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _unitOfWork.StaffDetails.GetById(id, includeProperties: "StaffCategory,Designation,Nationality,Religion,Gender,EmploymentType");

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<StaffDetailDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the staff details by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/staffDetails/staffSearch?staffCategoryId=5&employmentTypeId=5
        /// <summary>
        /// A method for retrieving staff details by searching.
        /// </summary>
        /// <param name="staffCategoryId">The staff category Id whose staffs is to be retrieved</param>
        /// <param name="employmentTypeId">The employment type Id whose staffs is to be retrieved</param>
        /// <returns></returns>
        [HttpGet("staffSearch")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExamDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> StaffSearch(Status? status, int? staffCategoryId = null, int? employmentTypeId = null)
        {
            try
            {
                var _item = await _unitOfWork.StaffDetails.SearchForStaff(staffCategoryId, employmentTypeId, status);
                var _itemDto = _mapper.Map<List<StaffDetailDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the staff by search items.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/staffDetails
        /// <summary>
        /// A method for creating an staff details record.
        /// </summary>
        /// <param name="model">The staff details record to be created</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StaffDetailDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateStaffDetailDto model)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.Students.ItemExistsAsync(s => s.UPI == model.UPI))
                    return Conflict(new { message = $"The provided UPI (Unique Personal Identifier) provided already exists in the system." });
                if (await _unitOfWork.StaffDetails.ItemExistsAsync(s => s.FullName == model.FullName && s.UPI == model.UPI && s.Status == model.Status))
                    return Conflict(new { message = $"The staff details submitted already exist." });
                try
                {
                    var _item = _mapper.Map<StaffDetails>(model);
                    _unitOfWork.StaffDetails.Create(_item);
                    await _unitOfWork.SaveChangesAsync();
                    var returnItem = _mapper.Map<StaffDetailDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the staff details.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the staff details - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/staffDetails/5
        /// <summary>
        /// A method for updating an staff details record.
        /// </summary>
        /// <param name="model">The staff details record to be updated</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(StaffDetailDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _unitOfWork.StaffDetails.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The staff details record of Id - '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var _item = _mapper.Map<StaffDetails>(model);
                    _unitOfWork.StaffDetails.Update(_item);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the staff details.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the staff details.");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE api/staffDetails/5
        /// <summary>
        /// A method for deleting the staff details record by Id.
        /// </summary>
        /// <param name="id">The staff details Id to be deleted</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _unitOfWork.StaffDetails.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The staff details of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _unitOfWork.StaffDetails.GetById(id);
                _unitOfWork.StaffDetails.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the staff details.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the staff details - " + ex.Message);
            }
        }
    }
}
