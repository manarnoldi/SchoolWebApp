using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.CBE.Values.StudentValueScore;
using SchoolWebApp.Core.Entities.CBE.Values;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Values;

namespace SchoolWebApp.API.Controllers.CBE.Values
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StudentValueScoresController : ControllerBase
    {
        private readonly ILogger<StudentValueScoresController> _logger;
        private readonly IStudentValueScoreService _modelSvc;
        private readonly IMapper _mapper;
        public StudentValueScoresController(ILogger<StudentValueScoresController> logger, IStudentValueScoreService service, IMapper mapper)
        {
            _logger = logger;
            _modelSvc = service;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentValueScoreDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var items = await _modelSvc.Find(includeProperties: "Value,ValueScore,Student,Session");
                var itemDtos = _mapper.Map<List<StudentValueScoreDto>>(items);
                return Ok(itemDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all student value scores list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentValueScoreDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _modelSvc.GetPaginatedData(pageNumber ?? 1, pageSize ?? 10,
                    includeProperties: "Value,ValueScore,Student,Session");
                var mappedData = _mapper.Map<List<StudentValueScoreDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<StudentValueScoreDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated student value scores.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentValueScoreDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _modelSvc.GetById(id, includeProperties: "Value,ValueScore,Student,Session");
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<StudentValueScoreDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the student value score by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("byStudentId/{studentId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentValueScoreDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByStudentId(int studentId)
        {
            try
            {
                var _item = await _modelSvc.GetByStudentId(studentId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<StudentValueScoreDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the student value scores by student id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("bySessionId/{sessionId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentValueScoreDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBySessionId(int sessionId)
        {
            try
            {
                var _item = await _modelSvc.GetBySessionId(sessionId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<StudentValueScoreDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the student value scores by session id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentValueScoreDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateStudentValueScoreDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var _item = _mapper.Map<StudentValueScore>(model);
                    _modelSvc.Create(_item);
                    await _modelSvc.SaveChangesAsync();
                    var returnItem = _mapper.Map<StudentValueScoreDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the student value score");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the student value score - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // POST api/studentValueScores/batch
        /// <summary>
        /// Upserts a batch of student value scores in a single request. Rows
        /// with Id &gt; 0 are updated; rows with Id &lt;= 0 are created. Rows
        /// omitted from the payload are left untouched (this is NOT a full-set
        /// replace), so the client can send only the records that were added or
        /// changed instead of the whole grid.
        /// </summary>
        [HttpPost("batch")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateMany(List<StudentValueScoreDto> model)
        {
            if (model == null || !model.Any())
                return BadRequest("No student value scores provided.");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                foreach (var item in model)
                {
                    var _item = _mapper.Map<StudentValueScore>(item);
                    if (item.Id > 0)
                        _modelSvc.Update(_item);
                    else
                        _modelSvc.Create(_item);
                }
                await _modelSvc.SaveChangesAsync();
                return Ok("Student value scores saved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the batch of student value scores.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(StudentValueScoreDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _modelSvc.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The student value score of Id- '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var _item = _mapper.Map<StudentValueScore>(model);
                    _modelSvc.Update(_item);
                    await _modelSvc.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the student value score - {ex.Message}");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the student value score");
                }
            }
            return BadRequest(ModelState);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _modelSvc.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The student value score of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _modelSvc.GetById(id);
                _modelSvc.Delete(entity);
                await _modelSvc.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the student value score.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the student value score - " + ex.Message);
            }
        }
    }
}
