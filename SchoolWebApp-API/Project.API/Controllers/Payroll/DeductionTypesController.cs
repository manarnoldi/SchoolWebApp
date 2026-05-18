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
    public class DeductionTypesController : ControllerBase
    {
        private readonly ILogger<DeductionTypesController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DeductionTypesController(ILogger<DeductionTypesController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger; _unitOfWork = unitOfWork; _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _unitOfWork.DeductionTypes.Find();
            return Ok(_mapper.Map<List<DeductionTypeDto>>(items));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _unitOfWork.DeductionTypes.GetById(id);
            if (item == null) return NotFound();
            return Ok(_mapper.Map<DeductionTypeDto>(item));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateDeductionTypeDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var item = _mapper.Map<DeductionType>(model);
            _unitOfWork.DeductionTypes.Create(item);
            await _unitOfWork.SaveChangesAsync();
            return Ok(_mapper.Map<DeductionTypeDto>(item));
        }

        [HttpPut]
        public async Task<IActionResult> Update(DeductionTypeDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var exists = await _unitOfWork.DeductionTypes.ItemExistsAsync(a => a.Id == model.Id);
            if (!exists) return NotFound();
            var item = _mapper.Map<DeductionType>(model);
            _unitOfWork.DeductionTypes.Update(item);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _unitOfWork.DeductionTypes.GetById(id);
            if (item == null) return NotFound();
            _unitOfWork.DeductionTypes.Delete(item);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }
    }
}
