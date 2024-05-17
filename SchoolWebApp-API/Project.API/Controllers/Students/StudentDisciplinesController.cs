using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.Students.StudentDiscipline;
using SchoolWebApp.Core.Entities.Students;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.API.Controllers.Students
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentDisciplinesController : ControllerBase
    {
        private readonly ILogger<StudentDisciplinesController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public StudentDisciplinesController(ILogger<StudentDisciplinesController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/studentDisciplines
        /// <summary>
        /// A method for retrieving all student disciplines
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentDisciplineDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(_mapper.Map<List<StudentDisciplineDto>>(await _unitOfWork.StudentDisciplines.GetAll()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all student disciplines list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/studentDisciplines/paginated
        /// <summary>
        /// A method for retrieving a list of paginated student disciplines
        /// </summary>
        /// <param name="pageNumber">The page number to return</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns></returns>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentDisciplineDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _unitOfWork.StudentDisciplines.GetPaginatedData(pageNumber ?? 1, pageSize ?? 4);
                var mappedData = _mapper.Map<List<StudentDisciplineDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<StudentDisciplineDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated student disciplines.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        // GET api/studentDisciplines/byStudentId/5
        /// <summary>
        /// A method for retrieving student disciplines by student Id.
        /// </summary>
        /// <param name="id">The student Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("byStudentId/{studentId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentDisciplineDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStudentDisciplinesByStudentId(int studentId)
        {
            try
            {
                if (studentId <= 0) return BadRequest(studentId);
                var _item = await _unitOfWork.StudentDisciplines.GetByStudentId(studentId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<StudentDisciplineDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the student disciplines by student id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/studentDisciplines/5
        /// <summary>
        /// A method for retrieving of student disciplines record by Id.
        /// </summary>
        /// <param name="id">The student disciplines Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentDisciplineDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _unitOfWork.StudentDisciplines.GetById(id);

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<StudentDisciplineDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the student disciplines details by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/studentDisciplines
        /// <summary>
        /// A method for creating a student discipline record.
        /// </summary>
        /// <param name="model">The student discipline record to be created</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentDisciplineDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateStudentDisciplineDto model)
        {
            if (ModelState.IsValid)
            {
                if (!await _unitOfWork.Students.ItemExistsAsync(s => s.Id == model.StudentId))
                    return Conflict(new { message = $"The student details submitted do not exist." });
                if (await _unitOfWork.StudentDisciplines.ItemExistsAsync(s => s.StudentId == model.StudentId && s.OccurenceDetails == model.OccurenceDetails &&
                s.OccurenceStartDate == model.OccurenceStartDate && s.OccurenceEndDate == model.OccurenceEndDate && s.OccurenceTypeId == model.OccurenceTypeId))
                    return Conflict(new { message = $"The student discipline record already exists" });
                try
                {
                    var _item = _mapper.Map<StudentDiscipline>(model);
                    _unitOfWork.StudentDisciplines.Create(_item);
                    await _unitOfWork.SaveChangesAsync();
                    var returnItem = _mapper.Map<StudentDisciplineDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the student discipline");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the student discipline - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/studentDisciplines/5
        /// <summary>
        /// A method for updating a student discipline record.
        /// </summary>
        /// <param name="model">The student discipline record to be updated</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(StudentDisciplineDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _unitOfWork.StudentDisciplines.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The student discipline of Id- '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var existingItem = await _unitOfWork.StudentDisciplines.GetById(model.Id);
                    //Manual mapping
                    existingItem.StudentId = model.StudentId;
                    existingItem.OccurenceTypeId = model.OccurenceTypeId;
                    existingItem.OccurenceDetails = model.OccurenceDetails;
                    existingItem.OccurenceStartDate = model.OccurenceStartDate;
                    existingItem.OccurenceEndDate = model.OccurenceEndDate;
                    existingItem.OutcomeId = model.OutcomeId;
                    _unitOfWork.StudentDisciplines.Update(existingItem);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the student discipline.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the student discipline.");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE api/studentDisciplines/5
        /// <summary>
        /// A method for deleting the student discipline record by Id.
        /// </summary>
        /// <param name="id">The student discipline Id to be retrieved</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _unitOfWork.StudentDisciplines.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The student discipline of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _unitOfWork.StudentDisciplines.GetById(id);
                _unitOfWork.StudentDisciplines.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the student discipline.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the student discipline - " + ex.Message);
            }
        }
    }
}
