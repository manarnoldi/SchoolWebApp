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
    public class EarningTypesController : ControllerBase
    {
        private readonly ILogger<EarningTypesController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EarningTypesController(ILogger<EarningTypesController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger; _unitOfWork = unitOfWork; _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _unitOfWork.EarningTypes.Find();
            return Ok(_mapper.Map<List<EarningTypeDto>>(items));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _unitOfWork.EarningTypes.GetById(id);
            if (item == null) return NotFound();
            return Ok(_mapper.Map<EarningTypeDto>(item));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateEarningTypeDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var item = _mapper.Map<EarningType>(model);
            _unitOfWork.EarningTypes.Create(item);
            await _unitOfWork.SaveChangesAsync();
            return Ok(_mapper.Map<EarningTypeDto>(item));
        }

        [HttpPut]
        public async Task<IActionResult> Update(EarningTypeDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var exists = await _unitOfWork.EarningTypes.ItemExistsAsync(a => a.Id == model.Id);
            if (!exists) return NotFound();
            var item = _mapper.Map<EarningType>(model);
            _unitOfWork.EarningTypes.Update(item);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _unitOfWork.EarningTypes.GetById(id);
            if (item == null) return NotFound();
            _unitOfWork.EarningTypes.Delete(item);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }
    }
}
