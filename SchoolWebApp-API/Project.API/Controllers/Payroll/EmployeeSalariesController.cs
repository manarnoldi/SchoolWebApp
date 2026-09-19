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

        // POST api/employeeSalaries/batch
        /// <summary>
        /// Upserts a batch of employee salary headers in a single request. Each row is
        /// matched on StaffDetailsId: a staff member who already has a salary gets it
        /// updated, one without gets a new record created. Salary line items (additional
        /// earnings and voluntary deductions) are NOT touched here - they are edited per
        /// employee through the single-record endpoints - so a batch save never discards
        /// them. Rows omitted from the payload are left untouched (this is not a full-set
        /// replace), so the client sends only the rows the user actually changed.
        /// </summary>
        [HttpPost("batch")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SaveBatch(List<CreateEmployeeSalaryDto> model)
        {
            if (model == null || !model.Any()) return BadRequest("No employee salaries provided.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var staffIds = model.Select(m => m.StaffDetailsId).Distinct().ToList();
            if (staffIds.Count != model.Count)
                return BadRequest("The batch contains more than one salary for the same staff member.");

            try
            {
                // One query covering every staff member in the batch, so the upsert
                // decision per row is an in-memory lookup rather than a round trip.
                // Where a staff member somehow has more than one salary row, the most
                // recent one wins - the same record the grid shows.
                var existing = (await _unitOfWork.EmployeeSalaries
                        .Find(s => staffIds.Contains(s.StaffDetailsId)))
                    .GroupBy(s => s.StaffDetailsId)
                    .ToDictionary(g => g.Key, g => g.OrderByDescending(s => s.EffectiveDate).First());

                var inserted = 0;
                var updated = 0;

                foreach (var row in model)
                {
                    if (existing.TryGetValue(row.StaffDetailsId, out var salary))
                    {
                        salary.BasicSalary = row.BasicSalary;
                        salary.HouseAllowance = row.HouseAllowance;
                        salary.TransportAllowance = row.TransportAllowance;
                        salary.OtherAllowances = row.OtherAllowances;
                        salary.EffectiveDate = row.EffectiveDate;
                        salary.IsActive = row.IsActive;
                        salary.Notes = row.Notes;
                        _unitOfWork.EmployeeSalaries.Update(salary);
                        updated++;
                    }
                    else
                    {
                        var newSalary = _mapper.Map<EmployeeSalary>(row);
                        newSalary.Items = new List<EmployeeSalaryItem>();
                        _unitOfWork.EmployeeSalaries.Create(newSalary);
                        inserted++;
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                return Ok(new { inserted, updated });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the batch of employee salaries.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
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
