using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.Entities.Communications;

namespace SchoolWebApp.API.Services.Communications
{
    /// <summary>
    /// Drains the message queue in the background: sends due Queued messages,
    /// retries failures with a growing delay, and polls the SMS gateway for
    /// delivery reports. Woken early by <see cref="MessageDispatchSignal"/>.
    ///
    /// Runs inside the API process, so on IIS it pauses while the app pool is
    /// idle-stopped; queued messages simply wait for the next start.
    /// </summary>
    public class MessageDispatchWorker : BackgroundService
    {
        private const int BatchSize = 100;
        public const int MaxAttempts = 5;
        private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(30);
        private static readonly TimeSpan DeliveryCheckInterval = TimeSpan.FromMinutes(2);
        private const int MaxDeliveryChecks = 8;

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly MessageDispatchSignal _signal;
        private readonly ILogger<MessageDispatchWorker> _logger;
        private DateTime _lastDeliveryCheck = DateTime.MinValue;

        public MessageDispatchWorker(IServiceScopeFactory scopeFactory, MessageDispatchSignal signal, ILogger<MessageDispatchWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _signal = signal;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Let startup (migrations, bootstrapper) finish first.
            try { await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); } catch (OperationCanceledException) { return; }
            await RecoverInterruptedAsync(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                var more = false;
                try
                {
                    more = await DispatchDueAsync(stoppingToken);
                    if (DateTime.UtcNow - _lastDeliveryCheck > DeliveryCheckInterval)
                    {
                        _lastDeliveryCheck = DateTime.UtcNow;
                        await CheckDeliveryAsync(stoppingToken);
                    }
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Message dispatch cycle failed");
                }

                if (more) continue;
                try { await _signal.WaitAsync(PollInterval, stoppingToken); }
                catch (OperationCanceledException) { break; }
            }
        }

        /// <summary>
        /// Messages left "Sending" by a stop mid-dispatch go back on the queue. A
        /// message the gateway had already accepted may go twice; losing it silently
        /// would be worse.
        /// </summary>
        private async Task RecoverInterruptedAsync(CancellationToken ct)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await db.OutboundMessages
                    .Where(m => m.Status == MessageStatus.Sending)
                    .ExecuteUpdateAsync(u => u.SetProperty(m => m.Status, MessageStatus.Queued), ct);
            }
            catch (Exception ex) when (!ct.IsCancellationRequested)
            {
                _logger.LogError(ex, "Could not recover interrupted messages");
            }
        }

        /// <returns>true when a full batch was taken, i.e. more may be waiting.</returns>
        private async Task<bool> DispatchDueAsync(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var now = DateTime.UtcNow;

            var due = await db.OutboundMessages
                .Where(m => m.Status == MessageStatus.Queued && (m.NextAttemptAt == null || m.NextAttemptAt <= now))
                .OrderBy(m => m.Id)
                .Take(BatchSize)
                .ToListAsync(ct);
            if (due.Count == 0) return false;

            var settings = await db.CommunicationSettings.AsNoTracking().OrderBy(s => s.Id).FirstOrDefaultAsync(ct)
                           ?? new CommunicationSetting { TestMode = true };

            foreach (var m in due) m.Status = MessageStatus.Sending;
            await db.SaveChangesAsync(ct);

            // Test messages with no test destination never leave the system.
            foreach (var m in due.Where(m => m.IsTest && m.OriginalDestination == null))
                MarkSent(m, null, "Test mode - not sent");

            var sms = due.Where(m => m.Status == MessageStatus.Sending && m.Channel == MessageChannel.Sms).ToList();
            var email = due.Where(m => m.Status == MessageStatus.Sending && m.Channel == MessageChannel.Email).ToList();

            if (sms.Count > 0) await SendSmsAsync(scope, settings, sms, ct);
            if (email.Count > 0) await SendEmailAsync(scope, settings, email, ct);

            await db.SaveChangesAsync(ct);
            return due.Count == BatchSize;
        }

        private async Task SendSmsAsync(IServiceScope scope, CommunicationSetting s, List<OutboundMessage> messages, CancellationToken ct)
        {
            // Test messages redirected to the test number go out even while SMS is
            // switched off - that is how a school tries the gateway before going live.
            var problem = TextSmsGateway.ConfigurationProblem(s);
            var blocked = messages.Where(m => !m.IsTest && !s.SmsEnabled).ToList();
            foreach (var m in blocked) Defer(m, "SMS sending is switched off.");
            var sendable = messages.Except(blocked).ToList();
            if (problem != null)
            {
                foreach (var m in sendable) Defer(m, problem);
                return;
            }

            var gateway = scope.ServiceProvider.GetRequiredService<TextSmsGateway>();
            foreach (var chunk in sendable.Chunk(TextSmsGateway.BulkLimit))
            {
                var items = chunk.Select(m => new SmsSendItem(m.Id.ToString(), m.Destination, m.Body)).ToList();
                var results = await gateway.SendBulkAsync(s, items, ct);
                foreach (var m in chunk)
                {
                    var r = results.FirstOrDefault(x => x.ClientId == m.Id.ToString());
                    m.Attempts++;
                    if (r != null && r.Success) MarkSent(m, r.MessageId, r.Description);
                    else Fail(m, r?.Description ?? "No response", r?.Retryable ?? true);
                }
            }
        }

        private async Task SendEmailAsync(IServiceScope scope, CommunicationSetting s, List<OutboundMessage> messages, CancellationToken ct)
        {
            var problem = SmtpEmailGateway.ConfigurationProblem(s);
            var blocked = messages.Where(m => !m.IsTest && !s.EmailEnabled).ToList();
            foreach (var m in blocked) Defer(m, "Email sending is switched off.");
            var sendable = messages.Except(blocked).ToList();
            if (problem != null)
            {
                foreach (var m in sendable) Defer(m, problem);
                return;
            }
            if (sendable.Count == 0) return;

            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var schoolName = await db.SchoolDetails.AsNoTracking().OrderBy(x => x.Id).Select(x => x.Name).FirstOrDefaultAsync(ct);
            var gateway = scope.ServiceProvider.GetRequiredService<SmtpEmailGateway>();
            var items = sendable.Select(m => new EmailItem(m.Destination, m.RecipientName, m.Subject ?? "",
                MessageText.ToEmailHtml(m.Body, schoolName))).ToList();
            var errors = await gateway.SendAsync(s, items, ct);
            for (var i = 0; i < sendable.Count; i++)
            {
                var m = sendable[i];
                m.Attempts++;
                if (errors[i] == null) MarkSent(m, null, "Accepted by mail server");
                else Fail(m, errors[i]!, retryable: true);
            }
        }

        private static void MarkSent(OutboundMessage m, string? providerId, string? providerStatus)
        {
            m.Status = MessageStatus.Sent;
            m.SentAt = DateTime.UtcNow;
            m.ProviderMessageId = providerId;
            m.ProviderStatus = Truncate(providerStatus, 255);
            m.ErrorMessage = null;
            m.NextAttemptAt = null;
        }

        private static void Fail(OutboundMessage m, string error, bool retryable)
        {
            m.ErrorMessage = Truncate(error, 1000);
            if (retryable && m.Attempts < MaxAttempts)
            {
                // 2, 4, 8, 16 minutes.
                m.Status = MessageStatus.Queued;
                m.NextAttemptAt = DateTime.UtcNow.AddMinutes(Math.Pow(2, m.Attempts));
            }
            else
            {
                m.Status = MessageStatus.Failed;
                m.NextAttemptAt = null;
            }
        }

        /// <summary>Not an attempt - the school's setup is not ready. Look again in 10 minutes.</summary>
        private static void Defer(OutboundMessage m, string reason)
        {
            m.Status = MessageStatus.Queued;
            m.ErrorMessage = reason;
            m.NextAttemptAt = DateTime.UtcNow.AddMinutes(10);
        }

        private async Task CheckDeliveryAsync(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var settings = await db.CommunicationSettings.AsNoTracking().OrderBy(s => s.Id).FirstOrDefaultAsync(ct);
            if (settings == null || TextSmsGateway.ConfigurationProblem(settings) != null) return;

            var since = DateTime.UtcNow.AddDays(-3);
            var settle = DateTime.UtcNow.AddMinutes(-1);
            var pending = await db.OutboundMessages
                .Where(m => m.Channel == MessageChannel.Sms && m.Status == MessageStatus.Sent
                            && m.ProviderMessageId != null && m.SentAt > since && m.SentAt < settle
                            && m.DeliveryChecks < MaxDeliveryChecks)
                .OrderBy(m => m.DeliveryChecks).ThenBy(m => m.Id)
                .Take(50)
                .ToListAsync(ct);
            if (pending.Count == 0) return;

            var gateway = scope.ServiceProvider.GetRequiredService<TextSmsGateway>();
            foreach (var m in pending)
            {
                m.DeliveryChecks++;
                try
                {
                    var r = await gateway.GetDeliveryAsync(settings, m.ProviderMessageId!, ct);
                    if (r.ProviderStatus != null) m.ProviderStatus = Truncate(r.ProviderStatus, 255);
                    if (r.Status != null)
                    {
                        m.Status = r.Status.Value;
                        if (r.Status == MessageStatus.Delivered) m.DeliveredAt = DateTime.UtcNow;
                    }
                }
                catch (Exception ex) when (!ct.IsCancellationRequested)
                {
                    _logger.LogWarning(ex, "Delivery report check failed for message {Id}", m.Id);
                }
            }
            await db.SaveChangesAsync(ct);
        }

        private static string? Truncate(string? s, int n) => s == null || s.Length <= n ? s : s[..n];
    }
}
