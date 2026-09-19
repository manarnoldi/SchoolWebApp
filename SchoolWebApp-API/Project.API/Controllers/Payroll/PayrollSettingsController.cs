using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs.Payroll;
using SchoolWebApp.Core.Entities.Payroll;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.API.Controllers.Payroll
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PayrollSettingsController : ControllerBase
    {
        private readonly ILogger<PayrollSettingsController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PayrollSettingsController(ILogger<PayrollSettingsController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger; _unitOfWork = unitOfWork; _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _unitOfWork.PayrollSettings.Find();
            return Ok(_mapper.Map<List<PayrollSettingDto>>(items));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _unitOfWork.PayrollSettings.GetById(id);
            if (item == null) return NotFound();
            return Ok(_mapper.Map<PayrollSettingDto>(item));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePayrollSettingDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var item = _mapper.Map<PayrollSetting>(model);
            _unitOfWork.PayrollSettings.Create(item);
            await _unitOfWork.SaveChangesAsync();
            return Ok(_mapper.Map<PayrollSettingDto>(item));
        }

        [HttpPut]
        public async Task<IActionResult> Update(PayrollSettingDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var existing = await _unitOfWork.PayrollSettings.GetById(model.Id);
            if (existing == null) return NotFound();

            // Copy the edit onto the record loaded from the database. Mapping the DTO to
            // a new entity instead lost the Id - the only map from a DTO is the create
            // map, which has none - so EF Core INSERTed a second row with the same key
            // rather than updating this one, and payroll would read whichever it found
            // first.
            _mapper.Map<CreatePayrollSettingDto, PayrollSetting>(model, existing);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _unitOfWork.PayrollSettings.GetById(id);
            if (item == null) return NotFound();
            _unitOfWork.PayrollSettings.Delete(item);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }
    }
}
