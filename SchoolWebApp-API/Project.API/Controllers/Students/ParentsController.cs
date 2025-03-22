using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using SchoolWebApp.API.Utils;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.Students.Parent;
using SchoolWebApp.Core.DTOs.Students.Student;
using SchoolWebApp.Core.Entities.Enums;
using SchoolWebApp.Core.Entities.Students;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.API.Controllers.Students
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ParentsController : ControllerBase
    {
        private readonly ILogger<ParentsController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ParentsController(ILogger<ParentsController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/parents/GetCount
        /// <summary>
        /// A method for getting the total number of parents in the school
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetCount")]
        public async Task<ActionResult<int>> GetCount(bool active)
        {
            var returnParentsReq = active ? _unitOfWork.Parents.RecordCount(s => s.Status == Status.Active) : _unitOfWork.Parents.RecordCount();
            return Ok(await returnParentsReq);
        }

        // GET: api/parents?active=true
        /// <summary>
        /// A method for retrieving all parents details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ParentDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(bool active)
        {
            try
            {
                var parentsReturn = active ? await _unitOfWork.Parents.Find(s => s.Status == Status.Active, includeProperties: "Occupation,Nationality,Religion,Gender") :
                    await _unitOfWork.Parents.Find(includeProperties: "Occupation,Nationality,Religion,Gender");
                return Ok(_mapper.Map<List<ParentDto>>(parentsReturn));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all parents list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/parents/paginated
        /// <summary>
        /// A method for retrieving a list of paginated parents
        /// </summary>
        /// <param name="pageNumber">The page number to return</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns></returns>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ParentDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _unitOfWork.Parents.GetPaginatedData(pageNumber ?? 1, pageSize ?? 4);
                var mappedData = _mapper.Map<List<ParentDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<ParentDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated parents list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/parents/parentStudents/5
        /// <summary>
        /// A method for retrieving students for parent Id.
        /// </summary>
        /// <param name="id">The parent Id whose records to be retrieved</param>
        /// <returns></returns>
        [HttpGet("parentStudents/{parentId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetParentStudents(int parentId)
        {
            try
            {
                if (parentId <= 0) return BadRequest(parentId);
                var _item = await _unitOfWork.Parents.GetStudentsByParentId(parentId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<StudentDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the parent's students by parent id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/parents/5
        /// <summary>
        /// A method for retrieving parent record by Id.
        /// </summary>
        /// <param name="id">The parent Id whose record is to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ParentDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _unitOfWork.Parents.GetById(id, includeProperties: "Occupation,Nationality,Religion,Gender");

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<ParentDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the parent record by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/parents
        /// <summary>
        /// A method for creating a parent record.
        /// </summary>
        /// <param name="model">The parent record to be created</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ParentDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateParentDto model)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.Parents.ItemExistsAsync(s => s.UPI == model.UPI))
                    return Conflict(new { message = $"The provided UPI (Unique Personal Identifier) provided already exists in the system." });
                if (await _unitOfWork.Parents.ItemExistsAsync(s => s.FullName == model.FullName && s.UPI == model.UPI && s.Status == model.Status))
                    return Conflict(new { message = $"The parent record submitted already exists." });
                try
                {
                    var _item = _mapper.Map<Parent>(model);
                    _unitOfWork.Parents.Create(_item);
                    await _unitOfWork.SaveChangesAsync();
                    var returnItem = _mapper.Map<ParentDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the parent details.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the parent details - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/parents/5
        /// <summary>
        /// A method for updating an parent record.
        /// </summary>
        /// <param name="model">The parent record to be updated</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(ParentDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _unitOfWork.Parents.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The parent record of Id - '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var _item = _mapper.Map<Parent>(model);
                    _unitOfWork.Parents.Update(_item);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the parent record.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the parent record.");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE api/parents/5
        /// <summary>
        /// A method for deleting the parent record by Id.
        /// </summary>
        /// <param name="id">The parent Id to be deleted</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _unitOfWork.Parents.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The parent of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _unitOfWork.Parents.GetById(id);
                _unitOfWork.Parents.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 2601 || ex.Number == 2627)
                {
                    _logger.LogError(ex, "Parent record cannot deleted since it contains related records." +
                        "Delete related records first before deleting this record.");
                    return StatusCode(StatusCodes.Status500InternalServerError, "Parent record cannot deleted since it contains related records." +
                        "Delete related records first before deleting this record.");
                }
                else
                {
                    _logger.LogError(ex, "An error occurred while deleting the parent record.");
                    return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the parent record - " +
                        HandleExceptions.GetMessageForInnerExceptions(ex));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the parent record.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the parent record - " +
                    HandleExceptions.GetMessageForInnerExceptions(ex));
            }
        }
    }
}
