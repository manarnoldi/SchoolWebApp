using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.Settings.OccurenceType;
using SchoolWebApp.Core.Entities.Settings;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.API.Controllers.Settings
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OccurenceTypesController : ControllerBase
    {
        private readonly ILogger<OccurenceTypesController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public OccurenceTypesController(ILogger<OccurenceTypesController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/occurenceTypes
        /// <summary>
        /// A method for retrieving all occurence types
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<OccurenceTypeDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(_mapper.Map<List<OccurenceTypeDto>>(await _unitOfWork.OccurenceTypes.GetAll()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all occurence types list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/occurenceTypes/paginated
        /// <summary>
        /// A method for retrieving a list of paginated occurence types
        /// </summary>
        /// <param name="pageNumber">The page number to return</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns></returns>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<OccurenceTypeDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _unitOfWork.OccurenceTypes.GetPaginatedData(pageNumber ?? 1, pageSize ?? 4);
                var mappedData = _mapper.Map<List<OccurenceTypeDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<OccurenceTypeDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated occurence types.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/occurenceTypes/5
        /// <summary>
        /// A method for retrieving of occurence types record by Id.
        /// </summary>
        /// <param name="id">The occurence types Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OccurenceTypeDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _unitOfWork.OccurenceTypes.GetById(id);

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<OccurenceTypeDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the occurence types details by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/occurencetype
        /// <summary>
        /// A method for creating an occurence type record.
        /// </summary>
        /// <param name="model">The occurence type record to be created</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OccurenceTypeDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateOccurenceTypeDto model)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.OccurenceTypes.IsExists("Name", model.Name))
                    return Conflict(new { message = $"The occurence type name - '{model.Name}' already exists" });
                try
                {
                    var _item = _mapper.Map<OccurenceType>(model);
                    _unitOfWork.OccurenceTypes.Create(_item);
                    await _unitOfWork.SaveChangesAsync();
                    var returnItem = _mapper.Map<OccurenceTypeDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the occurence type");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the occurence type - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/occurenceTypes/5
        /// <summary>
        /// A method for updating a occurence type record.
        /// </summary>
        /// <param name="model">The occurence type record to be updated</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(OccurenceTypeDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _unitOfWork.OccurenceTypes.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The occurence type of Id- '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var existingItem = await _unitOfWork.OccurenceTypes.GetById(model.Id);
                    //Manual mapping
                    existingItem.Name = model.Name;
                    existingItem.Description = model.Description;
                    existingItem.Abbreviation = model.Abbreviation;

                    _unitOfWork.OccurenceTypes.Update(existingItem);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the occurence type.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the occurence type.");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE api/occurenceTypes/5
        /// <summary>
        /// A method for deleting the occurence type record by Id.
        /// </summary>
        /// <param name="id">The occurence type Id to be retrieved</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _unitOfWork.OccurenceTypes.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The occurence type of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _unitOfWork.OccurenceTypes.GetById(id);
                _unitOfWork.OccurenceTypes.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the occurence type.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the occurence type - " + ex.Message);
            }
        }
    }
}
