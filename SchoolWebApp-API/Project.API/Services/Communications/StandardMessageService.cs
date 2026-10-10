using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.DTOs.Communications;
using SchoolWebApp.Core.Entities.Communications;
using SchoolWebApp.Core.Entities.Finance;

namespace SchoolWebApp.API.Services.Communications
{
    /// <summary>
    /// The standard messages - exam results, new invoices, fee balance reminders,
    /// payment acknowledgements - sent to the parents of each learner, from the
    /// message type's template and on its chosen channel. Every learner (or
    /// invoice, or payment) gets its own message; siblings are not merged, since
    /// the content differs.
    /// </summary>
    public class StandardMessageService
    {
        private readonly ApplicationDbContext _db;
        private readonly RecipientResolver _resolver;
        private readonly CommunicationService _communications;
        private readonly ExamResultsCalculator _calculator;

        public StandardMessageService(ApplicationDbContext db, RecipientResolver resolver, CommunicationService communications,
            ExamResultsCalculator calculator)
        {
            _db = db;
            _resolver = resolver;
            _communications = communications;
            _calculator = calculator;
        }

        /// <summary>
        /// Each learner's outstanding fees over all their invoices, cancelled ones
        /// left out - the same sum as the finance reports (invoiced - paid -
        /// discount), but never limited to one term: a reminder states what is owed.
        /// </summary>
        public async Task<Dictionary<int, decimal>> BalancesAsync(IReadOnlyCollection<int> studentIds, CancellationToken ct = default)
        {
            return await _db.StudentInvoices.AsNoTracking()
                .Where(i => studentIds.Contains(i.StudentId) && i.Status != InvoiceStatus.Cancelled)
                .GroupBy(i => i.StudentId)
                .Select(g => new { g.Key, Balance = g.Sum(i => i.TotalAmount - i.PaidAmount - i.DiscountAmount) })
                .ToDictionaryAsync(x => x.Key, x => x.Balance, ct);
        }

        public async Task<object> ExamResultsAsync(ExamResultsMessageDto dto, CancellationToken ct = default)
        {
            var template = await _communications.SystemTemplateAsync(MessageTypeCodes.ExamResults, ct);
            var results = dto.Results.GroupBy(r => r.StudentId).Select(g => g.First()).ToList();
            if (results.Count == 0) throw new CommunicationException("Choose at least one learner.");

            var contacts = await _resolver.ParentContactsAsync(results.Select(r => r.StudentId).ToList(), ct);
            var targets = new List<ContactTarget>();
            foreach (var r in results)
            {
                var withScores = r.Subjects.Where(s => !string.IsNullOrWhiteSpace(s.Score)).ToList();
                var sms = string.Join(", ", withScores.Select(s => $"{s.Subject} {s.Score}"));
                var email = string.Join("\n", withScores.Select(s =>
                    $"{s.SubjectName ?? s.Subject}: {s.Score}{(string.IsNullOrWhiteSpace(s.Grade) ? "" : $" ({s.Grade})")}"));
                foreach (var t in contacts.GetValueOrDefault(r.StudentId) ?? new())
                {
                    t.Values["ExamName"] = dto.ExamName;
                    t.Values["TermName"] = dto.TermName;
                    t.Values["SubjectScores"] = sms;
                    t.Values["TotalMarks"] = r.TotalMarks;
                    t.Values["MeanScore"] = r.MeanScore;
                    t.Values["MeanGrade"] = r.MeanGrade;
                    t.Values["Position"] = r.Position?.ToString();
                    t.Values["ClassSize"] = r.ClassSize?.ToString();
                    t.EmailValues["SubjectScores"] = email;
                    targets.Add(t);
                }
            }

            var spec = Spec(template, $"{dto.ExamName} results{(string.IsNullOrWhiteSpace(dto.ClassName) ? "" : " - " + dto.ClassName)}",
                $"Parents of {results.Count} learner(s){(string.IsNullOrWhiteSpace(dto.ClassName) ? "" : " in " + dto.ClassName)}");
            if (dto.Preview) return await _communications.PreviewAsync(spec, targets, ct);

            var queued = await _communications.QueueAsync(spec, targets, ct);
            if (dto.SchoolExamId != null && !queued.TestMode)
            {
                await _db.SchoolExams.Where(e => e.Id == dto.SchoolExamId)
                    .ExecuteUpdateAsync(u => u
                        .SetProperty(e => e.ParentsNotified, true)
                        .SetProperty(e => e.ParentsNotifiedDate, DateTime.UtcNow), ct);
            }
            return queued;
        }

        /// <summary>
        /// A school exam's results for all its classes, computed here as the
        /// broadsheet would (see <see cref="ExamResultsCalculator"/>), in one send.
        /// </summary>
        public async Task<object> SchoolExamResultsAsync(SchoolExamResultsMessageDto dto, CancellationToken ct = default)
        {
            var computed = await _calculator.ComputeAsync(dto.SchoolExamId, dto.SchoolClassIds, ct)
                ?? throw new CommunicationException("The school exam was not found.");
            var withMarks = computed.Classes.Where(c => c.Results.Count > 0).ToList();
            if (withMarks.Count == 0)
                throw new CommunicationException("No learner has marks in this exam yet, so there are no results to send.");

            return await ExamResultsAsync(new ExamResultsMessageDto
            {
                Preview = dto.Preview,
                SchoolExamId = dto.SchoolExamId,
                ExamName = computed.ExamName,
                TermName = computed.TermName,
                ClassName = withMarks.Count == 1 ? withMarks[0].ClassName : $"{withMarks.Count} classes",
                Results = withMarks.SelectMany(c => c.Results).ToList()
            }, ct);
        }

        public async Task<object> InvoicesAsync(InvoiceMessageDto dto, CancellationToken ct = default)
        {
            var template = await _communications.SystemTemplateAsync(MessageTypeCodes.FeeInvoice, ct);
            var invoices = await _db.StudentInvoices.AsNoTracking()
                .Where(i => dto.InvoiceIds.Contains(i.Id) && i.Status != InvoiceStatus.Cancelled)
                .Select(i => new
                {
                    i.Id,
                    i.StudentId,
                    i.InvoiceNumber,
                    Net = i.TotalAmount - i.DiscountAmount,
                    i.DueDate,
                    Session = i.Session != null ? i.Session.SessionName : null,
                    Year = i.AcademicYear!.Name,
                    Items = i.Items.Select(it => new { Category = it.FeeCategory!.Name, Net = it.Amount - it.Discount }).ToList()
                })
                .ToListAsync(ct);
            if (invoices.Count == 0) throw new CommunicationException("None of the chosen invoices can be sent (cancelled invoices are not).");

            var studentIds = invoices.Select(i => i.StudentId).Distinct().ToList();
            var contacts = await _resolver.ParentContactsAsync(studentIds, ct);
            var balances = await BalancesAsync(studentIds, ct);
            var targets = new List<ContactTarget>();
            foreach (var inv in invoices)
            {
                foreach (var t in (contacts.GetValueOrDefault(inv.StudentId) ?? new()).Select(Clone))
                {
                    t.Values["InvoiceNumber"] = inv.InvoiceNumber;
                    t.Values["InvoiceAmount"] = MessageText.Money(inv.Net);
                    t.Values["TermName"] = string.IsNullOrWhiteSpace(inv.Session) ? inv.Year : $"{inv.Session} {inv.Year}";
                    t.Values["DueDate"] = inv.DueDate?.ToString("dd MMM yyyy") ?? "-";
                    t.Values["Balance"] = MessageText.Money(balances.GetValueOrDefault(inv.StudentId));
                    t.Values["InvoiceItems"] = string.Join(", ", inv.Items.Select(x => $"{x.Category} {MessageText.Money(x.Net)}"));
                    t.EmailValues["InvoiceItems"] = string.Join("\n", inv.Items.Select(x => $"{x.Category}: KES {MessageText.Money(x.Net)}"));
                    targets.Add(t);
                }
            }

            var spec = Spec(template, invoices.Count == 1 ? $"Invoice {invoices[0].InvoiceNumber}" : $"{invoices.Count} fee invoices",
                $"Parents of {studentIds.Count} learner(s)");
            return dto.Preview ? await _communications.PreviewAsync(spec, targets, ct) : await _communications.QueueAsync(spec, targets, ct);
        }

        public async Task<object> FeeBalancesAsync(FeeBalanceMessageDto dto, CancellationToken ct = default)
        {
            var template = await _communications.SystemTemplateAsync(MessageTypeCodes.FeeBalance, ct);
            var balances = await BalancesAsync(dto.StudentIds.Distinct().ToList(), ct);
            var owing = balances.Where(b => b.Value > dto.MinBalance && b.Value > 0).Select(b => b.Key).ToList();
            if (owing.Count == 0) throw new CommunicationException("None of the chosen learners has a fee balance to remind about.");

            var contacts = await _resolver.ParentContactsAsync(owing, ct);
            var targets = new List<ContactTarget>();
            foreach (var id in owing)
            {
                foreach (var t in contacts.GetValueOrDefault(id) ?? new())
                {
                    t.Values["Balance"] = MessageText.Money(balances[id]);
                    targets.Add(t);
                }
            }

            var spec = Spec(template, "Fee balance reminder", $"Parents of {owing.Count} learner(s) with a balance");
            return dto.Preview ? await _communications.PreviewAsync(spec, targets, ct) : await _communications.QueueAsync(spec, targets, ct);
        }

        public async Task<object> PaymentsAsync(PaymentMessageDto dto, CancellationToken ct = default)
        {
            var template = await _communications.SystemTemplateAsync(MessageTypeCodes.FeePayment, ct);
            var payments = await _db.Payments.AsNoTracking()
                .Where(p => dto.PaymentIds.Contains(p.Id) && p.PaymentType == PaymentType.Receipt
                            && p.ApprovalStatus == PaymentApprovalStatus.Approved)
                .Select(p => new { p.Id, p.StudentId, p.ReceiptNumber, p.Amount })
                .ToListAsync(ct);
            if (payments.Count == 0) throw new CommunicationException("Only approved receipts can be acknowledged.");

            var studentIds = payments.Select(p => p.StudentId).Distinct().ToList();
            var contacts = await _resolver.ParentContactsAsync(studentIds, ct);
            var balances = await BalancesAsync(studentIds, ct);
            var targets = new List<ContactTarget>();
            foreach (var p in payments)
            {
                foreach (var t in (contacts.GetValueOrDefault(p.StudentId) ?? new()).Select(Clone))
                {
                    t.Values["AmountPaid"] = MessageText.Money(p.Amount);
                    t.Values["ReceiptNumber"] = p.ReceiptNumber;
                    t.Values["Balance"] = MessageText.Money(balances.GetValueOrDefault(p.StudentId));
                    targets.Add(t);
                }
            }

            var spec = Spec(template, payments.Count == 1 ? $"Receipt {payments[0].ReceiptNumber}" : $"{payments.Count} payment acknowledgements",
                $"Parents of {studentIds.Count} learner(s)");
            return dto.Preview ? await _communications.PreviewAsync(spec, targets, ct) : await _communications.QueueAsync(spec, targets, ct);
        }

        private static BatchSpec Spec(MessageTemplate t, string title, string summary) => new()
        {
            Title = title,
            MessageTypeCode = t.Code!,
            MessageTemplateId = t.Id,
            Channel = t.Channel,
            RecipientSummary = summary,
            SmsBody = t.SmsBody,
            EmailSubject = t.EmailSubject,
            EmailBody = t.EmailBody,
            MergeByDestination = false
        };

        // A learner with two invoices (or payments) gets a message for each, so
        // each needs its own copy of the parent contacts to carry its values.
        private static ContactTarget Clone(ContactTarget t) => new()
        {
            Type = t.Type,
            PersonId = t.PersonId,
            StudentId = t.StudentId,
            Name = t.Name,
            Phone = t.Phone,
            Email = t.Email,
            Values = new Dictionary<string, string?>(t.Values, StringComparer.OrdinalIgnoreCase),
            EmailValues = new Dictionary<string, string?>(t.EmailValues, StringComparer.OrdinalIgnoreCase)
        };
    }
}
