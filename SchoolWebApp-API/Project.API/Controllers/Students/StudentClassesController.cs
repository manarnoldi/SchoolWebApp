using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.API.Utils;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.Staff.StaffSubject;
using SchoolWebApp.Core.DTOs.Students.FormerSchool;
using SchoolWebApp.Core.DTOs.Students.StudentClass;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.Enums;
using SchoolWebApp.Core.Entities.Students;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.API.Controllers.Students
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StudentClassesController : ControllerBase
    {
        private readonly ILogger<StudentClassesController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public StudentClassesController(ILogger<StudentClassesController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/studentClasses
        /// <summary>
        /// A method for retrieving all student classes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentClassDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(_mapper.Map<List<StudentClassDto>>(await _unitOfWork.StudentClasses.Find(includeProperties: "Student,SchoolClass")));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all student classes list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/studentClasses/paginated
        /// <summary>
        /// A method for retrieving a list of paginated student classes
        /// </summary>
        /// <param name="pageNumber">The page number to return</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns></returns>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentClassDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _unitOfWork.StudentClasses.GetPaginatedData(pageNumber ?? 1, pageSize ?? 4);
                var mappedData = _mapper.Map<List<StudentClassDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<StudentClassDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated student classes.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/studentClasses/byStudentId/5
        /// <summary>
        /// A method for retrieving student classes by student Id.
        /// </summary>
        /// <param name="id">The student Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("byStudentId/{studentId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentClassDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByStudentId(int studentId)
        {
            try
            {
                if (studentId <= 0) return BadRequest(studentId);
                var _item = await _unitOfWork.StudentClasses.GetByStudentId(studentId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<StudentClassDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the former schools by student id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/studentClasses/bySchoolClassId/5
        /// <summary>
        /// A method for retrieving student classes by school class Id.
        /// </summary>
        /// <param name="schoolClassId">The school class Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("bySchoolClassId/{schoolClassId}/{status}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentClassDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBySchoolClassId(int schoolClassId, Status status = Status.Active)
        {
            try
            {
                if (schoolClassId <= 0) return BadRequest(schoolClassId);
                var _item = await _unitOfWork.StudentClasses.GetBySchoolClassId(schoolClassId, status);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<StudentClassDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the student classes by school class id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/studentClasses/byStudentYearId/5/5
        /// <summary>
        /// A method for retrieving student classes by student id and year id.
        /// </summary>
        /// <param name="studentId">The student Id whose student classes will be retrieved</param>
        /// <param name="yearId">The year Id whose student classes will be retrieved</param>
        /// <returns></returns>
        [HttpGet("byStudentYearId/{studentId}/{yearId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentClassDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByStudentYearId(int studentId, int yearId)
        {
            try
            {
                if (studentId <= 0) return BadRequest(studentId);
                if (yearId <= 0) return BadRequest(yearId);
                var _item = await _unitOfWork.StudentClasses.GetByStudentYearId(studentId, yearId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<StudentClassDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the student classes by student id and year id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/studentClasses/5
        /// <summary>
        /// A method for retrieving a student class record by Id.
        /// </summary>
        /// <param name="id">The student class Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentClassDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _unitOfWork.StudentClasses.GetById(id, includeProperties: "Student,SchoolClass");

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<StudentClassDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the student class details by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/studentClasses
        /// <summary>
        /// A method for creating a student class record.
        /// </summary>
        /// <param name="model">The student class record to be created</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentClassDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateStudentClassDto model)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.StudentClasses.CheckIfStudentAssignedForYear(model.SchoolClassId, model.StudentId))
                    return Conflict(new { message = $"The student is already assigned to a class in the year." });
                if (await _unitOfWork.StudentClasses.ItemExistsAsync(s => s.StudentId == model.StudentId && s.SchoolClassId == model.SchoolClassId))
                    return Conflict(new { message = $"The student class details submitted already exist." });
                try
                {
                    var _item = _mapper.Map<StudentClass>(model);
                    _unitOfWork.StudentClasses.Create(_item);
                    await _unitOfWork.SaveChangesAsync();
                    var returnItem = _mapper.Map<StudentClassDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the student class.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the student class - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/studentClasses/5
        /// <summary>
        /// A method for updating a student class record.
        /// </summary>
        /// <param name="model">The student class record to be updated</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(StudentClassDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _unitOfWork.StudentClasses.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The student class of Id - '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var _item = _mapper.Map<StudentClass>(model);
                    _unitOfWork.StudentClasses.Update(_item);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the student class.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the student class.");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE api/studentClasses/5
        /// <summary>
        /// A method for deleting the student class record by Id.
        /// </summary>
        /// <param name="id">The student class Id to be retrieved</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _unitOfWork.StudentClasses.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The student class of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _unitOfWork.StudentClasses.GetById(id);
                _unitOfWork.StudentClasses.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the student class.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the student class - " +
                    HandleExceptions.GetMessageForInnerExceptions(ex));
            }
        }
    }
}
