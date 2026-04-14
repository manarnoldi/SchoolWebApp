using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.CBE.Cocurriculum.StudentCoCurriculumScore;
using SchoolWebApp.Core.Entities.CBE.Cocurriculum;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Cocurriculum;

namespace SchoolWebApp.API.Controllers.CBE.Cocurriculum
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StudentCoCurriculumScoresController : ControllerBase
    {
        private readonly ILogger<StudentCoCurriculumScoresController> _logger;
        private readonly IStudentCoCurriculumScoreService _modelSvc;
        private readonly IMapper _mapper;
        public StudentCoCurriculumScoresController(ILogger<StudentCoCurriculumScoresController> logger, IStudentCoCurriculumScoreService service, IMapper mapper)
        {
            _logger = logger;
            _modelSvc = service;
            _mapper = mapper;
        }

        // GET: api/studentcocurriculumscores
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentCoCurriculumScoreDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var items = await _modelSvc.Find(includeProperties: "StudentCoCurriculumActivity,CoCurriculumScore");
                var itemDtos = _mapper.Map<List<StudentCoCurriculumScoreDto>>(items);
                return Ok(itemDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all student co-curriculum scores list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/studentcocurriculumscores/paginated
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentCoCurriculumScoreDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _modelSvc.GetPaginatedData(pageNumber ?? 1, pageSize ?? 10,
                    includeProperties: "StudentCoCurriculumActivity,CoCurriculumScore");
                var mappedData = _mapper.Map<List<StudentCoCurriculumScoreDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<StudentCoCurriculumScoreDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated student co-curriculum scores.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //GET: api/studentcocurriculumscores/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentCoCurriculumScoreDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _modelSvc.GetById(id, includeProperties: "StudentCoCurriculumActivity,CoCurriculumScore");

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<StudentCoCurriculumScoreDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the student co-curriculum score by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //GET: api/studentcocurriculumscores/byStudentCoCurriculumActivityId/5
        [HttpGet("byStudentCoCurriculumActivityId/{studentCoCurriculumActivityId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentCoCurriculumScoreDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByStudentCoCurriculumActivityId(int studentCoCurriculumActivityId)
        {
            try
            {
                var _item = await _modelSvc.GetByStudentCoCurriculumActivityId(studentCoCurriculumActivityId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<StudentCoCurriculumScoreDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the student co-curriculum scores by student co-curriculum activity id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST: api/studentcocurriculumscores
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentCoCurriculumScoreDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateStudentCoCurriculumScoreDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var _item = _mapper.Map<StudentCoCurriculumScore>(model);
                    _modelSvc.Create(_item);
                    await _modelSvc.SaveChangesAsync();
                    var returnItem = _mapper.Map<StudentCoCurriculumScoreDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the student co-curriculum score");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the student co-curriculum score - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT: api/studentcocurriculumscores
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(StudentCoCurriculumScoreDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _modelSvc.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The student co-curriculum score of Id- '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var _item = _mapper.Map<StudentCoCurriculumScore>(model);
                    _modelSvc.Update(_item);
                    await _modelSvc.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the student co-curriculum score - {ex.Message}");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the student co-curriculum score");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE: api/studentcocurriculumscores/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _modelSvc.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The student co-curriculum score of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _modelSvc.GetById(id);
                _modelSvc.Delete(entity);
                await _modelSvc.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the student co-curriculum score.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the student co-curriculum score - " + ex.Message);
            }
        }
    }
}
