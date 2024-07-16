using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.Students.FormerSchool;
using SchoolWebApp.Core.Entities.Staff;
using SchoolWebApp.Core.Entities.Students;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.API.Controllers.Students
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FormerSchoolsController : ControllerBase
    {
        private readonly ILogger<FormerSchoolsController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public FormerSchoolsController(ILogger<FormerSchoolsController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/formerSchools
        /// <summary>
        /// A method for retrieving all former schools
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<FormerSchoolDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(_mapper.Map<List<FormerSchoolDto>>(await _unitOfWork.FormerSchools.Find(includeProperties:"Student,Curriculum,EducationLevel")));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all former schools list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/formerSchools/paginated
        /// <summary>
        /// A method for retrieving a list of paginated former schools
        /// </summary>
        /// <param name="pageNumber">The page number to return</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns></returns>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<FormerSchoolDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _unitOfWork.FormerSchools.GetPaginatedData(pageNumber ?? 1, pageSize ?? 4);
                var mappedData = _mapper.Map<List<FormerSchoolDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<FormerSchoolDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated former schools.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        // GET api/formerSchools/byStudentId/5
        /// <summary>
        /// A method for retrieving former schools by student Id.
        /// </summary>
        /// <param name="id">The student Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("byStudentId/{studentId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FormerSchoolDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GeByStudentId(int studentId)
        {
            try
            {
                if (studentId <= 0) return BadRequest(studentId);
                var _item = await _unitOfWork.FormerSchools.GetByStudentId(studentId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<FormerSchoolDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the former schools by student id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/formerSchools/5
        /// <summary>
        /// A method for retrieving of former schools record by Id.
        /// </summary>
        /// <param name="id">The former schools Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FormerSchoolDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _unitOfWork.FormerSchools.GetById(id);

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<FormerSchoolDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the former schools details by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/formerSchools
        /// <summary>
        /// A method for creating a former school record.
        /// </summary>
        /// <param name="model">The former school record to be created</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FormerSchoolDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateFormerSchoolDto model)
        {
            if (ModelState.IsValid)
            {
                if (!await _unitOfWork.Students.ItemExistsAsync(s => s.Id == model.StudentId))
                    return Conflict(new { message = $"The student details submitted do not exist." });
                if (await _unitOfWork.FormerSchools.ItemExistsAsync(s => s.StudentId == model.StudentId && s.CurriculumId == model.CurriculumId &&
                s.EducationLevelId == model.EducationLevelId && s.SchoolName == model.SchoolName && s.ClassDetails == model.ClassDetails && 
                s.Score == model.Score && s.Position == model.Position))
                    return Conflict(new { message = $"The former school record already exists" });
                try
                {
                    var _item = _mapper.Map<FormerSchool>(model);
                    _unitOfWork.FormerSchools.Create(_item);
                    await _unitOfWork.SaveChangesAsync();
                    var returnItem = _mapper.Map<FormerSchoolDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the former school");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the former school - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/formerSchools/5
        /// <summary>
        /// A method for updating a former school record.
        /// </summary>
        /// <param name="model">The former school record to be updated</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(FormerSchoolDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _unitOfWork.FormerSchools.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The former school of Id- '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var _item = _mapper.Map<FormerSchool>(model);
                    _unitOfWork.FormerSchools.Update(_item);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the former school.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the former school.");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE api/formerSchools/5
        /// <summary>
        /// A method for deleting the former school record by Id.
        /// </summary>
        /// <param name="id">The former school Id to be retrieved</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _unitOfWork.FormerSchools.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The former school of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _unitOfWork.FormerSchools.GetById(id);
                _unitOfWork.FormerSchools.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the former school.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the former school - " + ex.Message);
            }
        }
    }
}
