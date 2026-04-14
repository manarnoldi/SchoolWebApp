using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs.Settings.GlobalSetting;
using SchoolWebApp.Core.Entities.Settings;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.API.Controllers.Settings
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GlobalSettingsController : ControllerBase
    {
        private readonly ILogger<GlobalSettingsController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GlobalSettingsController(ILogger<GlobalSettingsController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/globalSettings
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var settings = await _unitOfWork.Repository<GlobalSetting>().Find();
                return Ok(_mapper.Map<List<GlobalSettingDto>>(settings));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving global settings.");
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/globalSettings/byModule/{module}
        [HttpGet("byModule/{module}")]
        public async Task<IActionResult> GetByModule(string module)
        {
            try
            {
                var settings = await _unitOfWork.Repository<GlobalSetting>()
                    .Find(s => s.Module == module);
                return Ok(_mapper.Map<List<GlobalSettingDto>>(settings));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving settings by module.");
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/globalSettings/byKey/{module}/{key}
        [HttpGet("byKey/{module}/{key}")]
        public async Task<IActionResult> GetByKey(string module, string key)
        {
            try
            {
                var settings = await _unitOfWork.Repository<GlobalSetting>()
                    .Find(s => s.Module == module && s.SettingKey == key);
                var setting = settings.FirstOrDefault();
                if (setting == null) return NotFound();
                return Ok(_mapper.Map<GlobalSettingDto>(setting));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving setting by key.");
                return StatusCode(500, ex.Message);
            }
        }

        // POST: api/globalSettings
        [HttpPost]
        public async Task<IActionResult> Create(CreateGlobalSettingDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var existing = (await _unitOfWork.Repository<GlobalSetting>()
                    .Find(s => s.Module == model.Module && s.SettingKey == model.SettingKey)).FirstOrDefault();
                if (existing != null)
                    return Conflict(new { message = $"Setting '{model.SettingKey}' for module '{model.Module}' already exists." });

                var item = _mapper.Map<GlobalSetting>(model);
                _unitOfWork.Repository<GlobalSetting>().Create(item);
                await _unitOfWork.SaveChangesAsync();
                return Ok(_mapper.Map<GlobalSettingDto>(item));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating global setting.");
                return StatusCode(500, ex.Message);
            }
        }

        // PUT: api/globalSettings
        [HttpPut]
        public async Task<IActionResult> Update(GlobalSettingDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var item = _mapper.Map<GlobalSetting>(model);
                _unitOfWork.Repository<GlobalSetting>().Update(item);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating global setting.");
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE: api/globalSettings/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var item = await _unitOfWork.Repository<GlobalSetting>().GetById(id);
                if (item == null) return BadRequest("Setting not found.");
                _unitOfWork.Repository<GlobalSetting>().Delete(item);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting global setting.");
                return StatusCode(500, ex.Message);
            }
        }

        // PUT: api/globalSettings/upsert
        [HttpPut("upsert")]
        public async Task<IActionResult> Upsert(CreateGlobalSettingDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var existing = (await _unitOfWork.Repository<GlobalSetting>()
                    .Find(s => s.Module == model.Module && s.SettingKey == model.SettingKey)).FirstOrDefault();

                if (existing != null)
                {
                    existing.SettingValue = model.SettingValue;
                    existing.Description = model.Description;
                    _unitOfWork.Repository<GlobalSetting>().Update(existing);
                }
                else
                {
                    var item = _mapper.Map<GlobalSetting>(model);
                    _unitOfWork.Repository<GlobalSetting>().Create(item);
                }
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error upserting global setting.");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
