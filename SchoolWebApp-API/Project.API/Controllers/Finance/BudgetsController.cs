using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs.Finance.Budget;
using SchoolWebApp.Core.Entities.Approvals;
using SchoolWebApp.Core.Entities.Finance;
using SchoolWebApp.Core.Helpers;
using SchoolWebApp.Core.Interfaces.IRepositories;
using System.Security.Claims;

namespace SchoolWebApp.API.Controllers.Finance
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BudgetsController : ControllerBase
    {
        private readonly ILogger<BudgetsController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BudgetsController(ILogger<BudgetsController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger; _unitOfWork = unitOfWork; _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _unitOfWork.Budgets.GetAllWithLines();
            return Ok(_mapper.Map<List<BudgetDto>>(items));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var budget = await _unitOfWork.Budgets.GetByIdWithLines(id);
            if (budget == null) return NotFound();
            var dto = _mapper.Map<BudgetDto>(budget);

            // Compute actuals from posted journal lines within budget date range, by account
            var lines = await _unitOfWork.JournalLines.GetAllWithEntryAndAccount(budget.StartDate, budget.EndDate);
            var actualsByAccount = lines
                .GroupBy(l => l.AccountId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(l => (l.Account != null && (l.Account.AccountType == AccountType.Income || l.Account.AccountType == AccountType.Liability || l.Account.AccountType == AccountType.Equity))
                        ? l.Credit - l.Debit
                        : l.Debit - l.Credit)
                );

            // Amendments — apply only Approved ones to produce effective amounts
            var approvedAmendments = await _unitOfWork.BudgetAmendments.GetByBudgetId(id);
            var approved = approvedAmendments.Where(a => a.Status == BudgetAmendmentStatus.Approved).ToList();
            var amendByAccount = approved
                .SelectMany(a => a.Lines)
                .GroupBy(l => l.AccountId)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Delta));

            decimal totalAmendments = 0;
            foreach (var line in dto.Lines)
            {
                var amend = amendByAccount.TryGetValue(line.AccountId, out var dv) ? dv : 0;
                line.AmendedAmount = amend;
                line.EffectiveAmount = line.BudgetedAmount + amend;
                line.ActualAmount = actualsByAccount.TryGetValue(line.AccountId, out var v) ? v : 0;
                line.Variance = line.EffectiveAmount - line.ActualAmount;
                totalAmendments += amend;
            }
            dto.TotalAmendments = totalAmendments;
            dto.TotalEffective = dto.TotalBudgeted + totalAmendments;
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBudgetDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var item = _mapper.Map<Budget>(model);
            // New budgets are inactive until the approval workflow finalizes.
            item.IsActive = false;
            _unitOfWork.Budgets.Create(item);
            await _unitOfWork.SaveChangesAsync();
            return Ok(_mapper.Map<BudgetDto>(item));
        }

        [HttpPut]
        public async Task<IActionResult> Update(BudgetDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var existing = await _unitOfWork.Budgets.GetByIdWithLines(model.Id);
            if (existing == null) return NotFound();
            if (await ApprovalLockHelper.IsLockedAsync(_unitOfWork, "Budget", model.Id))
                return BadRequest(new { message = "This budget is locked by an active approval workflow." });
            int.TryParse(User.FindFirstValue("userid"), out var _uidUpd);
            if (await ApprovalLockHelper.IsApproverAsync(_unitOfWork, "Budget", model.Id, _uidUpd))
                return BadRequest(new { message = "Approvers cannot edit or delete submitted items. Use the approval workflow." });

            // Update header
            existing.Name = model.Name;
            existing.Description = model.Description;
            existing.StartDate = model.StartDate;
            existing.EndDate = model.EndDate;
            existing.AcademicYearId = model.AcademicYearId;
            existing.DepartmentId = model.DepartmentId;
            existing.BudgetMasterId = model.BudgetMasterId;
            existing.IsActive = model.IsActive;
            _unitOfWork.Budgets.Update(existing);

            // Replace lines
            foreach (var oldLine in existing.Lines.ToList())
                _unitOfWork.BudgetLines.Delete(oldLine);
            await _unitOfWork.SaveChangesAsync();

            foreach (var l in model.Lines)
            {
                var newLine = new BudgetLine
                {
                    BudgetId = existing.Id,
                    AccountId = l.AccountId,
                    BudgetedAmount = l.BudgetedAmount,
                    Notes = l.Notes
                };
                _unitOfWork.BudgetLines.Create(newLine);
            }
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _unitOfWork.Budgets.GetByIdWithLines(id);
            if (item == null) return NotFound();
            if (await ApprovalLockHelper.IsLockedAsync(_unitOfWork, "Budget", id))
                return BadRequest(new { message = "This budget is locked by an active approval workflow." });
            int.TryParse(User.FindFirstValue("userid"), out var _uidDel);
            if (await ApprovalLockHelper.IsApproverAsync(_unitOfWork, "Budget", id, _uidDel))
                return BadRequest(new { message = "Approvers cannot edit or delete submitted items. Use the approval workflow." });
            foreach (var l in item.Lines.ToList())
                _unitOfWork.BudgetLines.Delete(l);
            _unitOfWork.Budgets.Delete(item);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }
    }
}
