using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.API.Controllers.Staff;
using SchoolWebApp.Core.DTOs.Staff.StaffAttendance;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.Entities.Staff;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.DTOs.Students.StudentAttendance;
using SchoolWebApp.Core.Entities.Students;

namespace SchoolWebApp.API.Controllers.Students
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StudentAttendancesController : ControllerBase
    {
        private readonly ILogger<StudentAttendancesController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public StudentAttendancesController(ILogger<StudentAttendancesController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/studentAttendances
        /// <summary>
        /// A method for retrieving all student attendances
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentAttendanceDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(_mapper.Map<List<StudentAttendanceDto>>(await _unitOfWork.StudentAttendances.GetAll()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all student attendances list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/studentAttendances/paginated
        /// <summary>
        /// A method for retrieving a list of paginated student attendances
        /// </summary>
        /// <param name="pageNumber">The page number to return</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns></returns>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentAttendanceDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _unitOfWork.StudentAttendances.GetPaginatedData(pageNumber ?? 1, pageSize ?? 4);
                var mappedData = _mapper.Map<List<StudentAttendanceDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<StudentAttendanceDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated student attendances.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/studentAttendances/byStudentId/5
        /// <summary>
        /// A method for retrieving student attendances by student Id.
        /// </summary>
        /// <param name="id">The student attendance details Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("byStudentClassId/{studentId}/{classId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StaffAttendanceDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStudentAttendancesByStudentId(int studentId, int classId)
        {
            try
            {
                if (studentId <= 0) return BadRequest(studentId);
                if (classId <= 0) return BadRequest(classId);
                if(!await _unitOfWork.Students.ItemExistsAsync(s => s.Id == studentId))
                    return Conflict(new { message = $"The student details submitted do not exist." });
                if(!await _unitOfWork.SchoolClasses.ItemExistsAsync(s => s.Id ==classId))
                    return Conflict(new { message = $"The class details submitted do not exist." });
                var _item = await _unitOfWork.StudentAttendances.GetByStudentId(studentId,classId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<StaffAttendanceDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the student attendances by student id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/studentAttendances/5
        /// <summary>
        /// A method for retrieving of student attendances record by Id.
        /// </summary>
        /// <param name="id">The student attendances Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentAttendanceDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _unitOfWork.StudentAttendances.GetById(id);

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<StudentAttendanceDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the student attendances details by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/studentAttendances
        /// <summary>
        /// A method for creating a student attendance record.
        /// </summary>
        /// <param name="model">The student attendance record to be created</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentAttendanceDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateStudentAttendanceDto model)
        {
            if (ModelState.IsValid)
            {
                if (!await _unitOfWork.Students.ItemExistsAsync(s => s.Id == model.StudentClassId))
                    return Conflict(new { message = $"The student class details submitted do not exist." });
                if (await _unitOfWork.StudentAttendances.ItemExistsAsync(s => s.StudentClassId == model.StudentClassId && s.Date == model.Date))
                    return Conflict(new { message = $"The student attendance record already exists" });
                try
                {
                    var _item = _mapper.Map<StudentAttendance>(model);
                    _unitOfWork.StudentAttendances.Create(_item);
                    await _unitOfWork.SaveChangesAsync();
                    var returnItem = _mapper.Map<StudentAttendanceDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the student attendance");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the student attendance - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/studentAttendances/5
        /// <summary>
        /// A method for updating a student attendance record.
        /// </summary>
        /// <param name="model">The student attendance record to be updated</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(StudentAttendanceDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _unitOfWork.StudentAttendances.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The student attendance of Id- '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var existingItem = await _unitOfWork.StudentAttendances.GetById(model.Id);
                    //Manual mapping
                    existingItem.StudentClassId = model.StudentClassId;
                    existingItem.Date = model.Date;
                    existingItem.Present = model.Present;
                    existingItem.Remarks = model.Remarks;
                    _unitOfWork.StudentAttendances.Update(existingItem);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the student attendance.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the student attendance.");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE api/studentAttendances/5
        /// <summary>
        /// A method for deleting the student attendance record by Id.
        /// </summary>
        /// <param name="id">The student attendance Id to be retrieved</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _unitOfWork.StudentAttendances.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The student attendance of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _unitOfWork.StudentAttendances.GetById(id);
                _unitOfWork.StudentAttendances.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the student attendance.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the student attendance - " + ex.Message);
            }
        }
    }
}
