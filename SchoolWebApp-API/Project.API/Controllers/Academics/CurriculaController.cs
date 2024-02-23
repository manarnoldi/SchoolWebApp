using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.Academics.Curriculum;
using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.API.Controllers.Academics
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CurriculaController : ControllerBase
    {
        private readonly ILogger<CurriculaController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CurriculaController(ILogger<CurriculaController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/curricula
        /// <summary>
        /// A method for retrieving all curricula
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CurriculumDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(_mapper.Map<List<CurriculumDto>>(await _unitOfWork.Curricula.GetAll()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all curricula list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/curricula/paginated
        /// <summary>
        /// A method for retrieving a list of paginated curricula
        /// </summary>
        /// <param name="pageNumber">The page number to return</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns></returns>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CurriculumDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _unitOfWork.Curricula.GetPaginatedData(pageNumber ?? 1, pageSize ?? 4);
                var mappedData = _mapper.Map<List<CurriculumDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<CurriculumDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated curricula.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/curricula/5
        /// <summary>
        /// A method for retrieving of curriculum record by Id.
        /// </summary>
        /// <param name="id">The curriculum Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CurriculumDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _unitOfWork.Curricula.GetById(id);

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<CurriculumDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the curriculum details by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/curricula
        /// <summary>
        /// A method for creating a curriculum record.
        /// </summary>
        /// <param name="model">The curriculum record to be created</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CurriculumDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateCurriculumDto model)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.Curricula.IsExists("Name", model.Name))
                    return Conflict(new { message = $"The curriculum name - '{model.Name}' already exists" });
                try
                {
                    var _item = _mapper.Map<Curriculum>(model);
                    _unitOfWork.Curricula.Create(_item);
                    await _unitOfWork.SaveChangesAsync();
                    var returnItem = _mapper.Map<CurriculumDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the curriculum.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the curriculum - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/curricula/5
        /// <summary>
        /// A method for updating the curriculum record.
        /// </summary>
        /// <param name="model">The curriculum record to be updated</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(CurriculumDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _unitOfWork.Curricula.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The curriculum of Id- '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var existingItem = await _unitOfWork.Curricula.GetById(model.Id);
                    //Manual mapping
                    existingItem.Name = model.Name;
                    existingItem.Code = model.Code;
                    existingItem.Description = model.Description;
                    _unitOfWork.Curricula.Update(existingItem);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the curriculum.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the curriculum.");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE api/curricula/5
        /// <summary>
        /// A method for deleting the curriculum record by Id.
        /// </summary>
        /// <param name="id">The curriculum Id to be deleted</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _unitOfWork.Curricula.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The curriculum of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _unitOfWork.Curricula.GetById(id);
                _unitOfWork.Curricula.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the curriculum.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the curriculum - " + ex.Message);
            }
        }
    }
}
