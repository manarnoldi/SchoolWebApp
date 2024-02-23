using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.Academics.AcademicYear;
using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.API.Controllers.Academics
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AcademicYearsController : ControllerBase
    {
        private readonly ILogger<AcademicYearsController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public AcademicYearsController(ILogger<AcademicYearsController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/academicYears
        /// <summary>
        /// A method for retrieving all academic years
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AcademicYearDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(_mapper.Map<List<AcademicYearDto>>(await _unitOfWork.AcademicYears.GetAll()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all academic years list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/academicYears/paginated
        /// <summary>
        /// A method for retrieving a list of paginated academic years
        /// </summary>
        /// <param name="pageNumber">The page number to return</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns></returns>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AcademicYearDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _unitOfWork.AcademicYears.GetPaginatedData(pageNumber ?? 1, pageSize ?? 4);
                var mappedData = _mapper.Map<List<AcademicYearDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<AcademicYearDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated academic years.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/academicYears/5
        /// <summary>
        /// A method for retrieving of academic year record by Id.
        /// </summary>
        /// <param name="id">The academic year Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AcademicYearDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _unitOfWork.AcademicYears.GetById(id);

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<AcademicYearDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the academic year details by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/academicYears
        /// <summary>
        /// A method for creating a academic year record.
        /// </summary>
        /// <param name="model">The academic year record to be created</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AcademicYearDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateAcademicYearDto model)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.AcademicYears.ItemExistsAsync(a=> a.Name == model.Name && a.Status))
                    return Conflict(new { message = $"The academic year - '{model.Name}' already exists" });
                try
                {
                    var _item = _mapper.Map<AcademicYear>(model);
                    _unitOfWork.AcademicYears.Create(_item);
                    await _unitOfWork.SaveChangesAsync();
                    var returnItem = _mapper.Map<AcademicYearDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the academic year.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the academic year - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/academicYears/5
        /// <summary>
        /// A method for updating the academic year record.
        /// </summary>
        /// <param name="model">The academic year record to be updated</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(AcademicYearDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _unitOfWork.AcademicYears.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The academic year of Id- '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var existingItem = await _unitOfWork.AcademicYears.GetById(model.Id);
                    //Manual mapping
                    existingItem.Name = model.Name;
                    existingItem.Abbreviation = model.Abbreviation;
                    existingItem.StartDate = model.StartDate;
                    existingItem.EndDate = model.EndDate;
                    existingItem.Description = model.Description;
                    existingItem.Status = model.Status;
                    _unitOfWork.AcademicYears.Update(existingItem);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the academic year.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the academic year.");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE api/academicYears/5
        /// <summary>
        /// A method for deleting the academic year record by Id.
        /// </summary>
        /// <param name="id">The academic year Id to be deleted</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _unitOfWork.AcademicYears.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The academic year of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _unitOfWork.AcademicYears.GetById(id);
                _unitOfWork.AcademicYears.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the academic year.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the academic year - " + ex.Message);
            }
        }
    }
}
