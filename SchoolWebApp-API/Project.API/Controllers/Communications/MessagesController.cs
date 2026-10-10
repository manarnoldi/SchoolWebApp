using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.API.Services.Communications;
using SchoolWebApp.Core.DTOs.Communications;
using SchoolWebApp.Core.Entities.Communications;

namespace SchoolWebApp.API.Controllers.Communications
{
    /// <summary>
    /// Composing and queueing messages, the queue itself, and the monthly SMS report.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        public const string SenderRoles = "Administrator,SuperAdministrator,HeadTeacher,Accounts";

        private readonly ILogger<MessagesController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly CommunicationService _communications;
        private readonly RecipientResolver _resolver;
        private readonly MessageDispatchSignal _signal;
        private readonly StandardMessageService _standard;

        public MessagesController(ILogger<MessagesController> logger, ApplicationDbContext db,
            CommunicationService communications, RecipientResolver resolver, MessageDispatchSignal signal,
            StandardMessageService standard)
        {
            _logger = logger; _db = db; _communications = communications; _resolver = resolver; _signal = signal;
            _standard = standard;
        }

        /// <summary>Everything the compose screen's recipient pickers need, in one call.</summary>
        [HttpGet("recipientOptions")]
        public async Task<IActionResult> RecipientOptions()
        {
            var yearId = await _resolver.CurrentAcademicYearIdAsync();
            var classes = await _db.SchoolClasses.AsNoTracking()
                .Where(c => c.AcademicYearId == yearId)
                .OrderBy(c => c.Rank)
                .Select(c => new
                {
                    c.Id,
                    Name = c.LearningLevel!.Name + " " + c.SchoolStream!.Name,
                    c.LearningLevel.EducationLevelId,
                    Learners = c.StudentClasses.Count(sc => sc.Student!.Status == SchoolWebApp.Core.Entities.Enums.Status.Active)
                })
                .ToListAsync();
            var levels = await _db.EducationLevels.AsNoTracking().OrderBy(l => l.Rank)
                .Select(l => new { l.Id, l.Name }).ToListAsync();
            var categories = await _db.StaffCategories.AsNoTracking().OrderBy(c => c.Name)
                .Select(c => new { c.Id, c.Name }).ToListAsync();
            var staff = await _db.StaffDetails.AsNoTracking()
                .Where(s => s.Status == SchoolWebApp.Core.Entities.Enums.Status.Active && s.CurrentlyEmployed)
                .OrderBy(s => s.FullName)
                .Select(s => new { s.Id, s.FullName, s.StaffCategoryId, HasPhone = s.PhoneNumber != null && s.PhoneNumber != "", HasEmail = s.Email != null && s.Email != "" })
                .ToListAsync();
            var students = await _db.StudentClasses.AsNoTracking()
                .Where(sc => sc.SchoolClass!.AcademicYearId == yearId && sc.Student!.Status == SchoolWebApp.Core.Entities.Enums.Status.Active)
                .OrderBy(sc => sc.Student!.FullName)
                .Select(sc => new { Id = sc.StudentId, sc.Student!.FullName, AdmissionNo = sc.Student.UPI, sc.SchoolClassId })
                .ToListAsync();
            return Ok(new { classes, levels, categories, staff, students });
        }

        [HttpPost("preview")]
        public async Task<IActionResult> Preview(ComposeMessageDto model)
        {
            try
            {
                var targets = await _resolver.ResolveAsync(model.Recipients);
                var spec = await _communications.CustomSpecAsync(model);
                return Ok(await _communications.PreviewAsync(spec, targets));
            }
            catch (CommunicationException ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("send")]
        [Authorize(Roles = SenderRoles)]
        public async Task<IActionResult> Send(ComposeMessageDto model)
        {
            try
            {
                var targets = await _resolver.ResolveAsync(model.Recipients);
                var spec = await _communications.CustomSpecAsync(model);
                return Ok(await _communications.QueueAsync(spec, targets));
            }
            catch (CommunicationException ex) { return BadRequest(ex.Message); }
        }

        // Standard messages. Each takes Preview = true to return what would go out
        // (counts, the first rendered message, recipients) without queueing.

        [HttpPost("examResults")]
        [Authorize(Roles = SenderRoles)]
        public Task<IActionResult> ExamResults(ExamResultsMessageDto model) => Standard(() => _standard.ExamResultsAsync(model));

        [HttpPost("schoolExamResults")]
        [Authorize(Roles = SenderRoles)]
        public Task<IActionResult> SchoolExamResults(SchoolExamResultsMessageDto model) => Standard(() => _standard.SchoolExamResultsAsync(model));

        [HttpPost("feeInvoices")]
        [Authorize(Roles = SenderRoles)]
        public Task<IActionResult> FeeInvoices(InvoiceMessageDto model) => Standard(() => _standard.InvoicesAsync(model));

        [HttpPost("feeBalances")]
        [Authorize(Roles = SenderRoles)]
        public Task<IActionResult> FeeBalances(FeeBalanceMessageDto model) => Standard(() => _standard.FeeBalancesAsync(model));

        [HttpPost("feePayments")]
        [Authorize(Roles = SenderRoles)]
        public Task<IActionResult> FeePayments(PaymentMessageDto model) => Standard(() => _standard.PaymentsAsync(model));

        private async Task<IActionResult> Standard(Func<Task<object>> run)
        {
            try { return Ok(await run()); }
            catch (CommunicationException ex) { return BadRequest(ex.Message); }
        }

        // GET api/messages/queue?status=&channel=&batchId=&messageTypeCode=&from=&to=&search=&pageNumber=1&pageSize=50
        [HttpGet("queue")]
        public async Task<IActionResult> Queue(MessageStatus? status, MessageChannel? channel, int? batchId, string? messageTypeCode,
            DateTime? from, DateTime? to, string? search, int pageNumber = 1, int pageSize = 50)
        {
            var query = _db.OutboundMessages.AsNoTracking().AsQueryable();
            if (channel != null) query = query.Where(m => m.Channel == channel);
            if (batchId != null) query = query.Where(m => m.MessageBatchId == batchId);
            if (!string.IsNullOrWhiteSpace(messageTypeCode)) query = query.Where(m => m.MessageTypeCode == messageTypeCode);
            if (from != null) query = query.Where(m => m.Created >= from.Value.Date.Subtract(MessageText.SchoolUtcOffset));
            if (to != null) query = query.Where(m => m.Created < to.Value.Date.AddDays(1).Subtract(MessageText.SchoolUtcOffset));
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(m => m.Destination.Contains(term) || (m.RecipientName != null && m.RecipientName.Contains(term))
                                         || (m.OriginalDestination != null && m.OriginalDestination.Contains(term)));
            }

            var counts = await query.GroupBy(m => m.Status)
                .Select(g => new StatusCountDto { Status = g.Key, Count = g.Count() })
                .ToListAsync();
            if (status != null) query = query.Where(m => m.Status == status);

            pageSize = Math.Clamp(pageSize, 1, 500);
            var total = await query.CountAsync();
            var data = await query.OrderByDescending(m => m.Id)
                .Skip((Math.Max(pageNumber, 1) - 1) * pageSize).Take(pageSize)
                .Select(Projection)
                .ToListAsync();
            return Ok(new MessageQueuePageDto { Data = data, TotalCount = total, StatusCounts = counts });
        }

        // GET api/messages/batches?from=&to=&pageNumber=1&pageSize=25
        [HttpGet("batches")]
        public async Task<IActionResult> Batches(DateTime? from, DateTime? to, int pageNumber = 1, int pageSize = 25)
        {
            var query = _db.MessageBatches.AsNoTracking().AsQueryable();
            if (from != null) query = query.Where(b => b.Created >= from.Value.Date.Subtract(MessageText.SchoolUtcOffset));
            if (to != null) query = query.Where(b => b.Created < to.Value.Date.AddDays(1).Subtract(MessageText.SchoolUtcOffset));

            pageSize = Math.Clamp(pageSize, 1, 200);
            var total = await query.CountAsync();
            var data = await query.OrderByDescending(b => b.Id)
                .Skip((Math.Max(pageNumber, 1) - 1) * pageSize).Take(pageSize)
                .Select(b => new MessageBatchDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    MessageTypeCode = b.MessageTypeCode,
                    Channel = b.Channel,
                    RecipientSummary = b.RecipientSummary,
                    TotalMessages = b.TotalMessages,
                    IsTest = b.IsTest,
                    Created = b.Created,
                    CreatedBy = b.CreatedBy,
                    Queued = b.Messages.Count(m => m.Status == MessageStatus.Queued || m.Status == MessageStatus.Sending),
                    Sent = b.Messages.Count(m => m.Status == MessageStatus.Sent),
                    Delivered = b.Messages.Count(m => m.Status == MessageStatus.Delivered),
                    Failed = b.Messages.Count(m => m.Status == MessageStatus.Failed || m.Status == MessageStatus.Undelivered),
                    Cancelled = b.Messages.Count(m => m.Status == MessageStatus.Cancelled),
                    SmsParts = b.Messages.Sum(m => m.SmsParts)
                })
                .ToListAsync();
            return Ok(new { data, totalCount = total });
        }

        [HttpPost("{id}/retry")]
        [Authorize(Roles = SenderRoles)]
        public async Task<IActionResult> Retry(int id)
        {
            var n = await RequeueAsync(_db.OutboundMessages.Where(m => m.Id == id));
            return n == 0 ? BadRequest("Only failed or cancelled messages can be retried.") : Ok(new { requeued = n });
        }

        [HttpPost("{id}/cancel")]
        [Authorize(Roles = SenderRoles)]
        public async Task<IActionResult> Cancel(int id)
        {
            var n = await CancelAsync(_db.OutboundMessages.Where(m => m.Id == id));
            return n == 0 ? BadRequest("Only messages still waiting in the queue can be cancelled.") : Ok(new { cancelled = n });
        }

        [HttpPost("batches/{id}/retryFailed")]
        [Authorize(Roles = SenderRoles)]
        public async Task<IActionResult> RetryBatch(int id)
            => Ok(new { requeued = await RequeueAsync(_db.OutboundMessages.Where(m => m.MessageBatchId == id)) });

        [HttpPost("batches/{id}/cancel")]
        [Authorize(Roles = SenderRoles)]
        public async Task<IActionResult> CancelBatch(int id)
            => Ok(new { cancelled = await CancelAsync(_db.OutboundMessages.Where(m => m.MessageBatchId == id)) });

        private async Task<int> RequeueAsync(IQueryable<OutboundMessage> query)
        {
            var n = await query
                .Where(m => m.Status == MessageStatus.Failed || m.Status == MessageStatus.Cancelled)
                .ExecuteUpdateAsync(u => u
                    .SetProperty(m => m.Status, MessageStatus.Queued)
                    .SetProperty(m => m.Attempts, 0)
                    .SetProperty(m => m.NextAttemptAt, (DateTime?)null)
                    .SetProperty(m => m.ErrorMessage, (string?)null));
            if (n > 0) _signal.Notify();
            return n;
        }

        private static Task<int> CancelAsync(IQueryable<OutboundMessage> query) => query
            .Where(m => m.Status == MessageStatus.Queued)
            .ExecuteUpdateAsync(u => u
                .SetProperty(m => m.Status, MessageStatus.Cancelled)
                .SetProperty(m => m.NextAttemptAt, (DateTime?)null));

        /// <summary>
        /// SMS sent in a calendar month (school time), for billing. Billable =
        /// accepted by the gateway (sent, delivered or undelivered) and not a test;
        /// counted in SMS parts, which is what the gateway charges.
        /// </summary>
        // GET api/messages/smsReport?year=2026&month=10&includeDetails=true
        [HttpGet("smsReport")]
        public async Task<IActionResult> SmsReport(int year, int month, bool includeDetails = false)
        {
            if (month < 1 || month > 12 || year < 2000) return BadRequest("Choose a valid month.");
            var start = new DateTime(year, month, 1).Subtract(MessageText.SchoolUtcOffset);
            var end = new DateTime(year, month, 1).AddMonths(1).Subtract(MessageText.SchoolUtcOffset);

            var inMonth = _db.OutboundMessages.AsNoTracking()
                .Where(m => m.Channel == MessageChannel.Sms && m.SentAt >= start && m.SentAt < end);
            var rows = await inMonth
                .Select(m => new { m.MessageTypeCode, m.Status, m.SmsParts, m.IsTest, m.SentAt })
                .ToListAsync();
            var failed = await _db.OutboundMessages.AsNoTracking()
                .CountAsync(m => m.Channel == MessageChannel.Sms && m.Status == MessageStatus.Failed
                                 && m.Created >= start && m.Created < end && !m.IsTest);

            var billable = rows.Where(r => !r.IsTest &&
                (r.Status == MessageStatus.Sent || r.Status == MessageStatus.Delivered || r.Status == MessageStatus.Undelivered)).ToList();
            var settings = await _communications.GetSettingsAsync();
            var report = new SmsReportDto
            {
                Year = year,
                Month = month,
                Messages = billable.Count,
                Parts = billable.Sum(r => r.SmsParts),
                Delivered = billable.Count(r => r.Status == MessageStatus.Delivered),
                Undelivered = billable.Count(r => r.Status == MessageStatus.Undelivered),
                AwaitingReport = billable.Count(r => r.Status == MessageStatus.Sent),
                Failed = failed,
                TestMessages = rows.Count(r => r.IsTest),
                UnitPrice = settings.SmsUnitPrice,
                ByMessageType = billable.GroupBy(r => r.MessageTypeCode)
                    .Select(g => new SmsReportRowDto { Label = g.Key, Messages = g.Count(), Parts = g.Sum(r => r.SmsParts) })
                    .OrderByDescending(r => r.Parts).ToList(),
                ByDay = billable.GroupBy(r => r.SentAt!.Value.Add(MessageText.SchoolUtcOffset).Date)
                    .OrderBy(g => g.Key)
                    .Select(g => new SmsReportRowDto { Label = g.Key.ToString("yyyy-MM-dd"), Messages = g.Count(), Parts = g.Sum(r => r.SmsParts) })
                    .ToList()
            };
            report.Amount = report.Parts * report.UnitPrice;

            if (includeDetails)
            {
                report.Details = await inMonth
                    .Where(m => !m.IsTest && (m.Status == MessageStatus.Sent || m.Status == MessageStatus.Delivered || m.Status == MessageStatus.Undelivered))
                    .OrderBy(m => m.SentAt)
                    .Select(Projection)
                    .ToListAsync();
            }
            return Ok(report);
        }

        private static readonly System.Linq.Expressions.Expression<Func<OutboundMessage, OutboundMessageDto>> Projection = m => new OutboundMessageDto
        {
            Id = m.Id,
            MessageBatchId = m.MessageBatchId,
            BatchTitle = m.MessageBatch!.Title,
            MessageTypeCode = m.MessageTypeCode,
            Channel = m.Channel,
            RecipientType = m.RecipientType,
            RecipientName = m.RecipientName,
            Destination = m.Destination,
            OriginalDestination = m.OriginalDestination,
            Subject = m.Subject,
            Body = m.Body,
            Status = m.Status,
            Attempts = m.Attempts,
            NextAttemptAt = m.NextAttemptAt,
            SentAt = m.SentAt,
            DeliveredAt = m.DeliveredAt,
            ProviderMessageId = m.ProviderMessageId,
            ProviderStatus = m.ProviderStatus,
            ErrorMessage = m.ErrorMessage,
            SmsParts = m.SmsParts,
            IsTest = m.IsTest,
            Created = m.Created,
            CreatedBy = m.CreatedBy
        };
    }
}
