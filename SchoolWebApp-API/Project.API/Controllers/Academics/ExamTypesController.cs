using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.Academics.ExamType;
using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.API.Controllers.Academics
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamTypesController : ControllerBase
    {
        private readonly ILogger<ExamTypesController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ExamTypesController(ILogger<ExamTypesController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/examTypes
        /// <summary>
        /// A method for retrieving all exam types
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ExamTypeDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(_mapper.Map<List<ExamTypeDto>>(await _unitOfWork.ExamTypes.GetAll()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all exam types list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/examTypes/paginated
        /// <summary>
        /// A method for retrieving a list of paginated exam types
        /// </summary>
        /// <param name="pageNumber">The page number to return</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns></returns>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ExamTypeDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _unitOfWork.ExamTypes.GetPaginatedData(pageNumber ?? 1, pageSize ?? 4);
                var mappedData = _mapper.Map<List<ExamTypeDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<ExamTypeDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated exam types.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/examTypes/5
        /// <summary>
        /// A method for retrieving of exam types record by Id.
        /// </summary>
        /// <param name="id">The exam types Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExamTypeDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _unitOfWork.ExamTypes.GetById(id);

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<ExamTypeDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the exam types details by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/examTypes
        /// <summary>
        /// A method for creating a exam type record.
        /// </summary>
        /// <param name="model">The exam type record to be created</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExamTypeDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateExamTypeDto model)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.ExamTypes.IsExists("Name", model.Name))
                    return Conflict(new { message = $"The exam type name - '{model.Name}' already exists" });
                try
                {
                    var _item = _mapper.Map<ExamType>(model);
                    _unitOfWork.ExamTypes.Create(_item);
                    await _unitOfWork.SaveChangesAsync();
                    var returnItem = _mapper.Map<ExamTypeDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the exam type");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the exam type - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/examTypes/5
        /// <summary>
        /// A method for updating a exam type record.
        /// </summary>
        /// <param name="model">The exam type record to be updated</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(ExamTypeDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _unitOfWork.ExamTypes.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The exam type of Id- '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var existingItem = await _unitOfWork.ExamTypes.GetById(model.Id);
                    //Manual mapping
                    existingItem.Name = model.Name;
                    existingItem.Description = model.Description;
                    existingItem.Abbreviation = model.Abbreviation;
                    existingItem.Featured = model.Featured;
                    _unitOfWork.ExamTypes.Update(existingItem);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the exam type.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the exam type.");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE api/examTypes/5
        /// <summary>
        /// A method for deleting the exam type record by Id.
        /// </summary>
        /// <param name="id">The exam type Id to be retrieved</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _unitOfWork.ExamTypes.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The exam type of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _unitOfWork.ExamTypes.GetById(id);
                _unitOfWork.ExamTypes.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the exam type.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the exam type - " + ex.Message);
            }
        }
    }
}
