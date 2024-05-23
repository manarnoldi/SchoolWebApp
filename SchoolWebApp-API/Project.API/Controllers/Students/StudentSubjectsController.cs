using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.Students.StudentSubjects;
using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.Students;
using SchoolWebApp.Core.Interfaces.IRepositories;

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
                return Ok(_mapper.Map<List<StudentSubjectDto>>(await _unitOfWork.StudentSubjects.GetAll()));
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


        // GET api/studentSubjects/byAcademicYearId/5
        /// <summary>
        /// A method for retrieving student subjects by academic year Id.
        /// </summary>
        /// <param name="id">The academic year Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("byAcademicYearId/{academicYearId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentSubjectDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStudentSubjectsByAcademicYearId(int academicYearId)
        {
            try
            {
                if (academicYearId <= 0) return BadRequest(academicYearId);
                var _item = await _unitOfWork.StudentSubjects.GetByAcademicYearId(academicYearId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<StudentSubjectDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the student subjects by academic year id.");
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentSubjectDto))]
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
                var _item = await _unitOfWork.StudentSubjects.GetById(id);

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
        /// A method for creating a student subject record.
        /// </summary>
        /// <param name="model">The student subject record to be created</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentSubjectDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateStudentSubjectDto model)
        {
            if (ModelState.IsValid)
            {
                if (!await _unitOfWork.Students.ItemExistsAsync(s => s.Id == model.StudentId))
                    return Conflict(new { message = $"The student details submitted do not exist." });
                if (!await _unitOfWork.Subjects.ItemExistsAsync(s => s.Id == model.SubjectId))
                    return Conflict(new { message = $"The subject details submitted do not exist." });
                if (!await _unitOfWork.AcademicYears.ItemExistsAsync(s => s.Id == model.AcademicYearId))
                    return Conflict(new { message = $"The academic year details submitted do not exist." });
                if (await _unitOfWork.StudentSubjects.ItemExistsAsync(s => s.StudentId == model.StudentId && s.SubjectId == model.SubjectId &&
                s.AcademicYearId == model.AcademicYearId))
                    return Conflict(new { message = $"The student subject record already exists" });
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
        [Authorize(Policy = "AdminRole")]
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
                    var existingItem = await _unitOfWork.StudentSubjects.GetById(model.Id);
                    //Manual mapping
                    existingItem.StudentId = model.StudentId;
                    existingItem.SubjectId = model.SubjectId;
                    existingItem.AcademicYearId = model.AcademicYearId;
                    _unitOfWork.StudentSubjects.Update(existingItem);
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
        [Authorize(Policy = "AdminRole")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
