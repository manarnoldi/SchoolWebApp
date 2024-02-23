using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs.Academics.Grade;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.DTOs.Academics.SubjectGroup;

namespace SchoolWebApp.API.Controllers.Academics
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectGroupsController : ControllerBase
    {
        private readonly ILogger<SubjectGroupsController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public SubjectGroupsController(ILogger<SubjectGroupsController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/subjectGroups
        /// <summary>
        /// A method for retrieving all subject groups
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SubjectGroupDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(_mapper.Map<List<SubjectGroupDto>>(await _unitOfWork.SubjectGroups.GetAll()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all subject groups list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/subjectGroups/paginated
        /// <summary>
        /// A method for retrieving a list of paginated subject groups
        /// </summary>
        /// <param name="pageNumber">The page number to return</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns></returns>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SubjectGroupDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _unitOfWork.SubjectGroups.GetPaginatedData(pageNumber ?? 1, pageSize ?? 4);
                var mappedData = _mapper.Map<List<SubjectGroupDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<SubjectGroupDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated subject groups.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/subjectGroups/5
        /// <summary>
        /// A method for retrieving a subject group record by Id.
        /// </summary>
        /// <param name="id">The subject group Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SubjectGroupDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _unitOfWork.SubjectGroups.GetById(id);

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<SubjectGroupDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the subject group details by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/subjectGroups
        /// <summary>
        /// A method for creating a subject group record.
        /// </summary>
        /// <param name="model">The subject group record to be created</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SubjectGroupDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateSubjectGroupDto model)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.SubjectGroups.ItemExistsAsync(s => s.Name == model.Name && s.DepartmentId == model.DepartmentId))
                    return Conflict(new { message = $"The subject group details submitted already exist" });
                try
                {
                    var _item = _mapper.Map<SubjectGroup>(model);
                    _unitOfWork.SubjectGroups.Create(_item);
                    await _unitOfWork.SaveChangesAsync();
                    var returnItem = _mapper.Map<SubjectGroupDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the subject group.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the subject group - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/subjectGroups/5
        /// <summary>
        /// A method for updating a subject group record.
        /// </summary>
        /// <param name="model">The subject group record to be updated</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(SubjectGroupDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _unitOfWork.SubjectGroups.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The subject group of Id - '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var existingItem = await _unitOfWork.SubjectGroups.GetById(model.Id);
                    //Manual mapping
                    existingItem.Name = model.Name;
                    existingItem.DepartmentId = model.DepartmentId;
                    _unitOfWork.SubjectGroups.Update(existingItem);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the subject group.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the subject group.");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE api/subjectGroups/5
        /// <summary>
        /// A method for deleting the subject group record by Id.
        /// </summary>
        /// <param name="id">The subject group Id to be retrieved</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _unitOfWork.SubjectGroups.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The subject group of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _unitOfWork.SubjectGroups.GetById(id);
                _unitOfWork.SubjectGroups.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the subject group.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the subject group - " + ex.Message);
            }
        }
    }
}
