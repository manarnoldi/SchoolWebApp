using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs.CBE.Assessments.Theme;
using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Assessments;

namespace SchoolWebApp.API.Controllers.CBE.Assessments
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ThemesController : ControllerBase
    {
        private readonly ILogger<ThemesController> _logger;
        private readonly IThemeService _modelSvc;
        private readonly IMapper _mapper;
        public ThemesController(ILogger<ThemesController> logger, IThemeService service, IMapper mapper)
        {
            _logger = logger;
            _modelSvc = service;
            _mapper = mapper;
        }

        // GET: api/themes
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ThemeDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var _list = await _modelSvc.Find(includeProperties: "Subject,LearningLevel,Curriculum");
                var _listDto = _mapper.Map<List<ThemeDto>>(_list);
                return Ok(_listDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving themes list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/themes/bySubjectId?subjectId=1&learningLvlId=2
        [HttpGet("bySubjectId")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ThemeDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBySubjectId([FromQuery] int? subjectId, [FromQuery] int? learningLvlId)
        {
            try
            {
                var _list = await _modelSvc.GetBySubjectId(subjectId, learningLvlId);
                var _listDto = _mapper.Map<List<ThemeDto>>(_list);
                return Ok(_listDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving themes by subject id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/themes/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ThemeDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _modelSvc.GetById(id, includeProperties: "Subject,LearningLevel,Curriculum");
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<ThemeDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving theme by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/themes
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateThemeDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _modelSvc.ItemExistsAsync(m => m.Name == model.Name
                    && m.LearningLevelId == model.LearningLevelId
                    && m.SubjectId == model.SubjectId);
                if (itemExist)
                    return Conflict(new { message = $"The theme '{model.Name}' already exists" });
                try
                {
                    var _item = _mapper.Map<Theme>(model);
                    _modelSvc.Create(_item);
                    await _modelSvc.SaveChangesAsync();
                    var returnItem = _mapper.Map<ThemeDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while adding the theme.");
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/themes
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(ThemeDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _modelSvc.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return NotFound(new { message = $"Theme with Id '{model.Id}' not found" });
                try
                {
                    var _item = _mapper.Map<Theme>(model);
                    _modelSvc.Update(_item);
                    await _modelSvc.SaveChangesAsync();
                    var returnItem = _mapper.Map<ThemeDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while updating the theme.");
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE api/themes/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _modelSvc.GetById(id);
                if (itemExists == null)
                    return NotFound(new { message = $"Theme with Id '{id}' not found" });
                var entity = await _modelSvc.GetById(id);
                _modelSvc.Delete(entity);
                await _modelSvc.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the theme.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
