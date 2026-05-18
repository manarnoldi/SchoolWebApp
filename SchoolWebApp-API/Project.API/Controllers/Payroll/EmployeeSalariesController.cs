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
    public class EmployeeSalariesController : ControllerBase
    {
        private readonly ILogger<EmployeeSalariesController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EmployeeSalariesController(ILogger<EmployeeSalariesController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger; _unitOfWork = unitOfWork; _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _unitOfWork.EmployeeSalaries.GetAllWithStaff();
            return Ok(_mapper.Map<List<EmployeeSalaryDto>>(items));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _unitOfWork.EmployeeSalaries.GetById(id, includeProperties: "StaffDetails,Items.EarningType,Items.DeductionType");
            if (item == null) return NotFound();
            return Ok(_mapper.Map<EmployeeSalaryDto>(item));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateEmployeeSalaryDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var item = _mapper.Map<EmployeeSalary>(model);
            _unitOfWork.EmployeeSalaries.Create(item);
            await _unitOfWork.SaveChangesAsync();
            return Ok(_mapper.Map<EmployeeSalaryDto>(item));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CreateEmployeeSalaryDto model)
        {
            var existing = await _unitOfWork.EmployeeSalaries.GetById(id, includeProperties: "Items");
            if (existing == null) return NotFound();

            existing.StaffDetailsId = model.StaffDetailsId;
            existing.BasicSalary = model.BasicSalary;
            existing.HouseAllowance = model.HouseAllowance;
            existing.TransportAllowance = model.TransportAllowance;
            existing.OtherAllowances = model.OtherAllowances;
            existing.EffectiveDate = model.EffectiveDate;
            existing.IsActive = model.IsActive;
            existing.Notes = model.Notes;

            // Remove old items
            foreach (var oldItem in existing.Items.ToList())
                _unitOfWork.EmployeeSalaryItems.Delete(oldItem);
            await _unitOfWork.SaveChangesAsync();

            // Add new items
            foreach (var item in model.Items)
            {
                var newItem = new EmployeeSalaryItem
                {
                    EmployeeSalaryId = existing.Id,
                    EarningTypeId = item.EarningTypeId,
                    DeductionTypeId = item.DeductionTypeId,
                    Amount = item.Amount
                };
                _unitOfWork.EmployeeSalaryItems.Create(newItem);
            }

            _unitOfWork.EmployeeSalaries.Update(existing);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _unitOfWork.EmployeeSalaries.GetById(id);
            if (item == null) return NotFound();
            _unitOfWork.EmployeeSalaries.Delete(item);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }
    }
}
