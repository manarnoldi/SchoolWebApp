using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.Students.StudentParent;
using SchoolWebApp.Core.Entities.Students;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.API.Controllers.Students
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StudentParentController : ControllerBase
    {
        private readonly ILogger<StudentParentController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public StudentParentController(ILogger<StudentParentController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/studentParent
        /// <summary>
        /// A method for retrieving all student-parent
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentParentDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(_mapper.Map<List<StudentParentDto>>(await _unitOfWork.StudentParent.GetAll()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all student-parent list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/studentParent/paginated
        /// <summary>
        /// A method for retrieving a list of paginated Student-Parent
        /// </summary>
        /// <param name="pageNumber">The page number to return</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns></returns>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentParentDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _unitOfWork.StudentParent.GetPaginatedData(pageNumber ?? 1, pageSize ?? 4);
                var mappedData = _mapper.Map<List<StudentParentDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<StudentParentDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated student-parent list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/studentParent/5
        /// <summary>
        /// A method for retrieving a student-parent record by Id.
        /// </summary>
        /// <param name="id">The student-parent Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentParentDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _unitOfWork.StudentParent.GetById(id);

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<StudentParentDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the student-parent details by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/studentParent
        /// <summary>
        /// A method for creating a student-parent record.
        /// </summary>
        /// <param name="model">The student-parent record to be created</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentParentDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateStudentParentDto model)
        {
            if (ModelState.IsValid)
            {
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
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(StudentParentDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _unitOfWork.StudentParent.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The student-parent of Id - '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var existingItem = await _unitOfWork.StudentParent.GetById(model.Id);
                    //Manual mapping
                    existingItem.ParentId = model.ParentId;
                    existingItem.StudentId = model.StudentId;
                    existingItem.RelationShipId = model.RelationShipId;
                    existingItem.OtherDetails = model.OtherDetails;
                    _unitOfWork.StudentParent.Update(existingItem);
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

        // DELETE api/studentParent/5
        /// <summary>
        /// A method for deleting the student-parent record by Id.
        /// </summary>
        /// <param name="id">The student-parent Id to be deleted</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _unitOfWork.StudentParent.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The student-parent of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _unitOfWork.StudentParent.GetById(id);
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
