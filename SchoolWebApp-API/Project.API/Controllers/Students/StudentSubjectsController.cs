using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.Students.StudentSubjects;
using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.Students;
using SchoolWebApp.Core.Interfaces.IRepositories;
using System.Security.Policy;

namespace SchoolWebApp.API.Controllers.Students
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentSubjectsController : ControllerBase
    {
        private readonly ILogger<StudentSubjectsController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public StudentSubjectsController(ILogger<StudentSubjectsController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET api/studentSubjects/checkIfExists/5
        /// <summary>
        /// A method for checking if student subjects exists by id
        /// </summary>
        /// <param name="studentSubjectId">The student subject id to be checked</param>
        /// <returns>The result if the student subject exists or not</returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("checkIfExists/{studentSubjectId}")]
        public async Task<IActionResult> CheckIfExists(int studentSubjectId)
        {
            try
            {
                return Ok(await _unitOfWork.StudentSubjects.ItemExistsAsync(s => s.Id == studentSubjectId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checiking if student subject exists.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/studentSubjects
        /// <summary>
        /// A method for retrieving all student subjects
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentSubjectDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(_mapper.Map<List<StudentSubjectDto>>(await _unitOfWork.StudentSubjects
                    .Find(includeProperties: "Subject,StudentClass")));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all student subjects list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/studentSubjects/paginated
        /// <summary>
        /// A method for retrieving a list of paginated student subjects
        /// </summary>
        /// <param name="pageNumber">The page number to return</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns></returns>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentSubjectDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _unitOfWork.StudentSubjects.GetPaginatedData(pageNumber ?? 1, pageSize ?? 4);
                var mappedData = _mapper.Map<List<StudentSubjectDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<StudentSubjectDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated student subjects.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        
        // GET api/studentSubjects/byStudentClassId/5
        /// <summary>
        /// A method for retrieving student subjects by student class Id.
        /// </summary>
        /// <param name="id">The student class Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("byStudentClassId/{studentClassId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentSubjectDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStudentSubjectsByStudentClassId(int studentClassId)
        {
            try
            {
                if (studentClassId <= 0) return BadRequest(studentClassId);
                var _item = await _unitOfWork.StudentSubjects.GetByStudentClassId(studentClassId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<StudentSubjectDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the student subjects by student class id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/studentSubjects/bySchoolClassSubjectId/5/5
        /// <summary>
        /// A method for retrieving student subjects by school class and subject Ids.
        /// </summary>
        /// <param name="schoolClassId">The school class Id whose records are to be retrieved</param>
        /// <param name="subjectId">The subject Id whose records are to be retrieved</param>
        /// <returns></returns>
        [HttpGet("bySchoolClassSubjectId/{schoolClassId}/{subjectId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentSubjectDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStudentSubjectsBySchoolClassSubjectId(int schoolClassId, int subjectId)
        {
            try
            {
                if (schoolClassId <= 0) return BadRequest(schoolClassId);
                var _item = await _unitOfWork.StudentSubjects.GetBySchoolClassSubjectId(schoolClassId, subjectId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<StudentSubjectDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the student subjects by school class and subject ids.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/studentSubjects/bySchoolClassId/5
        /// <summary>
        /// A method for retrieving student subjects by school class Id.
        /// </summary>
        /// <param name="id">The school class Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("bySchoolClassId/{schoolClassId}/{studentId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentSubjectDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStudentSubjectsBySchoolClassId(int schoolClassId, int studentId)
        {
            try
            {
                if (schoolClassId <= 0) return BadRequest(schoolClassId);
                if (studentId <= 0) return BadRequest(studentId);
                var _item = await _unitOfWork.StudentSubjects.GetBySchoolClassId(schoolClassId, studentId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<StudentSubjectDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the student subjects by school class id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/studentSubjects/byStudentId/5
        /// <summary>
        /// A method for retrieving student subjects by student Id.
        /// </summary>
        /// <param name="id">The student Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("byStudentId/{studentId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentSubjectDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStudentSubjectsByStudentId(int studentId)
        {
            try
            {
                if (studentId <= 0) return BadRequest(studentId);
                var _item = await _unitOfWork.StudentSubjects.GetByStudentId(studentId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<StudentSubjectDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the student subjects by student id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/studentSubjects/bySubjectId/5
        /// <summary>
        /// A method for retrieving student subjects by subject Id.
        /// </summary>
        /// <param name="id">The subject Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("bySubjectId/{subjectId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentSubjectDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStudentSubjectsBySubjectId(int subjectId)
        {
            try
            {
                if (subjectId <= 0) return BadRequest(subjectId);
                var _item = await _unitOfWork.StudentSubjects.GetBySubjectId(subjectId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<StudentSubjectDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the student subjects by subject id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/studentSubjects/5
        /// <summary>
        /// A method for retrieving of student subjects record by Id.
        /// </summary>
        /// <param name="id">The student subjects Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentSubjectDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _unitOfWork.StudentSubjects.GetById(id, includeProperties: "StudentClass,Subject");

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<StudentSubjectDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the student subjects details by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/studentSubjects
        /// <summary>
        /// A method for creating multiple student subjects records
        /// </summary>
        /// <param name="model">The list of student subjects</param>
        /// <returns></returns>
        [HttpPost("batch")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateMany(List<StudentSubjectDto> model)
        {
            if (model == null || !model.Any())
            {
                return BadRequest("No student subjects provided.");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    foreach (var item in model)
                    {
                        var existingStudentSubject = await _unitOfWork.StudentSubjects.GetByStudentClassSubjectId(item.StudentClassId, item.SubjectId);

                        if (existingStudentSubject != null)
                        {
                            existingStudentSubject.Description = item.Description;
                            _unitOfWork.StudentSubjects.Update(existingStudentSubject);
                        }
                        else
                        {
                            var _item = _mapper.Map<StudentSubject>(item);
                            _unitOfWork.StudentSubjects.Create(_item);
                        }
                    }
                    await _unitOfWork.SaveChangesAsync();
                    return Ok("Student subjects updated successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the student subjects.");
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
            return BadRequest(ModelState);
        }

        // POST api/studentSubjects
        /// <summary>
        /// A method for creating a student subject record.
        /// </summary>
        /// <param name="model">The student subject record to be created</param>
        /// <returns></returns>        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentSubjectDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateStudentSubjectDto model)
        {
            if (ModelState.IsValid)
            {
                if (!await _unitOfWork.Students.ItemExistsAsync(s => s.Id == model.StudentClassId))
                    return Conflict(new { message = $"The student class details submitted do not exist." });
                if (!await _unitOfWork.Subjects.ItemExistsAsync(s => s.Id == model.SubjectId))
                    return Conflict(new { message = $"The subject details submitted do not exist." });
                if (await _unitOfWork.StudentSubjects.ItemExistsAsync(s => s.StudentClassId == model.StudentClassId && s.SubjectId == model.SubjectId))
                    return Conflict(new { message = $"The student subject record for the class already exists" });
                try
                {
                    var _item = _mapper.Map<StudentSubject>(model);
                    _unitOfWork.StudentSubjects.Create(_item);
                    await _unitOfWork.SaveChangesAsync();
                    var returnItem = _mapper.Map<StudentSubjectDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the student subject");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the student subject - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/studentSubjects/5
        /// <summary>
        /// A method for updating a student subject record.
        /// </summary>
        /// <param name="model">The student subject record to be updated</param>
        /// <returns></returns>        
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(StudentSubjectDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _unitOfWork.StudentSubjects.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The student subject of Id- '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var _item = _mapper.Map<StudentSubject>(model);
                    _unitOfWork.StudentSubjects.Update(_item);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the student subject.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the student subject.");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE api/studentSubjects/5
        /// <summary>
        /// A method for deleting the student subject record by Id.
        /// </summary>
        /// <param name="id">The student subject Id to be retrieved</param>
        /// <returns></returns>        
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _unitOfWork.StudentSubjects.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The student subject of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _unitOfWork.StudentSubjects.GetById(id);
                _unitOfWork.StudentSubjects.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the student subject.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the student subject - " + ex.Message);
            }
        }
    }
}
