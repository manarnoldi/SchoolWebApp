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
    public class PayrollReliefsController : ControllerBase
    {
        private readonly ILogger<PayrollReliefsController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PayrollReliefsController(ILogger<PayrollReliefsController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger; _unitOfWork = unitOfWork; _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = (await _unitOfWork.Repository<PayrollRelief>()
                    .Find(includeProperties: "DeductionType"))
                .OrderBy(r => r.Name).ToList();
            return Ok(_mapper.Map<List<PayrollReliefDto>>(items));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _unitOfWork.Repository<PayrollRelief>()
                .GetById(id, includeProperties: "DeductionType");
            if (item == null) return NotFound();
            return Ok(_mapper.Map<PayrollReliefDto>(item));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePayrollReliefDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var invalid = Validate(model);
            if (invalid != null) return BadRequest(new { message = invalid });
            if (await _unitOfWork.Repository<PayrollRelief>().ItemExistsAsync(r => r.Code == model.Code))
                return Conflict(new { message = $"A relief with the code '{model.Code}' already exists." });

            var item = _mapper.Map<PayrollRelief>(model);
            _unitOfWork.Repository<PayrollRelief>().Create(item);
            await _unitOfWork.SaveChangesAsync();
            return Ok(_mapper.Map<PayrollReliefDto>(item));
        }

        [HttpPut]
        public async Task<IActionResult> Update(PayrollReliefDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var invalid = Validate(model);
            if (invalid != null) return BadRequest(new { message = invalid });

            var existing = await _unitOfWork.Repository<PayrollRelief>().GetById(model.Id);
            if (existing == null) return NotFound();
            if (await _unitOfWork.Repository<PayrollRelief>()
                    .ItemExistsAsync(r => r.Code == model.Code && r.Id != model.Id))
                return Conflict(new { message = $"Another relief already uses the code '{model.Code}'." });

            existing.Name = model.Name;
            existing.Code = model.Code;
            existing.Basis = (ReliefBasis)model.Basis;
            existing.DeductionTypeId = model.DeductionTypeId;
            existing.Value = model.Value;
            existing.MonthlyCap = model.MonthlyCap;
            existing.AppliesToAll = model.AppliesToAll;
            existing.IsActive = model.IsActive;
            existing.Description = model.Description;

            _unitOfWork.Repository<PayrollRelief>().Update(existing);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _unitOfWork.Repository<PayrollRelief>().GetById(id);
            if (item == null) return NotFound();
            _unitOfWork.Repository<PayrollRelief>().Delete(item);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// A percentage relief has nothing to compute from without a deduction, so
        /// it would silently relieve nothing. Reject it rather than let it look set
        /// up but do nothing on the payroll run.
        /// </summary>
        private static string? Validate(CreatePayrollReliefDto model)
        {
            if (model.Value <= 0)
                return "The relief value must be greater than zero.";
            if ((ReliefBasis)model.Basis == ReliefBasis.PercentageOfDeduction && model.DeductionTypeId == null)
                return "A percentage relief must say which deduction it is worked out from.";
            if (model.MonthlyCap.HasValue && model.MonthlyCap.Value < 0)
                return "The monthly cap cannot be negative.";
            return null;
        }
    }
}
