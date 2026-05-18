using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs.Finance.Payment;
using SchoolWebApp.Core.Entities.Finance;
using SchoolWebApp.Core.Entities.Settings;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.API.Controllers.Finance
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly ILogger<PaymentsController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PaymentsController(ILogger<PaymentsController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger; _unitOfWork = unitOfWork; _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _unitOfWork.Payments.Find(includeProperties: "Student,StudentInvoice,BankAccount,OriginalPayment");
            return Ok(_mapper.Map<List<PaymentDto>>(items));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _unitOfWork.Payments.GetById(id, "Student,StudentInvoice,BankAccount,OriginalPayment,Allocations.StudentInvoiceItem.FeeCategory");
            if (item == null) return NotFound();
            return Ok(_mapper.Map<PaymentDto>(item));
        }

        [HttpGet("byStudentId/{studentId}")]
        public async Task<IActionResult> GetByStudent(int studentId)
        {
            var items = await _unitOfWork.Payments.GetByStudentId(studentId);
            return Ok(_mapper.Map<List<PaymentDto>>(items));
        }

        [HttpGet("byInvoiceId/{invoiceId}")]
        public async Task<IActionResult> GetByInvoice(int invoiceId)
        {
            var items = await _unitOfWork.Payments.GetByInvoiceId(invoiceId);
            return Ok(_mapper.Map<List<PaymentDto>>(items));
        }

        [HttpGet("byDateRange")]
        public async Task<IActionResult> GetByDateRange(DateTime from, DateTime to)
        {
            var items = await _unitOfWork.Payments.GetByDateRange(from, to);
            return Ok(_mapper.Map<List<PaymentDto>>(items));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePaymentDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var payment = _mapper.Map<Payment>(model);
            if (string.IsNullOrEmpty(payment.ReceiptNumber))
                payment.ReceiptNumber = $"RCP-{DateTime.Now:yyyyMMddHHmmssfff}";
            _unitOfWork.Payments.Create(payment);

            // Update invoice paid amount + apportion to items
            if (model.StudentInvoiceId.HasValue)
            {
                var invoice = await _unitOfWork.StudentInvoices.GetByIdWithDetails(model.StudentInvoiceId.Value);
                if (invoice != null)
                {
                    invoice.PaidAmount += model.Amount;

                    // Apportion to invoice items
                    var allocations = new List<PaymentAllocation>();

                    if (model.ItemAllocations != null && model.ItemAllocations.Count > 0)
                    {
                        // Manual mode: apply user-specified allocations
                        foreach (var alloc in model.ItemAllocations)
                        {
                            var item = invoice.Items.FirstOrDefault(i => i.Id == alloc.InvoiceItemId);
                            if (item != null)
                            {
                                item.PaidAmount += alloc.Amount;
                                allocations.Add(new PaymentAllocation
                                {
                                    StudentInvoiceItemId = item.Id,
                                    Amount = alloc.Amount
                                });
                            }
                        }
                    }
                    else
                    {
                        // Auto mode: distribute by fee category rank (lowest rank = highest priority)
                        var remaining = model.Amount;
                        var catIds = invoice.Items.Select(i => i.FeeCategoryId).Distinct().ToList();
                        var categories = new Dictionary<int, int>();
                        foreach (var catId in catIds)
                        {
                            var cat = await _unitOfWork.FeeCategories.GetById(catId);
                            categories[catId] = cat?.Rank ?? 999;
                        }
                        var sortedItems = invoice.Items
                            .OrderBy(i => categories.GetValueOrDefault(i.FeeCategoryId, 999))
                            .ThenBy(i => i.Id)
                            .ToList();
                        foreach (var item in sortedItems)
                        {
                            if (remaining <= 0) break;
                            var itemBalance = item.Amount - item.Discount - item.PaidAmount;
                            if (itemBalance <= 0) continue;
                            var apply = Math.Min(remaining, itemBalance);
                            item.PaidAmount += apply;
                            remaining -= apply;
                            allocations.Add(new PaymentAllocation
                            {
                                StudentInvoiceItemId = item.Id,
                                Amount = apply
                            });
                        }
                    }

                    payment.Allocations = allocations;

                    var balance = invoice.TotalAmount - invoice.PaidAmount - invoice.DiscountAmount;
                    if (balance <= 0) invoice.Status = InvoiceStatus.Paid;
                    else if (invoice.PaidAmount > 0) invoice.Status = InvoiceStatus.PartiallyPaid;
                    _unitOfWork.StudentInvoices.Update(invoice);
                }
            }

            await _unitOfWork.SaveChangesAsync();

            await AutoPostPaymentJournal(payment);

            return Ok(_mapper.Map<PaymentDto>(payment));
        }

        [HttpPost("{id}/creditNote")]
        public async Task<IActionResult> CreditNote(int id, [FromBody] NoteRequestDto model)
        {
            var original = await _unitOfWork.Payments.GetById(id, "Student,StudentInvoice");
            if (original == null) return NotFound();
            if (original.PaymentType != PaymentType.Receipt)
                return BadRequest("Credit notes can only be issued against receipts.");

            var amount = model.Amount > 0 ? model.Amount : original.Amount;
            if (amount > original.Amount)
                return BadRequest("Credit note amount cannot exceed original payment.");

            // Create in Draft status — no invoice update or journal posting until approved
            var creditNote = new Payment
            {
                ReceiptNumber = $"CN-{DateTime.Now:yyyyMMddHHmmssfff}",
                PaymentType = PaymentType.CreditNote,
                ApprovalStatus = PaymentApprovalStatus.Draft,
                StudentId = original.StudentId,
                StudentInvoiceId = original.StudentInvoiceId,
                OriginalPaymentId = original.Id,
                PaymentDate = DateTime.Now,
                Amount = amount,
                PaymentMethod = original.PaymentMethod,
                BankAccountId = original.BankAccountId,
                Description = $"Credit Note against {original.ReceiptNumber}",
                Reason = model.Reason
            };
            _unitOfWork.Payments.Create(creditNote);
            await _unitOfWork.SaveChangesAsync();
            return Ok(_mapper.Map<PaymentDto>(creditNote));
        }

        [HttpPost("{id}/debitNote")]
        public async Task<IActionResult> DebitNote(int id, [FromBody] NoteRequestDto model)
        {
            var original = await _unitOfWork.Payments.GetById(id, "Student,StudentInvoice");
            if (original == null) return NotFound();
            if (model.Amount <= 0) return BadRequest("Debit note amount must be positive.");

            // Create in Draft status — no invoice update until approved
            var debitNote = new Payment
            {
                ReceiptNumber = $"DN-{DateTime.Now:yyyyMMddHHmmssfff}",
                PaymentType = PaymentType.DebitNote,
                ApprovalStatus = PaymentApprovalStatus.Draft,
                StudentId = original.StudentId,
                StudentInvoiceId = original.StudentInvoiceId,
                OriginalPaymentId = original.Id,
                PaymentDate = DateTime.Now,
                Amount = model.Amount,
                PaymentMethod = original.PaymentMethod,
                Description = $"Debit Note against {original.ReceiptNumber}",
                Reason = model.Reason
            };
            _unitOfWork.Payments.Create(debitNote);
            await _unitOfWork.SaveChangesAsync();
            return Ok(_mapper.Map<PaymentDto>(debitNote));
        }

        [HttpPost("{id}/submit")]
        public async Task<IActionResult> SubmitNote(int id)
        {
            var note = await _unitOfWork.Payments.GetById(id);
            if (note == null) return NotFound();
            if (note.PaymentType == PaymentType.Receipt) return BadRequest("Receipts do not require approval.");
            if (note.ApprovalStatus != PaymentApprovalStatus.Draft && note.ApprovalStatus != PaymentApprovalStatus.Rejected)
                return BadRequest("Only draft or rejected notes can be submitted.");
            note.ApprovalStatus = PaymentApprovalStatus.Submitted;
            _unitOfWork.Payments.Update(note);
            await _unitOfWork.SaveChangesAsync();
            return Ok(new { message = "Note submitted for approval." });
        }

        [HttpPost("{id}/approveNote")]
        public async Task<IActionResult> ApproveNote(int id)
        {
            var note = await _unitOfWork.Payments.GetById(id);
            if (note == null) return NotFound();
            if (note.PaymentType == PaymentType.Receipt) return BadRequest("Receipts are auto-approved.");
            if (note.ApprovalStatus != PaymentApprovalStatus.Submitted)
                return BadRequest("Only submitted notes can be approved.");

            note.ApprovalStatus = PaymentApprovalStatus.Approved;
            _unitOfWork.Payments.Update(note);

            // Apply side-effects based on note type
            if (note.PaymentType == PaymentType.CreditNote && note.StudentInvoiceId.HasValue)
            {
                var invoice = await _unitOfWork.StudentInvoices.GetByIdWithDetails(note.StudentInvoiceId.Value);
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
                    _unitOfWork.StudentInvoices.Update(invoice);
                }
                await _unitOfWork.SaveChangesAsync();
                await AutoPostCreditNoteJournal(note);
            }
            else if (note.PaymentType == PaymentType.DebitNote && note.StudentInvoiceId.HasValue)
            {
                var invoice = await _unitOfWork.StudentInvoices.GetByIdWithDetails(note.StudentInvoiceId.Value);
                if (invoice != null)
                {
                    invoice.TotalAmount += note.Amount;
                    var balance = invoice.TotalAmount - invoice.PaidAmount - invoice.DiscountAmount;
                    if (balance > 0 && invoice.PaidAmount > 0) invoice.Status = InvoiceStatus.PartiallyPaid;
                    else if (balance > 0) invoice.Status = InvoiceStatus.Unpaid;
                    _unitOfWork.StudentInvoices.Update(invoice);
                }
                await _unitOfWork.SaveChangesAsync();
            }

            return Ok(new { message = "Note approved and applied." });
        }

        [HttpPost("{id}/rejectNote")]
        public async Task<IActionResult> RejectNote(int id, [FromBody] NoteRequestDto model)
        {
            var note = await _unitOfWork.Payments.GetById(id);
            if (note == null) return NotFound();
            if (note.PaymentType == PaymentType.Receipt) return BadRequest("Receipts cannot be rejected.");
            if (note.ApprovalStatus != PaymentApprovalStatus.Submitted)
                return BadRequest("Only submitted notes can be rejected.");
            note.ApprovalStatus = PaymentApprovalStatus.Rejected;
            note.Description = (note.Description ?? "") + $" [Rejected: {model.Reason}]";
            _unitOfWork.Payments.Update(note);
            await _unitOfWork.SaveChangesAsync();
            return Ok(new { message = "Note rejected." });
        }

        private async Task AutoPostCreditNoteJournal(Payment creditNote)
        {
            var debtorsAccountId = await GetSettingAccountId("DebtorsAccountId");
            if (debtorsAccountId == null) return;

            int? bankAccountId = creditNote.BankAccountId;
            if (bankAccountId == null)
                bankAccountId = await GetSettingAccountId("CashAccountId");
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
                    new JournalLine
                    {
                        AccountId = debtorsAccountId.Value,
                        Debit = creditNote.Amount,
                        Credit = 0,
                        Description = $"Debtors reinstated - {creditNote.ReceiptNumber}"
                    },
                    new JournalLine
                    {
                        AccountId = bankAccountId.Value,
                        Debit = 0,
                        Credit = creditNote.Amount,
                        Description = $"Refund/reversal - {creditNote.ReceiptNumber}"
                    }
                }
            };

            _unitOfWork.JournalEntries.Create(journal);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<int?> GetSettingAccountId(string key)
        {
            var settings = await _unitOfWork.Repository<SchoolWebApp.Core.Entities.Settings.GlobalSetting>()
                .Find(s => s.Module == "Finance" && s.SettingKey == key);
            var val = settings.FirstOrDefault()?.SettingValue;
            return int.TryParse(val, out var id) ? id : null;
        }

        private async Task AutoPostPaymentJournal(Payment payment)
        {
            var debtorsAccountId = await GetSettingAccountId("DebtorsAccountId");
            if (debtorsAccountId == null) return;

            int? bankAccountId = payment.BankAccountId;
            if (bankAccountId == null)
                bankAccountId = await GetSettingAccountId("CashAccountId");
            if (bankAccountId == null) return;

            var journal = new JournalEntry
            {
                ReferenceNumber = $"PAY-JNL-{payment.ReceiptNumber}",
                EntryDate = payment.PaymentDate,
                Description = $"Auto-posted: Payment {payment.ReceiptNumber}",
                IsPosted = true,
                Status = JournalEntryStatus.Approved,
                Lines = new List<JournalLine>
                {
                    new JournalLine
                    {
                        AccountId = bankAccountId.Value,
                        Debit = payment.Amount,
                        Credit = 0,
                        Description = $"Payment received - {payment.ReceiptNumber}"
                    },
                    new JournalLine
                    {
                        AccountId = debtorsAccountId.Value,
                        Debit = 0,
                        Credit = payment.Amount,
                        Description = $"Debtors cleared - {payment.ReceiptNumber}"
                    }
                }
            };

            _unitOfWork.JournalEntries.Create(journal);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
