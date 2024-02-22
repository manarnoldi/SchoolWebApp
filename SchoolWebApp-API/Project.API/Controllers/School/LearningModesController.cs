using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.School.LearningMode;
using SchoolWebApp.Core.Entities.School;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.API.Controllers.School
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LearningModesController : ControllerBase
    {
        private readonly ILogger<LearningModesController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public LearningModesController(ILogger<LearningModesController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/learningModes
        /// <summary>
        /// A method for retrieving all learning modes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<LearningModeDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(_mapper.Map<List<LearningModeDto>>(await _unitOfWork.LearningModes.GetAll()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all learning modes list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/learningModes/paginated
        /// <summary>
        /// A method for retrieving a list of paginated learning modes
        /// </summary>
        /// <param name="pageNumber">The page number to return</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns></returns>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<LearningModeDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _unitOfWork.LearningModes.GetPaginatedData(pageNumber ?? 1, pageSize ?? 4);
                var mappedData = _mapper.Map<List<LearningModeDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<LearningModeDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated learning modes.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/learningModes/5
        /// <summary>
        /// A method for retrieving of learning modes record by Id.
        /// </summary>
        /// <param name="id">The learning modes Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LearningModeDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _unitOfWork.LearningModes.GetById(id);

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<LearningModeDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the learning modes details by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/learningMode
        /// <summary>
        /// A method for creating a learning mode record.
        /// </summary>
        /// <param name="model">The learning mode record to be created</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LearningModeDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateLearningModeDto model)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.LearningModes.IsExists("Name", model.Name))
                    return Conflict(new { message = $"The learning mode name - '{model.Name}' already exists" });
                try
                {
                    var _item = _mapper.Map<LearningMode>(model);
                    _unitOfWork.LearningModes.Create(_item);
                    await _unitOfWork.SaveChangesAsync();
                    var returnItem = _mapper.Map<LearningModeDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the learning mode");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the learning mode - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/learningModes/5
        /// <summary>
        /// A method for updating a learning mode record.
        /// </summary>
        /// <param name="model">The learning mode record to be updated</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(LearningModeDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _unitOfWork.LearningModes.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The learning mode of Id- '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var existingItem = await _unitOfWork.LearningModes.GetById(model.Id);
                    //Manual mapping
                    existingItem.Name = model.Name;
                    existingItem.Description = model.Description;
                    _unitOfWork.LearningModes.Update(existingItem);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the learning mode.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the learning mode.");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE api/learningModes/5
        /// <summary>
        /// A method for deleting the learning mode record by Id.
        /// </summary>
        /// <param name="id">The learning mode Id to be retrieved</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _unitOfWork.LearningModes.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The learning mode of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _unitOfWork.LearningModes.GetById(id);
                _unitOfWork.LearningModes.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the learning mode.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the learning mode - " + ex.Message);
            }
        }
    }
}
