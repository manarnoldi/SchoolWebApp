using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.CBE.Assessments.Strand;
using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Assessments;

namespace SchoolWebApp.API.Controllers.CBE.Assessments
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StrandsController : ControllerBase
    {
        private readonly ILogger<StrandsController> _logger;
        private readonly IStrandService _modelSvc;
        private readonly IMapper _mapper;
        public StrandsController(ILogger<StrandsController> logger, IStrandService service, IMapper mapper)
        {
            _logger = logger;
            _modelSvc = service;
            _mapper = mapper;
        }

        // GET: api/strands
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StrandDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var strands = await _modelSvc.Find(includeProperties: "Subject,LearningLevel,Curriculum,Theme");
                var strandsDtos = _mapper.Map<List<StrandDto>>(strands);
                return Ok(strandsDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all strands list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/strands/paginated
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StrandDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _modelSvc.GetPaginatedData(pageNumber ?? 1, pageSize ?? 10,
                    includeProperties: "Subject,LearningLevel,Curriculum,Theme");
                var mappedData = _mapper.Map<List<StrandDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<StrandDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated strands.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //GET: api/strands/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StrandDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _modelSvc.GetById(id, includeProperties: "Subject,LearningLevel,Curriculum,Theme");

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<StrandDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the strand by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST: api/strands
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StrandDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateStrandDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _modelSvc.ItemExistsAsync(m => m.Name == model.Name && m.LearningLevelId == model.LearningLevelId
                && m.SubjectId == model.SubjectId );

                if (itemExist)
                    return Conflict(new { message = $"The strand name - '{model.Name}' already exists" });
                try
                {
                    var _item = _mapper.Map<Strand>(model);
                    _modelSvc.Create(_item);
                    await _modelSvc.SaveChangesAsync();

                    var returnItem = _mapper.Map<StrandDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the strand");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the strand - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT: api/strands
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(StrandDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _modelSvc.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The strand of Id- '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var _item = _mapper.Map<Strand>(model);
                    _modelSvc.Update(_item);
                    await _modelSvc.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the strand - {ex.Message}");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the strand");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE: api/strands/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _modelSvc.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The strand of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _modelSvc.GetById(id);
                _modelSvc.Delete(entity);
                await _modelSvc.SaveChangesAsync();
                return Ok();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "FK constraint error while deleting strand.");
                return Conflict(new { message = "Cannot delete this strand because it has child records (sub-strands, assessments, etc.). Delete them first." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the strand.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the strand - " + ex.Message);
            }
        }

        //GET api/strands/bySubjectId?subjectId=1&learningLvlId=1&academicYearId=1
        [HttpGet("bySubjectId")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StrandDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBySubjectId(int? subjectId, int? learningLvlId, int? academicYearId)
        {
            try
            {
                var _item = await _modelSvc.GetBySubjectId(subjectId, learningLvlId, academicYearId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<StrandDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the strands by subject id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
