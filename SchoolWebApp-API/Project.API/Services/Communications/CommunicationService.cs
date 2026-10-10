using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.DTOs.Communications;
using SchoolWebApp.Core.Entities.Communications;

namespace SchoolWebApp.API.Services.Communications
{
    /// <summary>What to send, before it is rendered per recipient.</summary>
    public class BatchSpec
    {
        public required string Title { get; set; }
        public required string MessageTypeCode { get; set; }
        public int? MessageTemplateId { get; set; }
        public MessageChannel Channel { get; set; }
        public string? RecipientSummary { get; set; }
        public string? SmsBody { get; set; }
        public string? EmailSubject { get; set; }
        public string? EmailBody { get; set; }

        /// <summary>
        /// Send one message per phone / email even when it belongs to several
        /// recipients - a parent with three learners gets one custom notice, the
        /// learners' names joined. Off for per-learner content (results, invoices).
        /// </summary>
        public bool MergeByDestination { get; set; }
    }

    public class CommunicationException : Exception
    {
        public CommunicationException(string message) : base(message) { }
    }

    /// <summary>
    /// Wakes the dispatch worker as soon as something is queued, instead of
    /// waiting out its polling interval.
    /// </summary>
    public class MessageDispatchSignal
    {
        private readonly SemaphoreSlim _signal = new(0);
        public void Notify() { if (_signal.CurrentCount == 0) _signal.Release(); }
        public Task WaitAsync(TimeSpan timeout, CancellationToken ct) => _signal.WaitAsync(timeout, ct);
    }

    /// <summary>
    /// Renders messages and puts them on the queue; the worker does the sending.
    /// </summary>
    public class CommunicationService
    {
        // In test mode only this many messages per channel in a batch actually go
        // out (to the test number / address) - enough to see the wording on a
        // phone without a 500-parent send costing 500 SMS. The rest are recorded
        // as sent without leaving the system.
        public const int TestModeLiveLimit = 3;

        private readonly ApplicationDbContext _db;
        private readonly RecipientResolver _resolver;
        private readonly MessageDispatchSignal _signal;

        public CommunicationService(ApplicationDbContext db, RecipientResolver resolver, MessageDispatchSignal signal)
        {
            _db = db;
            _resolver = resolver;
            _signal = signal;
        }

        public async Task<CommunicationSetting> GetSettingsAsync(CancellationToken ct = default)
        {
            var s = await _db.CommunicationSettings.OrderBy(x => x.Id).FirstOrDefaultAsync(ct);
            if (s != null) return s;
            s = new CommunicationSetting { SmsApiUrl = TextSmsGateway.DefaultApiUrl, TestMode = true };
            _db.CommunicationSettings.Add(s);
            await _db.SaveChangesAsync(ct);
            return s;
        }

        public async Task<Dictionary<string, string?>> SchoolValuesAsync(CancellationToken ct = default)
        {
            var school = await _db.SchoolDetails.AsNoTracking().OrderBy(x => x.Id)
                .Select(x => new { x.Name, x.Telephone, x.Email, x.Initials }).FirstOrDefaultAsync(ct);
            return new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
            {
                ["SchoolName"] = school?.Name,
                ["SchoolPhone"] = school?.Telephone,
                ["SchoolEmail"] = school?.Email,
                ["SchoolInitials"] = school?.Initials,
                ["Date"] = DateTime.UtcNow.Add(MessageText.SchoolUtcOffset).ToString("dd MMM yyyy")
            };
        }

        public async Task<MessageTemplate> SystemTemplateAsync(string code, CancellationToken ct = default)
        {
            var t = await _db.MessageTemplates.AsNoTracking().FirstOrDefaultAsync(x => x.Code == code, ct)
                ?? throw new CommunicationException($"The '{code}' message template is missing.");
            if (!t.IsActive)
                throw new CommunicationException($"The '{t.Name}' message type is switched off. Turn it on under Communications > Templates.");
            if (t.Channel == MessageChannel.None)
                throw new CommunicationException($"No channel (SMS / email) is chosen for '{t.Name}'.");
            return t;
        }

        /// <summary>
        /// Checks the chosen channel can be used now. In test mode a channel that
        /// is not yet configured may still be tried - nothing real goes out.
        /// </summary>
        public static void EnsureChannelUsable(CommunicationSetting s, MessageChannel channel)
        {
            if (channel == MessageChannel.None)
                throw new CommunicationException("Choose SMS, email or both.");
            if (s.TestMode) return;
            if (channel.HasFlag(MessageChannel.Sms) && !s.SmsEnabled)
                throw new CommunicationException("SMS sending is switched off for this school.");
            if (channel.HasFlag(MessageChannel.Email) && !s.EmailEnabled)
                throw new CommunicationException("Email sending is switched off for this school.");
        }

        private class Pending
        {
            public required ContactTarget Target { get; set; }
            public required MessageChannel Channel { get; set; }
            public required string Destination { get; set; }
            public List<ContactTarget> Merged { get; } = new();
        }

        private static List<Pending> Expand(BatchSpec spec, IEnumerable<ContactTarget> targets)
        {
            var list = new List<Pending>();
            foreach (var t in targets)
            {
                if (spec.Channel.HasFlag(MessageChannel.Sms) && t.Phone != null)
                    list.Add(new Pending { Target = t, Channel = MessageChannel.Sms, Destination = t.Phone });
                if (spec.Channel.HasFlag(MessageChannel.Email) && t.Email != null)
                    list.Add(new Pending { Target = t, Channel = MessageChannel.Email, Destination = t.Email });
            }
            if (!spec.MergeByDestination) return list;

            var merged = new List<Pending>();
            foreach (var g in list.GroupBy(p => (p.Channel, p.Destination.ToLowerInvariant())))
            {
                var first = g.First();
                first.Merged.AddRange(g.Select(p => p.Target));
                merged.Add(first);
            }
            return merged;
        }

        private static Dictionary<string, string?> ValuesFor(Pending p, Dictionary<string, string?> school)
        {
            var values = new Dictionary<string, string?>(school, StringComparer.OrdinalIgnoreCase);
            foreach (var kv in p.Target.Values) values[kv.Key] = kv.Value;
            if (p.Channel == MessageChannel.Email)
                foreach (var kv in p.Target.EmailValues) values[kv.Key] = kv.Value;
            if (p.Merged.Count > 1)
            {
                // One message for several learners: list them all.
                foreach (var key in new[] { "StudentName", "AdmissionNo", "ClassName" })
                {
                    var joined = string.Join(", ", p.Merged.Select(t => t.Values.GetValueOrDefault(key))
                        .Where(v => !string.IsNullOrWhiteSpace(v)).Distinct());
                    if (joined.Length > 0) values[key] = joined;
                }
            }
            return values;
        }

        private static OutboundMessage Render(BatchSpec spec, Pending p, Dictionary<string, string?> school)
        {
            var values = ValuesFor(p, school);
            var isSms = p.Channel == MessageChannel.Sms;
            var body = isSms
                ? MessageText.SanitizeSms(MessageText.Render(spec.SmsBody, values))
                : MessageText.Render(spec.EmailBody, values);
            return new OutboundMessage
            {
                MessageTypeCode = spec.MessageTypeCode,
                Channel = p.Channel,
                RecipientType = p.Target.Type,
                PersonId = p.Target.PersonId,
                StudentId = p.Target.StudentId,
                RecipientName = p.Target.Name,
                Destination = p.Destination,
                Subject = isSms ? null : MessageText.Render(spec.EmailSubject, values),
                Body = body,
                SmsParts = isSms ? MessageText.SmsParts(body) : 0
            };
        }

        public async Task<ComposePreviewDto> PreviewAsync(BatchSpec spec, IReadOnlyCollection<ContactTarget> targets, CancellationToken ct = default)
        {
            var s = await GetSettingsAsync(ct);
            var school = await SchoolValuesAsync(ct);
            var pending = Expand(spec, targets);
            var messages = pending.Select(p => Render(spec, p, school)).ToList();
            var firstSms = messages.FirstOrDefault(m => m.Channel == MessageChannel.Sms);
            var firstEmail = messages.FirstOrDefault(m => m.Channel == MessageChannel.Email);

            return new ComposePreviewDto
            {
                Recipients = targets.Count(t => t.Phone != null || t.Email != null),
                SmsCount = messages.Count(m => m.Channel == MessageChannel.Sms),
                EmailCount = messages.Count(m => m.Channel == MessageChannel.Email),
                MissingContact = targets.Count(t =>
                    !(spec.Channel.HasFlag(MessageChannel.Sms) && t.Phone != null) &&
                    !(spec.Channel.HasFlag(MessageChannel.Email) && t.Email != null)),
                SmsParts = messages.Sum(m => m.SmsParts),
                SampleSms = firstSms?.Body ?? (spec.Channel.HasFlag(MessageChannel.Sms)
                    ? MessageText.SanitizeSms(MessageText.Render(spec.SmsBody, school)) : null),
                SampleEmailSubject = firstEmail?.Subject,
                SampleEmailBody = firstEmail?.Body,
                TestMode = s.TestMode,
                RecipientList = targets.Select(t => new RecipientPreviewDto
                {
                    Name = t.Name,
                    RecipientType = t.Type,
                    StudentNames = t.Values.GetValueOrDefault("StudentName"),
                    Phone = t.Phone,
                    Email = t.Email
                }).ToList()
            };
        }

        /// <summary>
        /// Renders one message per recipient and channel and queues them as a batch.
        /// </summary>
        public async Task<QueueResultDto> QueueAsync(BatchSpec spec, IReadOnlyCollection<ContactTarget> targets, CancellationToken ct = default)
        {
            var s = await GetSettingsAsync(ct);
            EnsureChannelUsable(s, spec.Channel);
            if (spec.Channel.HasFlag(MessageChannel.Sms) && string.IsNullOrWhiteSpace(spec.SmsBody))
                throw new CommunicationException("The SMS text is empty.");
            if (spec.Channel.HasFlag(MessageChannel.Email) && (string.IsNullOrWhiteSpace(spec.EmailBody) || string.IsNullOrWhiteSpace(spec.EmailSubject)))
                throw new CommunicationException("The email subject or body is empty.");

            var school = await SchoolValuesAsync(ct);
            var pending = Expand(spec, targets);
            if (pending.Count == 0)
                throw new CommunicationException("None of the recipients has a phone number or email address for the chosen channel.");

            var messages = pending.Select(p => Render(spec, p, school)).ToList();
            if (s.TestMode)
            {
                var testPhone = MessageText.NormalizePhone(s.TestPhoneNumber);
                var testEmail = MessageText.NormalizeEmail(s.TestEmail);
                foreach (var group in messages.GroupBy(m => m.Channel))
                {
                    var testDestination = group.Key == MessageChannel.Sms ? testPhone : testEmail;
                    var live = 0;
                    foreach (var m in group)
                    {
                        m.IsTest = true;
                        // Redirected messages keep the real recipient in
                        // OriginalDestination; the dispatch worker simulates
                        // test messages that have none.
                        if (testDestination != null && live++ < TestModeLiveLimit)
                        {
                            m.OriginalDestination = m.Destination;
                            m.Destination = testDestination;
                        }
                    }
                }
            }

            var batch = new MessageBatch
            {
                Title = spec.Title,
                MessageTypeCode = spec.MessageTypeCode,
                MessageTemplateId = spec.MessageTemplateId,
                Channel = spec.Channel,
                RecipientSummary = spec.RecipientSummary,
                TotalMessages = messages.Count,
                IsTest = s.TestMode,
                Messages = messages
            };
            _db.MessageBatches.Add(batch);
            await _db.SaveChangesAsync(ct);
            _signal.Notify();

            return new QueueResultDto
            {
                BatchId = batch.Id,
                Queued = messages.Count,
                Skipped = targets.Count(t =>
                    !(spec.Channel.HasFlag(MessageChannel.Sms) && t.Phone != null) &&
                    !(spec.Channel.HasFlag(MessageChannel.Email) && t.Email != null)),
                TestMode = s.TestMode
            };
        }

        public async Task<BatchSpec> CustomSpecAsync(ComposeMessageDto dto, CancellationToken ct = default)
        {
            var summary = await _resolver.DescribeAsync(dto.Recipients, ct);
            return new BatchSpec
            {
                Title = string.IsNullOrWhiteSpace(dto.Title) ? "Custom message - " + summary : dto.Title.Trim(),
                MessageTypeCode = MessageTypeCodes.Custom,
                MessageTemplateId = dto.MessageTemplateId,
                Channel = dto.Channel,
                RecipientSummary = summary,
                SmsBody = dto.SmsBody,
                EmailSubject = dto.EmailSubject,
                EmailBody = dto.EmailBody,
                MergeByDestination = true
            };
        }
    }
}
