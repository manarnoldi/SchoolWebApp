using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.Class.SchoolClass;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.Staff;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.API.Controllers.Class
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolClassesController : ControllerBase
    {
        private readonly ILogger<SchoolClassesController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public SchoolClassesController(ILogger<SchoolClassesController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/schoolClasses
        /// <summary>
        /// A method for retrieving all school classes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SchoolClassDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(_mapper.Map<List<SchoolClassDto>>(await _unitOfWork.SchoolClasses.Find(includeProperties: "LearningLevel,SchoolStream,AcademicYear")));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all school classes list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/schoolClasses/paginated
        /// <summary>
        /// A method for retrieving a list of paginated school classes
        /// </summary>
        /// <param name="pageNumber">The page number to return</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns></returns>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SchoolClassDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _unitOfWork.SchoolClasses.GetPaginatedData(pageNumber ?? 1, pageSize ?? 4);
                var mappedData = _mapper.Map<List<SchoolClassDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<SchoolClassDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated school classes.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/schoolClasses/byAcademicYearId/5
        /// <summary>
        /// A method for retrieving school classes by academic Id.
        /// </summary>
        /// <param name="id">The academic year Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("byAcademicYearId/{academicYearId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SchoolClassDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByAcademicYearId(int academicYearId)
        {
            try
            {
                if (academicYearId <= 0) return BadRequest(academicYearId);
                var _item = await _unitOfWork.SchoolClasses.GetByAcademicYearId(academicYearId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<SchoolClassDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the school classes by academic year id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/schoolClasses/byYearClassStream
        /// <summary>
        /// A method for retrieving school classes by year class stream combination.
        /// </summary>
        /// <param name="id">The academic year Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("byYearClassStream")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SchoolClassDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByYearClassStream(int academicYearId, int learningLevelId, int schoolStreamId)
        {
            try
            {
                if (academicYearId <= 0 || learningLevelId <= 0 || schoolStreamId <= 0) return BadRequest(academicYearId);
                var _item = await _unitOfWork.SchoolClasses.GetByYearClassStream(academicYearId, learningLevelId, schoolStreamId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<SchoolClassDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the school classes by year, class, stream.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/schoolClasses/5
        /// <summary>
        /// A method for retrieving a school class record by Id.
        /// </summary>
        /// <param name="id">The school class Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SchoolClassDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _unitOfWork.SchoolClasses.GetById(id, includeProperties: "LearningLevel,SchoolStream,AcademicYear");

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<SchoolClassDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the school class details by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/schoolClasses
        /// <summary>
        /// A method for creating a school class record.
        /// </summary>
        /// <param name="model">The school class record to be created</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SchoolClassDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateSchoolClassDto model)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.SchoolClasses.ItemExistsAsync(s => s.Name == model.Name && s.AcademicYearId == model.AcademicYearId &&
                s.SchoolStreamId == model.SchoolStreamId && s.LearningLevelId == model.LearningLevelId))
                    return Conflict(new { message = $"The school class details submitted already exist." });
                try
                {
                    var _item = _mapper.Map<SchoolClass>(model);
                    _unitOfWork.SchoolClasses.Create(_item);
                    await _unitOfWork.SaveChangesAsync();
                    var returnItem = _mapper.Map<SchoolClassDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the school class.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the school class - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/schoolClasses/5
        /// <summary>
        /// A method for updating a school class record.
        /// </summary>
        /// <param name="model">The school class record to be updated</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(SchoolClassDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _unitOfWork.SchoolClasses.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The school class of Id - '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var _item = _mapper.Map<SchoolClass>(model);
                    _unitOfWork.SchoolClasses.Update(_item);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the school class.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the school class.");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE api/schoolClasses/5
        /// <summary>
        /// A method for deleting the school class record by Id.
        /// </summary>
        /// <param name="id">The school class Id to be retrieved</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _unitOfWork.SchoolClasses.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The school class of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _unitOfWork.SchoolClasses.GetById(id);
                _unitOfWork.SchoolClasses.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the school class.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the school class - " + ex.Message);
            }
        }
    }
}