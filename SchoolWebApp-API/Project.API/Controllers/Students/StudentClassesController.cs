using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.API.Controllers.Academics;
using SchoolWebApp.Core.DTOs.Academics.Grade;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.DTOs.Class.SchoolClass;
using Microsoft.AspNetCore.Authorization;
using SchoolWebApp.Core.DTOs.Students.StudentClass;
using SchoolWebApp.Core.Entities.Students;

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
                return Ok(_mapper.Map<List<StudentClassDto>>(await _unitOfWork.StudentClasses.GetAll()));
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
                var _item = await _unitOfWork.StudentClasses.GetById(id);

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
                    var existingItem = await _unitOfWork.StudentClasses.GetById(model.Id);
                    //Manual mapping
                    existingItem.StudentId = model.StudentId;
                    existingItem.SchoolClassId = model.SchoolClassId;
                    existingItem.Description = model.Description;
                    _unitOfWork.StudentClasses.Update(existingItem);
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
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the student class - " + ex.Message);
            }
        }
    }
}
