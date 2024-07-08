using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.Academics.Grade;
using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.API.Controllers.Academics
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class GradesController : ControllerBase
    {
        private readonly ILogger<GradesController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GradesController(ILogger<GradesController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/grades
        /// <summary>
        /// A method for retrieving all grades
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GradeDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(_mapper.Map<List<GradeDto>>(await _unitOfWork.Grades.Find(includeProperties: "Curriculum")));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all grades list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/grades/paginated
        /// <summary>
        /// A method for retrieving a list of paginated grades
        /// </summary>
        /// <param name="pageNumber">The page number to return</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns></returns>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GradeDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _unitOfWork.Grades.GetPaginatedData(pageNumber ?? 1, pageSize ?? 4);
                var mappedData = _mapper.Map<List<GradeDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<GradeDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated grades.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/grades/byCurriculumId/5
        /// <summary>
        /// A method for retrieving grades by curriculum Id.
        /// </summary>
        /// <param name="id">The curriculum Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("byCurriculumId/{curriculumId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GradeDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGradesByCurriculumId(int curriculumId)
        {
            try
            {
                if (curriculumId <= 0) return BadRequest(curriculumId);
                var _item = await _unitOfWork.Grades.GetByCurriculumId(curriculumId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<GradeDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the grades by curriculum id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/grades/5
        /// <summary>
        /// A method for retrieving a grade record by Id.
        /// </summary>
        /// <param name="id">The grade Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GradeDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _unitOfWork.Grades.GetById(id);

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<GradeDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the grade details by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/grades
        /// <summary>
        /// A method for creating a grade record.
        /// </summary>
        /// <param name="model">The grade record to be created</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GradeDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateGradeDto model)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.Grades.ItemExistsAsync(s => s.Name == model.Name && s.CurriculumId == model.CurriculumId))
                    return Conflict(new { message = $"The grade details submitted already exist." });
                try
                {
                    var _item = _mapper.Map<Grade>(model);
                    _unitOfWork.Grades.Create(_item);
                    await _unitOfWork.SaveChangesAsync();
                    var returnItem = _mapper.Map<GradeDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the grade.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the grade - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/grades/5
        /// <summary>
        /// A method for updating a grade record.
        /// </summary>
        /// <param name="model">The grade record to be updated</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(GradeDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _unitOfWork.Grades.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The grade of Id - '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var _item = _mapper.Map<Grade>(model);
                    _unitOfWork.Grades.Update(_item);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the grade.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the grade.");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE api/grades/5
        /// <summary>
        /// A method for deleting the grade record by Id.
        /// </summary>
        /// <param name="id">The grade Id to be retrieved</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _unitOfWork.Grades.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The grade of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _unitOfWork.Grades.GetById(id);
                _unitOfWork.Grades.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the grade.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the grade - " + ex.Message);
            }
        }
    }
}
