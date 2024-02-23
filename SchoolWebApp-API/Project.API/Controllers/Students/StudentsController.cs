using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.Academics.Grade;
using SchoolWebApp.Core.DTOs.Students.Student;
using SchoolWebApp.Core.DTOs.Students.Parent;
using SchoolWebApp.Core.Entities.Students;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.DTOs.Students.StudentParent;

namespace SchoolWebApp.API.Controllers.Students
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly ILogger<StudentsController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public StudentsController(ILogger<StudentsController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/students
        /// <summary>
        /// A method for retrieving all students details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(_mapper.Map<List<StudentDto>>(await _unitOfWork.Students.GetAll()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all studnets list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/students/paginated
        /// <summary>
        /// A method for retrieving a list of paginated students
        /// </summary>
        /// <param name="pageNumber">The page number to return</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns></returns>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _unitOfWork.Students.GetPaginatedData(pageNumber ?? 1, pageSize ?? 4);
                var mappedData = _mapper.Map<List<StudentDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<StudentDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated students list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/students/studentParents/5
        /// <summary>
        /// A method for retrieving parents for student Id.
        /// </summary>
        /// <param name="id">The student Id whose records to be retrieved</param>
        /// <returns></returns>
        [HttpGet("studentParents/{studentId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentParentDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetParentStudents(int studentId)
        {
            try
            {
                if (studentId <= 0) return BadRequest(studentId);
                var _item = await _unitOfWork.StudentParent.GetParentsByStudentId(studentId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<StudentParentDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the student's parents by student id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/students/byLearningModeId/5
        /// <summary>
        /// A method for retrieving students by learning mode Id.
        /// </summary>
        /// <param name="id">The learning mode Id whose students are to be retrieved</param>
        /// <returns></returns>
        [HttpGet("byLearningModeId/{learningModeId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStudentsByLearningModeId(int learningModeId)
        {
            try
            {
                if (learningModeId <= 0) return BadRequest(learningModeId);
                var _item = await _unitOfWork.Students.GetByLearningModeId(learningModeId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<StudentDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the studnets by learning mode id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/students/5
        /// <summary>
        /// A method for retrieving student record by Id.
        /// </summary>
        /// <param name="id">The student Id whose record is to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _unitOfWork.Students.GetById(id);

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<StudentDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the student record by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/students
        /// <summary>
        /// A method for creating a student record.
        /// </summary>
        /// <param name="model">The student record to be created</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateStudentDto model)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.Students.ItemExistsAsync(s => s.FullName == model.FullName && s.UPI == model.UPI && s.Status == model.Status))
                    return Conflict(new { message = $"The student record submitted already exists." });
                try
                {
                    var _item = _mapper.Map<Student>(model);
                    _unitOfWork.Students.Create(_item);
                    await _unitOfWork.SaveChangesAsync();
                    var returnItem = _mapper.Map<StudentDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the student details.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the student details - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/students/5
        /// <summary>
        /// A method for updating a student record.
        /// </summary>
        /// <param name="model">The student record to be updated</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(StudentDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _unitOfWork.Students.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The student record of Id - '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var existingItem = await _unitOfWork.Students.GetById(model.Id);
                    //Manual mapping
                    existingItem.AdmissionDate = model.AdmissionDate;
                    existingItem.ApplicationDate = model.ApplicationDate;
                    existingItem.HealthConcerns = model.HealthConcerns;
                    existingItem.LearningModeId = model.LearningModeId;
                    existingItem.Status = model.Status;
                    existingItem.FullName = model.FullName;
                    existingItem.UPI = model.UPI;
                    existingItem.DateOfBirth = model.DateOfBirth;
                    existingItem.Address = model.Address;
                    existingItem.PhoneNumber = model.PhoneNumber;
                    existingItem.Email = model.Email;
                    existingItem.NationalityId = model.NationalityId;
                    existingItem.ReligionId = model.ReligionId;
                    existingItem.GenderId = model.GenderId;
                    _unitOfWork.Students.Update(existingItem);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the student record.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the student record.");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE api/students/5
        /// <summary>
        /// A method for deleting the student record by Id.
        /// </summary>
        /// <param name="id">The student Id to be deleted</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _unitOfWork.Students.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The student of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _unitOfWork.Students.GetById(id);
                _unitOfWork.Students.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the student record.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the student record - " + ex.Message);
            }
        }
    }
}
