using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.Class.LearningLevel;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.API.Controllers.Class
{
    [Route("api/[controller]")]
    [ApiController]
    public class LearningLevelsController : ControllerBase
    {
        private readonly ILogger<LearningLevelsController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public LearningLevelsController(ILogger<LearningLevelsController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/learningLevels
        /// <summary>
        /// A method for retrieving all learning levels
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<LearningLevelDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(_mapper.Map<List<LearningLevelDto>>(await _unitOfWork.LearningLevels.Find(includeProperties: "EducationLevel")));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all learning levels list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/learningLevels/paginated
        /// <summary>
        /// A method for retrieving a list of paginated learning levels
        /// </summary>
        /// <param name="pageNumber">The page number to return</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns></returns>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<LearningLevelDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _unitOfWork.LearningLevels.GetPaginatedData(pageNumber ?? 1, pageSize ?? 4);
                var mappedData = _mapper.Map<List<LearningLevelDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<LearningLevelDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated learning levels.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/learningLevels/5
        /// <summary>
        /// A method for retrieving of learning levels record by Id.
        /// </summary>
        /// <param name="id">The learning levels Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LearningLevelDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _unitOfWork.LearningLevels.GetById(id);

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<LearningLevelDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the learning levels details by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/learningLevels
        /// <summary>
        /// A method for creating a learning level record.
        /// </summary>
        /// <param name="model">The learning level record to be created</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LearningLevelDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateLearningLevelDto model)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.LearningLevels.IsExists("Name", model.Name))
                    return Conflict(new { message = $"The learning level name - '{model.Name}' already exists" });
                try
                {
                    var _item = _mapper.Map<LearningLevel>(model);
                    _unitOfWork.LearningLevels.Create(_item);
                    await _unitOfWork.SaveChangesAsync();
                    var returnItem = _mapper.Map<LearningLevelDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the learning level");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the learning level - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/learningLevels/5
        /// <summary>
        /// A method for updating a learning level record.
        /// </summary>
        /// <param name="model">The learning level record to be updated</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(LearningLevelDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _unitOfWork.LearningLevels.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The learning level of Id- '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var existingItem = await _unitOfWork.LearningLevels.GetById(model.Id);
                    //Manual mapping
                    existingItem.Name = model.Name;
                    existingItem.Description = model.Description;
                    existingItem.EducationLevelId = model.EducationLevelId;
                    _unitOfWork.LearningLevels.Update(existingItem);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the learning level.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the learning level.");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE api/learningLevels/5
        /// <summary>
        /// A method for deleting the learning level record by Id.
        /// </summary>
        /// <param name="id">The learning level Id to be retrieved</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _unitOfWork.LearningLevels.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The learning level of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _unitOfWork.LearningLevels.GetById(id);
                _unitOfWork.LearningLevels.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the learning level.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the learning level - " + ex.Message);
            }
        }
    }
}
