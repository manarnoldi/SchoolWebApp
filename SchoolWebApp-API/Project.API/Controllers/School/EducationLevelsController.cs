using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.School.EducationLevel;
using SchoolWebApp.Core.Entities.School;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.API.Controllers.School
{
    [Route("api/[controller]")]
    [ApiController]
    public class EducationLevelsController : ControllerBase
    {
        private readonly ILogger<EducationLevelsController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public EducationLevelsController(ILogger<EducationLevelsController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/educationLevels
        /// <summary>
        /// A method for retrieving all education levels
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<EducationLevelDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(_mapper.Map<List<EducationLevelDto>>(await _unitOfWork.EducationLevels.Find(includeProperties: "Curriculum,EducationLevelType")));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all education levels list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/educationLevels/paginated
        /// <summary>
        /// A method for retrieving a list of paginated education levels
        /// </summary>
        /// <param name="pageNumber">The page number to return</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns></returns>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<EducationLevelDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _unitOfWork.EducationLevels.GetPaginatedData(pageNumber ?? 1, pageSize ?? 4);
                var mappedData = _mapper.Map<List<EducationLevelDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<EducationLevelDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated education levels.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/educationLevels/byCurriculumId/5
        /// <summary>
        /// A method for retrieving education levels by curriculum Id.
        /// </summary>
        /// <param name="id">The curriculum Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("byCurriculumId/{curriculumId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EducationLevelDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEducationLevelsByCurriculumId(int curriculumId)
        {
            try
            {
                if (curriculumId <= 0) return BadRequest(curriculumId);
                var _item = await _unitOfWork.EducationLevels.GetByCurriculumId(curriculumId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<EducationLevelDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the education levels by curriculum id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/educationLevels/5
        /// <summary>
        /// A method for retrieving a education level record by Id.
        /// </summary>
        /// <param name="id">The education level Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EducationLevelDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _unitOfWork.EducationLevels.GetById(id);

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<EducationLevelDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the education level details by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/educationLevels
        /// <summary>
        /// A method for creating a education level record.
        /// </summary>
        /// <param name="model">The education level record to be created</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EducationLevelDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateEducationLevelDto model)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.EducationLevels.ItemExistsAsync(s => s.Name == model.Name && s.EducationLevelTypeId == model.EducationLevelTypeId
                && s.CurriculumId == model.CurriculumId))
                    return Conflict(new { message = $"The education level details submitted already exists" });
                try
                {
                    var _item = _mapper.Map<EducationLevel>(model);
                    _unitOfWork.EducationLevels.Create(_item);
                    await _unitOfWork.SaveChangesAsync();
                    var returnItem = _mapper.Map<EducationLevelDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the education level.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the education level - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/educationLevels/5
        /// <summary>
        /// A method for updating a education level record.
        /// </summary>
        /// <param name="model">The education level record to be updated</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(EducationLevelDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _unitOfWork.EducationLevels.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The education level of Id - '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var existingItem = await _unitOfWork.EducationLevels.GetById(model.Id);
                    //Manual mapping
                    existingItem.Name = model.Name;
                    existingItem.Abbr = model.Abbr;
                    existingItem.NumOfYears = model.NumOfYears;
                    existingItem.Description = model.Description;
                    existingItem.CurriculumId = model.CurriculumId;
                    existingItem.EducationLevelTypeId = model.EducationLevelTypeId;
                    _unitOfWork.EducationLevels.Update(existingItem);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the education level.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the education level.");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE api/educationLevels/5
        /// <summary>
        /// A method for deleting the education level record by Id.
        /// </summary>
        /// <param name="id">The education level Id to be deleted</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _unitOfWork.EducationLevels.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The education level of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _unitOfWork.EducationLevels.GetById(id);
                _unitOfWork.EducationLevels.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the education level.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the education level - " + ex.Message);
            }
        }
    }
}
