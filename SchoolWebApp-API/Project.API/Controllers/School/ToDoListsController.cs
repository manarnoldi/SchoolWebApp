using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs.School.Event;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.Entities.School;
using SchoolWebApp.Core.Interfaces.IRepositories;
using Microsoft.AspNetCore.Authorization;
using SchoolWebApp.Core.DTOs.School.ToDoList;

namespace SchoolWebApp.API.Controllers.School
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoListsController : ControllerBase
    {
        private readonly ILogger<ToDoListsController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ToDoListsController(ILogger<ToDoListsController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/toDoLists
        /// <summary>
        /// A method for retrieving all to do lists
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ToDoListDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(_mapper.Map<List<ToDoListDto>>(await _unitOfWork.ToDoLists.GetAll()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all to do list items.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/toDoLists/paginated
        /// <summary>
        /// A method for retrieving a list of paginated to do lists
        /// </summary>
        /// <param name="pageNumber">The page number to return</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns></returns>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ToDoListDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _unitOfWork.ToDoLists.GetPaginatedData(pageNumber ?? 1, pageSize ?? 4);
                var mappedData = _mapper.Map<List<ToDoListDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<ToDoListDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated to do lists.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/toDoLists/byStaffId/5
        /// <summary>
        /// A method for retrieving to do list items by staff Id.
        /// </summary>
        /// <param name="id">The staff Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("byStaffId/{staffId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ToDoListDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetToDoListsByStaffId(int staffId)
        {
            try
            {
                if (staffId <= 0) return BadRequest(staffId);
                var _item = await _unitOfWork.ToDoLists.GetByStaffId(staffId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<ToDoListDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the to do list items by staff id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/toDoLists/5
        /// <summary>
        /// A method for retrieving an to do list items by Id.
        /// </summary>
        /// <param name="id">The to do list item Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ToDoListDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _unitOfWork.ToDoLists.GetById(id);

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<ToDoListDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the to do list item by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/toDoLists
        /// <summary>
        /// A method for creating a to do list item.
        /// </summary>
        /// <param name="model">The to do list item to be created</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ToDoListDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateToDoListDto model)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.ToDoLists.ItemExistsAsync(s => s.ItemName == model.ItemName && s.Completed == model.Completed
                && s.StaffDetailsId == model.StaffDetailsId && s.CompleteBy == model.CompleteBy))
                    return Conflict(new { message = $"The to do list item submitted already exist." });
                try
                {
                    var _item = _mapper.Map<ToDoList>(model);
                    _unitOfWork.ToDoLists.Create(_item);
                    await _unitOfWork.SaveChangesAsync();
                    var returnItem = _mapper.Map<ToDoListDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the to do list item.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the to do list item - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/toDoLists/5
        /// <summary>
        /// A method for updating a to do list item.
        /// </summary>
        /// <param name="model">The to do list item to be updated</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(ToDoListDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _unitOfWork.ToDoLists.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The to do list item of Id - '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var existingItem = await _unitOfWork.ToDoLists.GetById(model.Id);
                    //Manual mapping
                    existingItem.ItemName = model.ItemName;
                    existingItem.CompleteBy = model.CompleteBy;
                    existingItem.Completed = model.Completed;
                    existingItem.StaffDetailsId = model.StaffDetailsId;
                    _unitOfWork.ToDoLists.Update(existingItem);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the to do list item.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the to do list item.");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE api/toDoLists/5
        /// <summary>
        /// A method for deleting the to do list item by Id.
        /// </summary>
        /// <param name="id">The to do list item Id to be retrieved</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _unitOfWork.ToDoLists.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The to do list item of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _unitOfWork.ToDoLists.GetById(id);
                _unitOfWork.ToDoLists.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the to do list item.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the to do list item - " + ex.Message);
            }
        }
    }
}
