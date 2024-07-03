using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.School.SchoolStream;
using SchoolWebApp.Core.Entities.School;
using SchoolWebApp.Core.Entities.Staff;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.API.Controllers.School
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolStreamsController : ControllerBase
    {
        private readonly ILogger<SchoolStreamsController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public SchoolStreamsController(ILogger<SchoolStreamsController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/schoolStreams
        /// <summary>
        /// A method for retrieving all school streams
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SchoolStreamDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(_mapper.Map<List<SchoolStreamDto>>(await _unitOfWork.SchoolStreams.GetAll()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all school streams list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/schoolStreams/paginated
        /// <summary>
        /// A method for retrieving a list of paginated school streams
        /// </summary>
        /// <param name="pageNumber">The page number to return</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns></returns>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SchoolStreamDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _unitOfWork.SchoolStreams.GetPaginatedData(pageNumber ?? 1, pageSize ?? 4);
                var mappedData = _mapper.Map<List<SchoolStreamDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<SchoolStreamDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated school streams.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/schoolStreams/5
        /// <summary>
        /// A method for retrieving of school stream record by Id.
        /// </summary>
        /// <param name="id">The school stream Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SchoolStreamDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _unitOfWork.SchoolStreams.GetById(id);

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<SchoolStreamDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the school stream details by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/schoolStreams
        /// <summary>
        /// A method for creating school an school stream record.
        /// </summary>
        /// <param name="model">The school stream record to be created</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SchoolStreamDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateSchoolStreamDto model)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.SchoolStreams.IsExists("Name", model.Name))
                    return Conflict(new { message = $"The school stream name - '{model.Name}' already exists" });
                try
                {
                    var _item = _mapper.Map<SchoolStream>(model);
                    _unitOfWork.SchoolStreams.Create(_item);
                    await _unitOfWork.SaveChangesAsync();
                    var returnItem = _mapper.Map<SchoolStreamDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the school stream");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the school stream - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/schoolStreams/5
        /// <summary>
        /// A method for updating a school stream record.
        /// </summary>
        /// <param name="model">The school stream record to be updated</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(SchoolStreamDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _unitOfWork.SchoolStreams.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The school stream of Id- '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var _item = _mapper.Map<SchoolStream>(model);
                    _unitOfWork.SchoolStreams.Update(_item);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the school stream.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the school stream.");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE api/schoolStreams/5
        /// <summary>
        /// A method for deleting the school stream record by Id.
        /// </summary>
        /// <param name="id">The school stream Id to be retrieved</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _unitOfWork.SchoolStreams.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The school stream of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _unitOfWork.SchoolStreams.GetById(id);
                _unitOfWork.SchoolStreams.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the school stream.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the school stream - " + ex.Message);
            }
        }
    }
}
