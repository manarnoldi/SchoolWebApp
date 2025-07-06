using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs.Academics.ExamName;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.DTOs.Academics.ExamName;

namespace SchoolWebApp.API.Controllers.Academics
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ExamNamesController : ControllerBase
    {
        private readonly ILogger<ExamNamesController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ExamNamesController(ILogger<ExamNamesController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/examNames
        /// <summary>
        /// A method for retrieving all exam ty
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ExamNameDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(_mapper.Map<List<ExamNameDto>>(await _unitOfWork.ExamNames.GetAll()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all exam names list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/examNames/paginated
        /// <summary>
        /// A method for retrieving a list of paginated exam names
        /// </summary>
        /// <param name="pageNumber">The page number to return</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns></returns>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ExamNameDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _unitOfWork.ExamNames.GetPaginatedData(pageNumber ?? 1, pageSize ?? 4);
                var mappedData = _mapper.Map<List<ExamNameDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<ExamNameDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated exam names.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/examNames/5
        /// <summary>
        /// A method for retrieving of exam names record by Id.
        /// </summary>
        /// <param name="id">The exam names Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExamNameDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _unitOfWork.ExamNames.GetById(id);

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<ExamNameDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the exam names details by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/examNames
        /// <summary>
        /// A method for creating a exam name record.
        /// </summary>
        /// <param name="model">The exam name record to be created</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExamNameDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateExamNameDto model)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.ExamNames.IsExists("Name", model.Name))
                    return Conflict(new { message = $"The exam name name - '{model.Name}' already exists" });
                try
                {
                    var _item = _mapper.Map<ExamName>(model);
                    _unitOfWork.ExamNames.Create(_item);
                    await _unitOfWork.SaveChangesAsync();
                    var returnItem = _mapper.Map<ExamNameDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the exam name");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the exam name - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/examNames/5
        /// <summary>
        /// A method for updating a exam name record.
        /// </summary>
        /// <param name="model">The exam name record to be updated</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(ExamNameDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _unitOfWork.ExamNames.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The exam name of Id- '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var _item = _mapper.Map<ExamName>(model);
                    _unitOfWork.ExamNames.Update(_item);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the exam name.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the exam name.");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE api/examNames/5
        /// <summary>
        /// A method for deleting the exam name record by Id.
        /// </summary>
        /// <param name="id">The exam name Id to be retrieved</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _unitOfWork.ExamNames.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The exam name of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _unitOfWork.ExamNames.GetById(id);
                _unitOfWork.ExamNames.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the exam name.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the exam name - " + ex.Message);
            }
        }
    }
}
