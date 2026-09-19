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
    public class NssfBandsController : ControllerBase
    {
        private readonly ILogger<NssfBandsController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public NssfBandsController(ILogger<NssfBandsController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger; _unitOfWork = unitOfWork; _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = (await _unitOfWork.Repository<NssfBand>().Find())
                .OrderByDescending(b => b.EffectiveDate).ThenBy(b => b.Tier).ToList();
            return Ok(_mapper.Map<List<NssfBandDto>>(items));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _unitOfWork.Repository<NssfBand>().GetById(id);
            if (item == null) return NotFound();
            return Ok(_mapper.Map<NssfBandDto>(item));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateNssfBandDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (model.UpperLimit <= model.LowerLimit)
                return BadRequest(new { message = "The upper limit must be above the lower limit." });
            var item = _mapper.Map<NssfBand>(model);
            _unitOfWork.Repository<NssfBand>().Create(item);
            await _unitOfWork.SaveChangesAsync();
            return Ok(_mapper.Map<NssfBandDto>(item));
        }

        [HttpPut]
        public async Task<IActionResult> Update(NssfBandDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var existing = await _unitOfWork.Repository<NssfBand>().GetById(model.Id);
            if (existing == null) return NotFound();
            if (model.UpperLimit <= model.LowerLimit)
                return BadRequest(new { message = "The upper limit must be above the lower limit." });

            existing.Name = model.Name;
            existing.Tier = model.Tier;
            existing.LowerLimit = model.LowerLimit;
            existing.UpperLimit = model.UpperLimit;
            existing.Rate = model.Rate;
            existing.EffectiveDate = model.EffectiveDate;
            existing.IsActive = model.IsActive;
            existing.Description = model.Description;

            _unitOfWork.Repository<NssfBand>().Update(existing);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _unitOfWork.Repository<NssfBand>().GetById(id);
            if (item == null) return NotFound();
            _unitOfWork.Repository<NssfBand>().Delete(item);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }
    }
}
