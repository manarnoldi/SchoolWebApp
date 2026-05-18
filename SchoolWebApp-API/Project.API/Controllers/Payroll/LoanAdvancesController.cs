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
    public class LoanAdvancesController : ControllerBase
    {
        private readonly ILogger<LoanAdvancesController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LoanAdvancesController(ILogger<LoanAdvancesController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger; _unitOfWork = unitOfWork; _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _unitOfWork.LoanAdvances.Find(includeProperties: "StaffDetails");
            return Ok(_mapper.Map<List<LoanAdvanceDto>>(items));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _unitOfWork.LoanAdvances.GetById(id, includeProperties: "StaffDetails");
            if (item == null) return NotFound();
            return Ok(_mapper.Map<LoanAdvanceDto>(item));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateLoanAdvanceDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var item = _mapper.Map<LoanAdvance>(model);
            _unitOfWork.LoanAdvances.Create(item);
            await _unitOfWork.SaveChangesAsync();
            return Ok(_mapper.Map<LoanAdvanceDto>(item));
        }

        [HttpPut]
        public async Task<IActionResult> Update(LoanAdvanceDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var existing = await _unitOfWork.LoanAdvances.GetById(model.Id);
            if (existing == null) return NotFound();
            _mapper.Map(model, existing);
            _unitOfWork.LoanAdvances.Update(existing);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _unitOfWork.LoanAdvances.GetById(id);
            if (item == null) return NotFound();
            _unitOfWork.LoanAdvances.Delete(item);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }
    }
}
