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
using SchoolWebApp.Core.Entities.Staff;

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
                return Ok(_mapper.Map<List<StudentDto>>(await _unitOfWork.Students.Find(includeProperties: "LearningMode,Nationality,Religion,Gender")));
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
        /// A method for retrieving parents for a student Id.
        /// </summary>
        /// <param name="id">The student Id whose records to be retrieved</param>
        /// <returns></returns>
        [HttpGet("studentParents/{studentId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentParentDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetParentsByStudentId(int studentId)
        {
            try
            {
                if (studentId <= 0) return BadRequest(studentId);
                var _item = await _unitOfWork.Students.GetParentsByStudentId(studentId);
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

        // GET api/students/studentParent/5/5
        /// <summary>
        /// A method for retrieving parent for a student by Ids.
        /// </summary>
        /// <param name="id">The student Id and parent Id whose record needs to be retrieved</param>
        /// <returns></returns>
        [HttpGet("studentParents/{studentId}/{parentId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentParentDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStudentParentByIds(int studentId, int parentId)
        {
            try
            {
                if (studentId <= 0) return BadRequest(studentId);
                if (studentId <= 0) return BadRequest(parentId);
                var _item = await _unitOfWork.StudentParent.GetStudentParentByIds(studentId,parentId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<StudentParentDto>(_item);
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
                var _item = await _unitOfWork.Students.GetById(id, includeProperties: "LearningMode,Nationality,Religion,Gender");

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
                if (await _unitOfWork.Students.ItemExistsAsync(s => s.UPI == model.UPI))
                    return Conflict(new { message = $"The provided UPI (Unique Personal Identifier) provided already exists in the system." });
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
                    var _item = _mapper.Map<Student>(model);
                    _unitOfWork.Students.Update(_item);
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

        // POST api/students/studentParent
        /// <summary>
        /// A method for creating a student-parent record.
        /// </summary>
        /// <param name="model">The student-parent record to be created</param>
        /// <returns></returns>
        [HttpPost("studentParent")]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentParentDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddStudentParent(CreateStudentParentDto model)
        {
            if (ModelState.IsValid)
            {
                if (!await _unitOfWork.Relationships.ItemExistsAsync(r => r.Id == model.RelationShipId))
                    return BadRequest("An error occurred while adding the student-parent. The relationship does not exist in the database or has been deleted.");
                if (!await _unitOfWork.Students.ItemExistsAsync(s => s.Id == model.StudentId))
                    return BadRequest("An error occurred while adding the student-parent. The student does not exist in the database or has been deleted.");
                if (!await _unitOfWork.Parents.ItemExistsAsync(s => s.Id == model.ParentId))
                    return BadRequest("An error occurred while adding the student-parent. The parent does not exist in the database or has been deleted.");
                if (await _unitOfWork.StudentParent.ItemExistsAsync(s => s.StudentId == model.StudentId && s.ParentId == model.ParentId
                && s.RelationShipId == model.RelationShipId))
                    return Conflict(new { message = $"The student-parent details submitted already exists" });
                try
                {
                    var _item = _mapper.Map<StudentParent>(model);
                    _unitOfWork.StudentParent.Create(_item);
                    await _unitOfWork.SaveChangesAsync();
                    var returnItem = _mapper.Map<StudentParentDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the student-parent.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the student-parent - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/studentParent/5
        /// <summary>
        /// A method for updating a student-parent record.
        /// </summary>
        /// <param name="model">The student-parent record to be updated</param>
        /// <returns></returns>
        [HttpPut("studentParent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> EditStudentParent(StudentParentDto model)
        {
            if (ModelState.IsValid)
            {
                if (!await _unitOfWork.Relationships.ItemExistsAsync(r => r.Id == model.RelationShipId))
                    return BadRequest("An error occurred while adding the student-parent. The relationship does not exist in the database or has been deleted.");
                if (!await _unitOfWork.Students.ItemExistsAsync(s => s.Id == model.StudentId))
                    return BadRequest("An error occurred while adding the student-parent. The student does not exist in the database or has been deleted.");
                if (!await _unitOfWork.Parents.ItemExistsAsync(s => s.Id == model.ParentId))
                    return BadRequest("An error occurred while adding the student-parent. The parent does not exist in the database or has been deleted.");
                var itemExist = await _unitOfWork.StudentParent.ItemExistsAsync(m => m.ParentId == model.ParentId && m.StudentId == model.StudentId);
                if (!itemExist)
                    return BadRequest($"The student-parent of student Id - '{model.StudentId}' and parent Id - '{model.ParentId}' does not exist hence cannot be updated.");
                try
                {
                    var _item = _mapper.Map<StudentParent>(model);
                    _unitOfWork.StudentParent.Update(_item);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the student-parent.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the student-parent.");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE api/students/studentParent/5/5
        /// <summary>
        /// A method for deleting the student-parent record by Id.
        /// </summary>
        /// <param name="id">The student-parent Id to be deleted</param>
        /// <returns></returns>
        [HttpDelete("studentParent/{parentId}/{studentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteStudentParent(int parentId, int studentId)
        {
            try
            {
                var itemExists = await _unitOfWork.StudentParent.GetStudentParentByIds(parentId, studentId);
                if (itemExists == null)
                    return BadRequest($"The student-parent of parent Id - '{parentId}' and student Id '{studentId}' does not exist hence cannot be deleted.");
                var entity = await _unitOfWork.StudentParent.GetStudentParentByIds(parentId, studentId);
                _unitOfWork.StudentParent.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the student-parent.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the student-parent - " + ex.Message);
            }
        }


    }
}
