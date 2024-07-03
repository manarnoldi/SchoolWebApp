using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.Settings.Outcome;
using SchoolWebApp.Core.Entities.Settings;
using SchoolWebApp.Core.Entities.Staff;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.API.Controllers.Settings
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OutcomesController : ControllerBase
    {
        private readonly ILogger<OutcomesController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public OutcomesController(ILogger<OutcomesController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/outcomes
        /// <summary>
        /// A method for retrieving all outcomes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<OutcomeDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(_mapper.Map<List<OutcomeDto>>(await _unitOfWork.Outcomes.GetAll()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all outcomes list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/outcomes/paginated
        /// <summary>
        /// A method for retrieving a list of paginated outcomes
        /// </summary>
        /// <param name="pageNumber">The page number to return</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns></returns>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<OutcomeDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _unitOfWork.Outcomes.GetPaginatedData(pageNumber ?? 1, pageSize ?? 4);
                var mappedData = _mapper.Map<List<OutcomeDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<OutcomeDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated outcomes.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/outcomes/5
        /// <summary>
        /// A method for retrieving of outcomes record by Id.
        /// </summary>
        /// <param name="id">The outcomes Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OutcomeDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _unitOfWork.Outcomes.GetById(id);

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<OutcomeDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the outcomes details by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/outcome
        /// <summary>
        /// A method for creating a outcome record.
        /// </summary>
        /// <param name="model">The outcome record to be created</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OutcomeDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateOutcomeDto model)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.Outcomes.IsExists("Name", model.Name))
                    return Conflict(new { message = $"The outcome name - '{model.Name}' already exists" });
                try
                {
                    var _item = _mapper.Map<Outcome>(model);
                    _unitOfWork.Outcomes.Create(_item);
                    await _unitOfWork.SaveChangesAsync();
                    var returnItem = _mapper.Map<OutcomeDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the outcome");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the outcome - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/outcomes/5
        /// <summary>
        /// A method for updating a outcome record.
        /// </summary>
        /// <param name="model">The outcome record to be updated</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(OutcomeDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _unitOfWork.Outcomes.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The outcome of Id- '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var _item = _mapper.Map<Outcome>(model);
                    _unitOfWork.Outcomes.Update(_item);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the outcome.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the outcome.");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE api/outcomes/5
        /// <summary>
        /// A method for deleting the outcome record by Id.
        /// </summary>
        /// <param name="id">The outcome Id to be retrieved</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _unitOfWork.Outcomes.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The outcome of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _unitOfWork.Outcomes.GetById(id);
                _unitOfWork.Outcomes.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the outcome.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the outcome - " + ex.Message);
            }
        }
    }
}
