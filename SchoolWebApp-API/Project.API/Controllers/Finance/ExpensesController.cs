using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs.Finance.Expense;
using SchoolWebApp.Core.DTOs.Finance.ExpenseCategory;
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
    public class ExpenseCategoriesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ExpenseCategoriesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork; _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _unitOfWork.ExpenseCategories.Find(includeProperties: "ExpenseAccount");
            return Ok(_mapper.Map<List<ExpenseCategoryDto>>(items));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _unitOfWork.ExpenseCategories.GetById(id, includeProperties: "ExpenseAccount");
            if (item == null) return NotFound();
            return Ok(_mapper.Map<ExpenseCategoryDto>(item));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateExpenseCategoryDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var item = _mapper.Map<ExpenseCategory>(model);
            _unitOfWork.ExpenseCategories.Create(item);
            await _unitOfWork.SaveChangesAsync();
            return Ok(_mapper.Map<ExpenseCategoryDto>(item));
        }

        [HttpPut]
        public async Task<IActionResult> Update(ExpenseCategoryDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var exists = await _unitOfWork.ExpenseCategories.ItemExistsAsync(a => a.Id == model.Id);
            if (!exists) return NotFound();
            var item = _mapper.Map<ExpenseCategory>(model);
            _unitOfWork.ExpenseCategories.Update(item);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _unitOfWork.ExpenseCategories.GetById(id);
            if (item == null) return NotFound();
            _unitOfWork.ExpenseCategories.Delete(item);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExpensesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ExpensesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork; _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _unitOfWork.Expenses.GetAllWithLines();
            return Ok(_mapper.Map<List<ExpenseDto>>(items));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _unitOfWork.Expenses.GetByIdWithLines(id);
            if (item == null) return NotFound();
            return Ok(_mapper.Map<ExpenseDto>(item));
        }

        [HttpGet("byDateRange")]
        public async Task<IActionResult> ByDateRange(DateTime from, DateTime to)
        {
            var items = await _unitOfWork.Expenses.GetByDateRange(from, to);
            return Ok(_mapper.Map<List<ExpenseDto>>(items));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateExpenseDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var item = _mapper.Map<Expense>(model);
            if (string.IsNullOrEmpty(item.ReferenceNumber))
                item.ReferenceNumber = $"EXP-{DateTime.Now:yyyyMMddHHmmssfff}";
            _unitOfWork.Expenses.Create(item);
            await _unitOfWork.SaveChangesAsync();
            var saved = await _unitOfWork.Expenses.GetByIdWithLines(item.Id);
            return Ok(_mapper.Map<ExpenseDto>(saved));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CreateExpenseDto model)
        {
            var existing = await _unitOfWork.Expenses.GetByIdWithLines(id);
            if (existing == null) return NotFound();
            if (existing.Status == ExpenseStatus.Approved)
                return BadRequest("Cannot edit an approved expense.");
            if (existing.Status == ExpenseStatus.Submitted)
                return BadRequest("Cannot edit a submitted expense. Reject it first.");
            if (await ApprovalLockHelper.IsLockedAsync(_unitOfWork, "Expense", id))
                return BadRequest(new { message = "This expense is locked by an active approval workflow." });
            int.TryParse(User.FindFirstValue("userid"), out var _uid);
            if (await ApprovalLockHelper.IsApproverAsync(_unitOfWork, "Expense", id, _uid))
                return BadRequest(new { message = "Approvers cannot edit or delete submitted items. Use the approval workflow." });

            existing.ExpenseDate = model.ExpenseDate;
            existing.PaymentMethod = (PaymentMethod)model.PaymentMethod;
            existing.TransactionReference = model.TransactionReference;
            existing.PaidFromAccountId = model.PaidFromAccountId;
            existing.Status = (ExpenseStatus)model.Status;
            existing.Description = model.Description;

            // Replace lines
            foreach (var old in existing.Lines.ToList())
                _unitOfWork.ExpenseLines.Delete(old);
            await _unitOfWork.SaveChangesAsync();

            foreach (var l in model.Lines)
            {
                _unitOfWork.ExpenseLines.Create(new ExpenseLine
                {
                    ExpenseId = existing.Id,
                    ExpenseCategoryId = l.ExpenseCategoryId,
                    Amount = l.Amount,
                    Vendor = l.Vendor,
                    BudgetLineId = l.BudgetLineId,
                    Description = l.Description
                });
            }

            _unitOfWork.Expenses.Update(existing);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _unitOfWork.Expenses.GetByIdWithLines(id);
            if (item == null) return NotFound();
            if (item.Status == ExpenseStatus.Approved)
                return BadRequest("Cannot delete an approved expense.");
            if (await ApprovalLockHelper.IsLockedAsync(_unitOfWork, "Expense", id))
                return BadRequest(new { message = "This expense is locked by an active approval workflow." });
            int.TryParse(User.FindFirstValue("userid"), out var _uid);
            if (await ApprovalLockHelper.IsApproverAsync(_unitOfWork, "Expense", id, _uid))
                return BadRequest(new { message = "Approvers cannot edit or delete submitted items. Use the approval workflow." });
            foreach (var l in item.Lines.ToList())
                _unitOfWork.ExpenseLines.Delete(l);
            _unitOfWork.Expenses.Delete(item);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("{id}/submit")]
        public async Task<IActionResult> Submit(int id)
        {
            var item = await _unitOfWork.Expenses.GetByIdWithLines(id);
            if (item == null) return NotFound();
            if (item.Status != ExpenseStatus.Draft && item.Status != ExpenseStatus.Rejected)
                return BadRequest("Only draft or rejected expenses can be submitted.");
            if (!item.Lines.Any())
                return BadRequest("Cannot submit an expense with no lines.");
            item.Status = ExpenseStatus.Submitted;
            _unitOfWork.Expenses.Update(item);
            await _unitOfWork.SaveChangesAsync();
            return Ok(new { message = "Expense submitted for approval." });
        }

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            var item = await _unitOfWork.Expenses.GetByIdWithLines(id);
            if (item == null) return NotFound();
            if (item.Status != ExpenseStatus.Submitted)
                return BadRequest("Only submitted expenses can be approved.");
            item.Status = ExpenseStatus.Approved;
            _unitOfWork.Expenses.Update(item);
            await _unitOfWork.SaveChangesAsync();

            // Auto-post journal: Debit expense accounts, Credit cash/bank
            await AutoPostExpenseJournal(item);

            return Ok(new { message = "Expense approved and posted to GL." });
        }

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> Reject(int id, [FromBody] RejectExpenseDto model)
        {
            var item = await _unitOfWork.Expenses.GetById(id);
            if (item == null) return NotFound();
            if (item.Status != ExpenseStatus.Submitted)
                return BadRequest("Only submitted expenses can be rejected.");
            item.Status = ExpenseStatus.Rejected;
            item.Description = (item.Description ?? "") + $" [Rejected: {model.Reason}]";
            _unitOfWork.Expenses.Update(item);
            await _unitOfWork.SaveChangesAsync();
            return Ok(new { message = "Expense rejected." });
        }

        private async Task<int?> GetSettingAccountId(string key)
        {
            var settings = await _unitOfWork.Repository<SchoolWebApp.Core.Entities.Settings.GlobalSetting>()
                .Find(s => s.Module == "Finance" && s.SettingKey == key);
            var val = settings.FirstOrDefault()?.SettingValue;
            return int.TryParse(val, out var acctId) ? acctId : null;
        }

        private async Task AutoPostExpenseJournal(Expense expense)
        {
            int? cashAccountId = expense.PaidFromAccountId;
            if (cashAccountId == null)
                cashAccountId = await GetSettingAccountId("CashAccountId");
            if (cashAccountId == null) return;

            var journal = new JournalEntry
            {
                ReferenceNumber = $"EXP-JNL-{expense.ReferenceNumber}",
                EntryDate = expense.ExpenseDate,
                Description = $"Auto-posted: Expense {expense.ReferenceNumber}",
                IsPosted = true,
                Status = JournalEntryStatus.Approved,
                Lines = new List<JournalLine>()
            };

            // Credit: Cash/Bank for total
            var totalAmount = expense.Lines.Sum(l => l.Amount);
            journal.Lines.Add(new JournalLine
            {
                AccountId = cashAccountId.Value,
                Debit = 0,
                Credit = totalAmount,
                Description = $"Payment for expense {expense.ReferenceNumber}"
            });

            // Debit: Each line's expense category account
            foreach (var line in expense.Lines)
            {
                var cat = await _unitOfWork.ExpenseCategories.GetById(line.ExpenseCategoryId, "ExpenseAccount");
                if (cat?.ExpenseAccountId != null)
                {
                    // Group by account or add individually
                    journal.Lines.Add(new JournalLine
                    {
                        AccountId = cat.ExpenseAccountId.Value,
                        Debit = line.Amount,
                        Credit = 0,
                        Description = $"{cat.Name} - {line.Vendor ?? ""} {line.Description ?? ""}".Trim()
                    });
                }
            }

            if (journal.Lines.Count > 1) // At least credit + one debit
            {
                _unitOfWork.JournalEntries.Create(journal);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}

public class RejectExpenseDto
{
    public string Reason { get; set; } = string.Empty;
}
