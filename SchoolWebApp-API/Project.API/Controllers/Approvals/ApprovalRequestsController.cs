using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.DTOs.Approvals;
using SchoolWebApp.Core.Entities.Approvals;
using SchoolWebApp.Core.Entities.Finance;
using SchoolWebApp.Core.Entities.Identity;
using SchoolWebApp.Core.Entities.Settings;
using System.Security.Claims;

namespace SchoolWebApp.API.Controllers.Approvals
{
    [Authorize]
    [Route("api/approvalRequests")]
    [ApiController]
    public class ApprovalRequestsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<ApprovalRequestsController> _logger;

        public ApprovalRequestsController(ApplicationDbContext db, IMapper mapper,
            UserManager<AppUser> userManager, ILogger<ApprovalRequestsController> logger)
        {
            _db = db;
            _mapper = mapper;
            _userManager = userManager;
            _logger = logger;
        }

        private async Task<AppUser?> GetCurrentUserAsync()
        {
            var username = User.FindFirstValue("username") ?? User.Identity?.Name;
            if (string.IsNullOrEmpty(username)) return null;
            return await _userManager.FindByNameAsync(username);
        }

        // GET api/approvalRequests/myPending — approvals where the current user is the assignee of the current step
        [HttpGet("myPending")]
        public async Task<IActionResult> GetMyPending()
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null) return Unauthorized();

            var pending = await _db.ApprovalRequests
                .Include(r => r.Actions)
                .Include(r => r.SubmittedBy)
                .Include(r => r.ApprovalWorkflow)
                .Where(r => r.Status == ApprovalRequestStatus.Submitted
                    && r.Actions.Any(a => a.StepRank == r.CurrentStepRank && a.AssignedToUserId == currentUser.Id))
                .OrderByDescending(r => r.SubmittedAt)
                .Select(r => new
                {
                    id = r.Id,
                    entityType = r.EntityType,
                    entityId = r.EntityId,
                    currentStepRank = r.CurrentStepRank,
                    stepName = r.Actions.Where(a => a.StepRank == r.CurrentStepRank).Select(a => a.StepName).FirstOrDefault(),
                    workflowName = r.ApprovalWorkflow!.Name,
                    submittedById = r.SubmittedById,
                    submittedByName = r.SubmittedBy != null ? (r.SubmittedBy.FirstName + " " + r.SubmittedBy.LastName).Trim() : null,
                    submittedAt = r.SubmittedAt
                })
                .ToListAsync();

            return Ok(pending);
        }

        // GET api/approvalRequests/statuses?entityType=Expense
        [HttpGet("statuses")]
        public async Task<IActionResult> GetStatusesByEntityType(string entityType)
        {
            var currentUser = await GetCurrentUserAsync();
            var currentUserId = currentUser?.Id ?? 0;

            // Load the active edit-lock policy once.
            var policy = (await _db.GlobalSettings
                .Where(s => s.Module == "General" && s.SettingKey == "ApprovalEditLockPolicy")
                .Select(s => s.SettingValue)
                .FirstOrDefaultAsync()) ?? "Strict";
            bool lenient = string.Equals(policy, "AfterFirstApproval", StringComparison.OrdinalIgnoreCase);

            var requests = await _db.ApprovalRequests
                .Include(r => r.Actions)
                .Where(r => r.EntityType == entityType)
                .ToListAsync();

            var result = requests.Select(r => new
            {
                entityId = r.EntityId,
                status = (int)r.Status,
                currentStepRank = r.CurrentStepRank,
                currentAssigneeUserId = r.Actions
                    .Where(a => a.StepRank == r.CurrentStepRank)
                    .Select(a => (int?)a.AssignedToUserId)
                    .FirstOrDefault(),
                submittedById = r.SubmittedById,
                // True only when the workflow is currently active (Submitted/Approved) AND the logged-in user
                // is assigned to any approval step. Historical approvers on Returned/Rejected/Reversed requests
                // should not stay blocked, because the submitter will normally edit/resubmit next.
                isApproverForMe = currentUserId > 0
                    && (r.Status == ApprovalRequestStatus.Submitted || r.Status == ApprovalRequestStatus.Approved)
                    && r.Actions.Any(a => a.AssignedToUserId == currentUserId),
                isLocked = r.Status == ApprovalRequestStatus.Approved
                    || (r.Status == ApprovalRequestStatus.Submitted
                        && (!lenient || r.Actions.Any(a => a.Status == StepActionStatus.Approved)))
            }).ToList();

            return Ok(result);
        }

        // GET api/approvalRequests/for?entityType=Expense&entityId=5
        [HttpGet("for")]
        public async Task<IActionResult> GetForEntity(string entityType, int entityId)
        {
            var req = await _db.ApprovalRequests
                .Include(r => r.ApprovalWorkflow)
                    .ThenInclude(w => w!.Steps.OrderBy(s => s.Rank))
                        .ThenInclude(s => s.Role)
                .Include(r => r.SubmittedBy)
                .Include(r => r.Actions.OrderBy(a => a.StepRank))
                    .ThenInclude(a => a.AssignedTo)
                .Include(r => r.Actions)
                    .ThenInclude(a => a.ActionedBy)
                .FirstOrDefaultAsync(r => r.EntityType == entityType && r.EntityId == entityId);
            if (req == null) return Ok(null);
            return Ok(_mapper.Map<ApprovalRequestDto>(req));
        }

        // POST api/approvalRequests/submit
        [HttpPost("submit")]
        public async Task<IActionResult> Submit([FromBody] SubmitApprovalDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null) return Unauthorized();

            var wf = await _db.ApprovalWorkflows
                .Include(w => w.Steps.OrderBy(s => s.Rank))
                .FirstOrDefaultAsync(w => w.FormKey == model.FormKey && w.IsActive);
            if (wf == null) return BadRequest(new { message = $"No active workflow for form '{model.FormKey}'." });

            if (wf.Steps.Count == 0)
                return BadRequest(new { message = "Workflow has no steps configured." });

            // Validate step assignments match workflow steps
            var assignmentByRank = model.StepAssignments.ToDictionary(a => a.StepRank, a => a.AssignedToUserId);
            foreach (var s in wf.Steps)
            {
                if (!assignmentByRank.ContainsKey(s.Rank))
                    return BadRequest(new { message = $"Missing assignment for step '{s.Name}' (rank {s.Rank})." });
                if (assignmentByRank[s.Rank] <= 0)
                    return BadRequest(new { message = $"Please select an approver for step '{s.Name}'." });
            }

            // Maker-checker: submitter cannot approve their own request
            if (wf.IsMakerChecker)
            {
                foreach (var s in wf.Steps)
                {
                    if (assignmentByRank[s.Rank] == currentUser.Id)
                        return BadRequest(new { message = $"Maker-checker enabled: you cannot assign yourself as approver on step '{s.Name}'." });
                }
            }

            // Upsert: delete any prior request for this entity (to allow resubmission of draft), keep approved ones locked
            var existing = await _db.ApprovalRequests
                .Include(r => r.Actions)
                .FirstOrDefaultAsync(r => r.EntityType == model.EntityType && r.EntityId == model.EntityId);
            if (existing != null)
            {
                if (existing.Status == ApprovalRequestStatus.Approved)
                    return BadRequest(new { message = "This request is already approved and locked." });
                _db.ApprovalStepActions.RemoveRange(existing.Actions);
                _db.ApprovalRequests.Remove(existing);
                await _db.SaveChangesAsync();
            }

            var request = new ApprovalRequest
            {
                ApprovalWorkflowId = wf.Id,
                EntityType = model.EntityType,
                EntityId = model.EntityId,
                Status = ApprovalRequestStatus.Submitted,
                SubmittedById = currentUser.Id,
                SubmittedAt = DateTime.Now,
                CurrentStepRank = wf.Steps.First().Rank,
                Actions = wf.Steps.Select(s => new ApprovalStepAction
                {
                    StepRank = s.Rank,
                    StepName = s.Name,
                    AssignedToUserId = assignmentByRank[s.Rank],
                    Status = StepActionStatus.Pending
                }).ToList()
            };

            _db.ApprovalRequests.Add(request);
            await _db.SaveChangesAsync();

            var saved = await _db.ApprovalRequests
                .Include(r => r.ApprovalWorkflow).ThenInclude(w => w!.Steps.OrderBy(s => s.Rank)).ThenInclude(s => s.Role)
                .Include(r => r.SubmittedBy)
                .Include(r => r.Actions.OrderBy(a => a.StepRank)).ThenInclude(a => a.AssignedTo)
                .Include(r => r.Actions).ThenInclude(a => a.ActionedBy)
                .FirstAsync(r => r.Id == request.Id);
            return Ok(_mapper.Map<ApprovalRequestDto>(saved));
        }

        // POST api/approvalRequests/{id}/action
        [HttpPost("{id}/action")]
        public async Task<IActionResult> Action(int id, [FromBody] ApprovalActionRequestDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null) return Unauthorized();

            var request = await _db.ApprovalRequests
                .Include(r => r.ApprovalWorkflow).ThenInclude(w => w!.Steps)
                .Include(r => r.Actions)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (request == null) return NotFound();

            if (request.Status == ApprovalRequestStatus.Approved || request.Status == ApprovalRequestStatus.Rejected)
                return BadRequest(new { message = "This request is already finalized." });

            var currentAction = request.Actions.FirstOrDefault(a => a.StepRank == request.CurrentStepRank);
            if (currentAction == null) return BadRequest(new { message = "No pending step found." });

            if (currentAction.AssignedToUserId != currentUser.Id)
                return Forbid();

            var action = (model.Action ?? "").ToLower();
            currentAction.ActionedByUserId = currentUser.Id;
            currentAction.ActionedAt = DateTime.Now;
            currentAction.Comment = model.Comment;

            if (action == "approve")
            {
                currentAction.Status = StepActionStatus.Approved;

                var step = request.ApprovalWorkflow!.Steps.First(s => s.Rank == currentAction.StepRank);
                if (step.IsFinal || request.ApprovalWorkflow!.Steps.Max(s => s.Rank) == currentAction.StepRank)
                {
                    request.Status = ApprovalRequestStatus.Approved;
                    await _db.SaveChangesAsync();
                    await FinalizeEntityApproval(request);
                }
                else
                {
                    var next = request.ApprovalWorkflow!.Steps.Where(s => s.Rank > currentAction.StepRank).OrderBy(s => s.Rank).First();
                    request.CurrentStepRank = next.Rank;
                }
            }
            else if (action == "reject")
            {
                currentAction.Status = StepActionStatus.Rejected;
                request.Status = ApprovalRequestStatus.Rejected;
            }
            else if (action == "return")
            {
                currentAction.Status = StepActionStatus.Returned;
                request.Status = ApprovalRequestStatus.Returned;
            }
            else
            {
                return BadRequest(new { message = "Action must be 'approve', 'reject' or 'return'." });
            }

            await _db.SaveChangesAsync();

            var saved = await _db.ApprovalRequests
                .Include(r => r.ApprovalWorkflow).ThenInclude(w => w!.Steps.OrderBy(s => s.Rank)).ThenInclude(s => s.Role)
                .Include(r => r.SubmittedBy)
                .Include(r => r.Actions.OrderBy(a => a.StepRank)).ThenInclude(a => a.AssignedTo)
                .Include(r => r.Actions).ThenInclude(a => a.ActionedBy)
                .FirstAsync(r => r.Id == request.Id);
            return Ok(_mapper.Map<ApprovalRequestDto>(saved));
        }

        // POST api/approvalRequests/{id}/reverse — SuperAdministrator only
        [HttpPost("{id}/reverse")]
        [Authorize(Policy = "SuperAdminRole")]
        public async Task<IActionResult> Reverse(int id, [FromBody] ReverseApprovalDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (string.IsNullOrWhiteSpace(model.Comment))
                return BadRequest(new { message = "A reversal comment is required." });

            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null) return Unauthorized();

            var request = await _db.ApprovalRequests
                .Include(r => r.Actions)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (request == null) return NotFound();
            if (request.Status != ApprovalRequestStatus.Approved)
                return BadRequest(new { message = "Only approved requests can be reversed." });

            // Undo entity-specific side effects + reset entity to draft
            await ReverseEntityApproval(request);

            // Mark the request itself as reversed and leave an audit trail
            request.Status = ApprovalRequestStatus.Reversed;
            var reversalAction = new ApprovalStepAction
            {
                ApprovalRequestId = request.Id,
                StepRank = 999,
                StepName = "Reversal (Super Admin)",
                AssignedToUserId = currentUser.Id,
                ActionedByUserId = currentUser.Id,
                ActionedAt = DateTime.Now,
                Status = StepActionStatus.Approved,
                Comment = model.Comment
            };
            _db.ApprovalStepActions.Add(reversalAction);
            await _db.SaveChangesAsync();

            var saved = await _db.ApprovalRequests
                .Include(r => r.ApprovalWorkflow).ThenInclude(w => w!.Steps.OrderBy(s => s.Rank)).ThenInclude(s => s.Role)
                .Include(r => r.SubmittedBy)
                .Include(r => r.Actions.OrderBy(a => a.StepRank)).ThenInclude(a => a.AssignedTo)
                .Include(r => r.Actions).ThenInclude(a => a.ActionedBy)
                .FirstAsync(r => r.Id == request.Id);
            return Ok(_mapper.Map<ApprovalRequestDto>(saved));
        }

        private async Task ReverseEntityApproval(ApprovalRequest request)
        {
            try
            {
                switch (request.EntityType)
                {
                    case "Expense": await ReverseExpense(request.EntityId); break;
                    case "JournalEntry": await ReverseJournalEntry(request.EntityId); break;
                    case "CreditDebitNote": await ReverseCreditDebitNote(request.EntityId); break;
                    case "BudgetAmendment": await ReverseBudgetAmendment(request.EntityId); break;
                    case "Budget": await ReverseBudget(request.EntityId); break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error reversing approval for {request.EntityType}#{request.EntityId}.");
            }
        }

        private async Task ReverseExpense(int id)
        {
            var item = await _db.Expenses.FirstOrDefaultAsync(e => e.Id == id);
            if (item == null) return;
            // Delete the auto-posted journal entry
            var autoRef = $"EXP-JNL-{item.ReferenceNumber}";
            var journal = await _db.JournalEntries.Include(j => j.Lines).FirstOrDefaultAsync(j => j.ReferenceNumber == autoRef);
            if (journal != null)
            {
                _db.JournalLines.RemoveRange(journal.Lines);
                _db.JournalEntries.Remove(journal);
            }
            item.Status = ExpenseStatus.Draft;
            await _db.SaveChangesAsync();
        }

        private async Task ReverseJournalEntry(int id)
        {
            var item = await _db.JournalEntries.FirstOrDefaultAsync(e => e.Id == id);
            if (item == null) return;
            item.Status = JournalEntryStatus.Draft;
            item.IsPosted = false;
            await _db.SaveChangesAsync();
        }

        private async Task ReverseCreditDebitNote(int id)
        {
            var note = await _db.Payments.FirstOrDefaultAsync(p => p.Id == id);
            if (note == null) return;

            // Reverse invoice changes
            if (note.PaymentType == PaymentType.CreditNote && note.StudentInvoiceId.HasValue)
            {
                var invoice = await _db.StudentInvoices.Include(i => i.Items)
                    .FirstOrDefaultAsync(i => i.Id == note.StudentInvoiceId.Value);
                if (invoice != null)
                {
                    invoice.PaidAmount += note.Amount;
                    // Redistribute: add note.Amount back proportionally (best-effort)
                    var totalPaid = invoice.Items.Sum(i => i.PaidAmount);
                    if (totalPaid > 0)
                    {
                        foreach (var it in invoice.Items)
                        {
                            var proportion = it.PaidAmount / totalPaid;
                            it.PaidAmount += note.Amount * proportion;
                        }
                    }
                    var balance = invoice.TotalAmount - invoice.PaidAmount - invoice.DiscountAmount;
                    if (invoice.PaidAmount <= 0) invoice.Status = InvoiceStatus.Unpaid;
                    else if (balance <= 0) invoice.Status = InvoiceStatus.Paid;
                    else invoice.Status = InvoiceStatus.PartiallyPaid;
                }
                // Delete the auto-posted journal entry
                var autoRef = $"CN-JNL-{note.ReceiptNumber}";
                var journal = await _db.JournalEntries.Include(j => j.Lines).FirstOrDefaultAsync(j => j.ReferenceNumber == autoRef);
                if (journal != null)
                {
                    _db.JournalLines.RemoveRange(journal.Lines);
                    _db.JournalEntries.Remove(journal);
                }
            }
            else if (note.PaymentType == PaymentType.DebitNote && note.StudentInvoiceId.HasValue)
            {
                var invoice = await _db.StudentInvoices.FirstOrDefaultAsync(i => i.Id == note.StudentInvoiceId.Value);
                if (invoice != null)
                {
                    invoice.TotalAmount = Math.Max(0, invoice.TotalAmount - note.Amount);
                    var balance = invoice.TotalAmount - invoice.PaidAmount - invoice.DiscountAmount;
                    if (balance <= 0 && invoice.PaidAmount > 0) invoice.Status = InvoiceStatus.Paid;
                    else if (balance > 0 && invoice.PaidAmount > 0) invoice.Status = InvoiceStatus.PartiallyPaid;
                    else invoice.Status = InvoiceStatus.Unpaid;
                }
            }
            note.ApprovalStatus = PaymentApprovalStatus.Draft;
            await _db.SaveChangesAsync();
        }

        private async Task ReverseBudgetAmendment(int id)
        {
            var item = await _db.BudgetAmendments.Include(a => a.Lines).FirstOrDefaultAsync(a => a.Id == id);
            if (item == null) return;
            // Remove any budget lines that this amendment created (PreviousAmount was 0 and BudgetedAmount is still 0)
            var budget = await _db.Budgets.Include(b => b.Lines).FirstOrDefaultAsync(b => b.Id == item.BudgetId);
            if (budget != null)
            {
                foreach (var al in item.Lines.Where(l => l.PreviousAmount == 0))
                {
                    var existing = budget.Lines.FirstOrDefault(bl => bl.AccountId == al.AccountId && bl.BudgetedAmount == 0);
                    if (existing != null) _db.BudgetLines.Remove(existing);
                }
            }
            item.Status = BudgetAmendmentStatus.Pending;
            item.ApprovedDate = null;
            item.ApprovedById = null;
            await _db.SaveChangesAsync();
        }

        private async Task ReverseBudget(int id)
        {
            var item = await _db.Budgets.FirstOrDefaultAsync(b => b.Id == id);
            if (item == null) return;
            item.IsActive = false;
            await _db.SaveChangesAsync();
        }

        // ========= Finalization: run entity-specific side effects on full approval =========

        private async Task FinalizeEntityApproval(ApprovalRequest request)
        {
            try
            {
                // The user who performed the final approval action — used to stamp audit fields on the entity.
                var finalApproverId = request.Actions
                    .Where(a => a.Status == StepActionStatus.Approved && a.ActionedByUserId.HasValue)
                    .OrderByDescending(a => a.StepRank)
                    .Select(a => a.ActionedByUserId)
                    .FirstOrDefault();

                switch (request.EntityType)
                {
                    case "Expense": await FinalizeExpense(request.EntityId); break;
                    case "JournalEntry": await FinalizeJournalEntry(request.EntityId); break;
                    case "CreditDebitNote": await FinalizeCreditDebitNote(request.EntityId); break;
                    case "BudgetAmendment": await FinalizeBudgetAmendment(request.EntityId, finalApproverId); break;
                    case "Budget": await FinalizeBudget(request.EntityId); break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error finalizing approval for {request.EntityType}#{request.EntityId}.");
            }
        }

        private async Task<int?> GetSettingAccountId(string key)
        {
            var s = await _db.GlobalSettings.FirstOrDefaultAsync(x => x.Module == "Finance" && x.SettingKey == key);
            return int.TryParse(s?.SettingValue, out var id) ? id : (int?)null;
        }

        private async Task FinalizeExpense(int expenseId)
        {
            var item = await _db.Expenses.Include(e => e.Lines).FirstOrDefaultAsync(e => e.Id == expenseId);
            if (item == null) return;
            if (item.Status == ExpenseStatus.Approved) return;
            item.Status = ExpenseStatus.Approved;
            await _db.SaveChangesAsync();

            int? cashAccountId = item.PaidFromAccountId ?? await GetSettingAccountId("CashAccountId");
            if (cashAccountId == null) return;

            var journal = new JournalEntry
            {
                ReferenceNumber = $"EXP-JNL-{item.ReferenceNumber}",
                EntryDate = item.ExpenseDate,
                Description = $"Auto-posted: Expense {item.ReferenceNumber}",
                IsPosted = true,
                Status = JournalEntryStatus.Approved,
                Lines = new List<JournalLine>()
            };
            var total = item.Lines.Sum(l => l.Amount);
            journal.Lines.Add(new JournalLine {
                AccountId = cashAccountId.Value, Debit = 0, Credit = total,
                Description = $"Payment for expense {item.ReferenceNumber}"
            });
            foreach (var line in item.Lines)
            {
                var cat = await _db.ExpenseCategories.FindAsync(line.ExpenseCategoryId);
                if (cat?.ExpenseAccountId != null)
                {
                    journal.Lines.Add(new JournalLine {
                        AccountId = cat.ExpenseAccountId.Value, Debit = line.Amount, Credit = 0,
                        Description = $"{cat.Name} - {line.Vendor ?? ""} {line.Description ?? ""}".Trim()
                    });
                }
            }
            if (journal.Lines.Count > 1)
            {
                _db.JournalEntries.Add(journal);
                await _db.SaveChangesAsync();
            }
        }

        private async Task FinalizeJournalEntry(int entryId)
        {
            var item = await _db.JournalEntries.FirstOrDefaultAsync(e => e.Id == entryId);
            if (item == null) return;
            if (item.Status == JournalEntryStatus.Approved) return;
            item.Status = JournalEntryStatus.Approved;
            item.IsPosted = true;
            await _db.SaveChangesAsync();
        }

        private async Task FinalizeCreditDebitNote(int noteId)
        {
            var note = await _db.Payments.FirstOrDefaultAsync(p => p.Id == noteId);
            if (note == null) return;
            if (note.ApprovalStatus == PaymentApprovalStatus.Approved) return;
            note.ApprovalStatus = PaymentApprovalStatus.Approved;

            if (note.PaymentType == PaymentType.CreditNote && note.StudentInvoiceId.HasValue)
            {
                var invoice = await _db.StudentInvoices.Include(i => i.Items)
                    .FirstOrDefaultAsync(i => i.Id == note.StudentInvoiceId.Value);
                if (invoice != null)
                {
                    var totalItemPaid = invoice.Items.Sum(i => i.PaidAmount);
                    if (totalItemPaid > 0)
                    {
                        foreach (var invItem in invoice.Items)
                        {
                            var proportion = invItem.PaidAmount / totalItemPaid;
                            invItem.PaidAmount = Math.Max(0, invItem.PaidAmount - (note.Amount * proportion));
                        }
                    }
                    invoice.PaidAmount = Math.Max(0, invoice.PaidAmount - note.Amount);
                    var balance = invoice.TotalAmount - invoice.PaidAmount - invoice.DiscountAmount;
                    if (invoice.PaidAmount <= 0) invoice.Status = InvoiceStatus.Unpaid;
                    else if (balance <= 0) invoice.Status = InvoiceStatus.Paid;
                    else invoice.Status = InvoiceStatus.PartiallyPaid;
                }
                await _db.SaveChangesAsync();
                await PostCreditNoteJournal(note);
            }
            else if (note.PaymentType == PaymentType.DebitNote && note.StudentInvoiceId.HasValue)
            {
                var invoice = await _db.StudentInvoices.FirstOrDefaultAsync(i => i.Id == note.StudentInvoiceId.Value);
                if (invoice != null)
                {
                    invoice.TotalAmount += note.Amount;
                    var balance = invoice.TotalAmount - invoice.PaidAmount - invoice.DiscountAmount;
                    if (balance > 0 && invoice.PaidAmount > 0) invoice.Status = InvoiceStatus.PartiallyPaid;
                    else if (balance > 0) invoice.Status = InvoiceStatus.Unpaid;
                }
                await _db.SaveChangesAsync();
            }
            else
            {
                await _db.SaveChangesAsync();
            }
        }

        private async Task PostCreditNoteJournal(Payment creditNote)
        {
            var debtorsAccountId = await GetSettingAccountId("DebtorsAccountId");
            if (debtorsAccountId == null) return;
            int? bankAccountId = creditNote.BankAccountId ?? await GetSettingAccountId("CashAccountId");
            if (bankAccountId == null) return;

            var journal = new JournalEntry
            {
                ReferenceNumber = $"CN-JNL-{creditNote.ReceiptNumber}",
                EntryDate = creditNote.PaymentDate,
                Description = $"Auto-posted: Credit Note {creditNote.ReceiptNumber}",
                IsPosted = true,
                Status = JournalEntryStatus.Approved,
                Lines = new List<JournalLine>
                {
                    new JournalLine { AccountId = debtorsAccountId.Value, Debit = creditNote.Amount, Credit = 0,
                        Description = $"Debtors reinstated - {creditNote.ReceiptNumber}" },
                    new JournalLine { AccountId = bankAccountId.Value, Debit = 0, Credit = creditNote.Amount,
                        Description = $"Refund/reversal - {creditNote.ReceiptNumber}" }
                }
            };
            _db.JournalEntries.Add(journal);
            await _db.SaveChangesAsync();
        }

        private async Task FinalizeBudgetAmendment(int amendmentId, int? finalApproverId)
        {
            var item = await _db.BudgetAmendments.Include(a => a.Lines).FirstOrDefaultAsync(a => a.Id == amendmentId);
            if (item == null) return;
            if (item.Status == BudgetAmendmentStatus.Approved) return;

            var budget = await _db.Budgets.Include(b => b.Lines).FirstOrDefaultAsync(b => b.Id == item.BudgetId);
            if (budget != null)
            {
                var existingAccountIds = budget.Lines.Select(x => x.AccountId).ToHashSet();
                foreach (var al in item.Lines)
                {
                    if (!existingAccountIds.Contains(al.AccountId))
                    {
                        _db.BudgetLines.Add(new BudgetLine
                        {
                            BudgetId = budget.Id,
                            AccountId = al.AccountId,
                            BudgetedAmount = 0,
                            Notes = al.Notes
                        });
                        existingAccountIds.Add(al.AccountId);
                    }
                }
            }
            item.Status = BudgetAmendmentStatus.Approved;
            item.ApprovedDate = DateTime.Now;
            item.ApprovedById = finalApproverId;
            await _db.SaveChangesAsync();
        }

        private async Task FinalizeBudget(int budgetId)
        {
            var item = await _db.Budgets.FirstOrDefaultAsync(b => b.Id == budgetId);
            if (item == null) return;
            if (item.IsActive) return;
            item.IsActive = true;
            await _db.SaveChangesAsync();
        }
    }
}
