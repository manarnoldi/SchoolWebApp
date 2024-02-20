using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.Settings.SchoolLevel;
using SchoolWebApp.Core.Entities.Settings;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.API.Controllers.Settings
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolLevelsController : ControllerBase
    {
        private readonly ILogger<SchoolLevelsController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public SchoolLevelsController(ILogger<SchoolLevelsController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/schoolLevels
        /// <summary>
        /// A method for retrieving all school levels
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SchoolLevelDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(_mapper.Map<List<SchoolLevelDto>>(await _unitOfWork.SchoolLevels.GetAll()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all school levels list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/schoolLevels/paginated
        /// <summary>
        /// A method for retrieving a list of paginated school levels
        /// </summary>
        /// <param name="pageNumber">The page number to return</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns></returns>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SchoolLevelDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _unitOfWork.SchoolLevels.GetPaginatedData(pageNumber ?? 1, pageSize ?? 4);
                var mappedData = _mapper.Map<List<SchoolLevelDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<SchoolLevelDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated school levels.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/schoolLevels/5
        /// <summary>
        /// A method for retrieving of school levels record by Id.
        /// </summary>
        /// <param name="id">The school levels Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SchoolLevelDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _unitOfWork.SchoolLevels.GetById(id);

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<SchoolLevelDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the school levels details by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/schoolLevel
        /// <summary>
        /// A method for creating a school level record.
        /// </summary>
        /// <param name="model">The school level record to be created</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SchoolLevelDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateSchoolLevelDto model)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.SchoolLevels.IsExists("Name", model.Name))
                    return Conflict(new { message = $"The school level name - '{model.Name}' already exists" });
                try
                {
                    var _item = _mapper.Map<SchoolLevel>(model);
                    _unitOfWork.SchoolLevels.Create(_item);
                    await _unitOfWork.SaveChangesAsync();
                    var returnItem = _mapper.Map<SchoolLevelDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the school level");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the school level - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/schoolLevels/5
        /// <summary>
        /// A method for updating a school level record.
        /// </summary>
        /// <param name="model">The school level record to be updated</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(SchoolLevelDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _unitOfWork.SchoolLevels.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The school level of Id- '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var existingItem = await _unitOfWork.SchoolLevels.GetById(model.Id);
                    //Manual mapping
                    existingItem.Name = model.Name;
                    existingItem.Description = model.Description;
                    _unitOfWork.SchoolLevels.Update(existingItem);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the school level.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the school level.");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE api/schoolLevels/5
        /// <summary>
        /// A method for deleting the school level record by Id.
        /// </summary>
        /// <param name="id">The school level Id to be retrieved</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _unitOfWork.SchoolLevels.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The school level of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _unitOfWork.SchoolLevels.GetById(id);
                _unitOfWork.SchoolLevels.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the school level.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the school level - " + ex.Message);
            }
        }
    }
}
