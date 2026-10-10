using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Infrastructure.Data;
using SchoolWebApp.API.Services.Communications;
using SchoolWebApp.Core.DTOs.Communications;
using SchoolWebApp.Core.Entities.Communications;

namespace SchoolWebApp.API.Controllers.Communications
{
    /// <summary>
    /// SMS gateway and SMTP settings. SuperAdministrator only - the SMS account is
    /// billed separately, so schools do not change it themselves. The status
    /// endpoint is open to every signed-in user.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommunicationSettingsController : ControllerBase
    {
        private readonly ILogger<CommunicationSettingsController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly CommunicationService _communications;
        private readonly RecipientResolver _resolver;
        private readonly TextSmsGateway _sms;
        private readonly SmtpEmailGateway _email;

        public CommunicationSettingsController(ILogger<CommunicationSettingsController> logger, ApplicationDbContext db,
            CommunicationService communications, RecipientResolver resolver, TextSmsGateway sms, SmtpEmailGateway email)
        {
            _logger = logger; _db = db; _communications = communications; _resolver = resolver; _sms = sms; _email = email;
        }

        [HttpGet("status")]
        public async Task<IActionResult> Status()
        {
            var s = await _communications.GetSettingsAsync();
            return Ok(new CommunicationStatusDto
            {
                SmsEnabled = s.SmsEnabled,
                EmailEnabled = s.EmailEnabled,
                TestMode = s.TestMode,
                ParentContactSource = await _resolver.GetParentContactSourceAsync()
            });
        }

        [HttpGet]
        [Authorize(Policy = "SuperAdminRole")]
        public async Task<IActionResult> Get()
        {
            var s = await _communications.GetSettingsAsync();
            return Ok(new CommunicationSettingDto
            {
                SmsEnabled = s.SmsEnabled,
                SmsApiUrl = s.SmsApiUrl,
                HasSmsApiKey = !string.IsNullOrEmpty(s.SmsApiKey),
                SmsPartnerId = s.SmsPartnerId,
                SmsSenderId = s.SmsSenderId,
                SmsUnitPrice = s.SmsUnitPrice,
                EmailEnabled = s.EmailEnabled,
                SmtpHost = s.SmtpHost,
                SmtpPort = s.SmtpPort,
                SmtpUseSsl = s.SmtpUseSsl,
                SmtpUsername = s.SmtpUsername,
                HasSmtpPassword = !string.IsNullOrEmpty(s.SmtpPassword),
                FromEmail = s.FromEmail,
                FromName = s.FromName,
                ReplyToEmail = s.ReplyToEmail,
                TestMode = s.TestMode,
                TestPhoneNumber = s.TestPhoneNumber,
                TestEmail = s.TestEmail
            });
        }

        [HttpPut]
        [Authorize(Policy = "SuperAdminRole")]
        public async Task<IActionResult> Update(CommunicationSettingDto model)
        {
            var s = await _communications.GetSettingsAsync();
            if (model.SmsEnabled)
            {
                var probe = new CommunicationSetting
                {
                    SmsApiKey = string.IsNullOrWhiteSpace(model.SmsApiKey) ? s.SmsApiKey : model.SmsApiKey,
                    SmsPartnerId = model.SmsPartnerId,
                    SmsSenderId = model.SmsSenderId
                };
                var problem = TextSmsGateway.ConfigurationProblem(probe);
                if (problem != null) return BadRequest(problem + " SMS cannot be switched on without it.");
            }
            if (model.EmailEnabled)
            {
                var problem = SmtpEmailGateway.ConfigurationProblem(new CommunicationSetting
                    { SmtpHost = model.SmtpHost, SmtpPort = model.SmtpPort, FromEmail = model.FromEmail });
                if (problem != null) return BadRequest(problem + " Email cannot be switched on without it.");
            }
            if (!string.IsNullOrWhiteSpace(model.TestPhoneNumber) && MessageText.NormalizePhone(model.TestPhoneNumber) == null)
                return BadRequest("The test phone number is not a valid Kenyan mobile number.");

            s.SmsEnabled = model.SmsEnabled;
            s.SmsApiUrl = string.IsNullOrWhiteSpace(model.SmsApiUrl) ? TextSmsGateway.DefaultApiUrl : model.SmsApiUrl.Trim();
            if (!string.IsNullOrWhiteSpace(model.SmsApiKey)) s.SmsApiKey = model.SmsApiKey.Trim();
            s.SmsPartnerId = model.SmsPartnerId?.Trim();
            s.SmsSenderId = model.SmsSenderId?.Trim();
            s.SmsUnitPrice = model.SmsUnitPrice;
            s.EmailEnabled = model.EmailEnabled;
            s.SmtpHost = model.SmtpHost?.Trim();
            s.SmtpPort = model.SmtpPort;
            s.SmtpUseSsl = model.SmtpUseSsl;
            s.SmtpUsername = model.SmtpUsername?.Trim();
            if (!string.IsNullOrEmpty(model.SmtpPassword)) s.SmtpPassword = model.SmtpPassword;
            s.FromEmail = model.FromEmail?.Trim();
            s.FromName = model.FromName?.Trim();
            s.ReplyToEmail = model.ReplyToEmail?.Trim();
            s.TestMode = model.TestMode;
            s.TestPhoneNumber = model.TestPhoneNumber?.Trim();
            s.TestEmail = model.TestEmail?.Trim();
            await _db.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Sends one SMS straight through the gateway, bypassing the queue and test
        /// mode, and records it (as a test) so it shows in the queue.
        /// </summary>
        [HttpPost("testSms")]
        [Authorize(Policy = "SuperAdminRole")]
        public async Task<IActionResult> TestSms(GatewayTestDto model)
        {
            var s = await _communications.GetSettingsAsync();
            var problem = TextSmsGateway.ConfigurationProblem(s);
            if (problem != null) return BadRequest(problem);
            var phone = MessageText.NormalizePhone(model.Destination);
            if (phone == null) return BadRequest("Enter a valid Kenyan mobile number.");
            var text = MessageText.SanitizeSms(string.IsNullOrWhiteSpace(model.Message)
                ? "Test message from ShuleNova. SMS is set up correctly." : model.Message);

            var result = await _sms.SendSingleAsync(s, phone, text, HttpContext.RequestAborted);
            await RecordTestAsync(MessageChannel.Sms, phone, null, text, result.Success, result.MessageId, result.Description);
            return result.Success
                ? Ok(new { message = "SMS sent to " + phone, result.MessageId })
                : BadRequest("The gateway refused the SMS: " + result.Description);
        }

        [HttpPost("testEmail")]
        [Authorize(Policy = "SuperAdminRole")]
        public async Task<IActionResult> TestEmail(GatewayTestDto model)
        {
            var s = await _communications.GetSettingsAsync();
            var problem = SmtpEmailGateway.ConfigurationProblem(s);
            if (problem != null) return BadRequest(problem);
            var to = MessageText.NormalizeEmail(model.Destination);
            if (to == null) return BadRequest("Enter a valid email address.");
            var body = string.IsNullOrWhiteSpace(model.Message) ? "Test email from ShuleNova. Email is set up correctly." : model.Message;
            var subject = "ShuleNova test email";

            var errors = await _email.SendAsync(s, new[] { new EmailItem(to, null, subject, MessageText.ToEmailHtml(body, null)) }, HttpContext.RequestAborted);
            await RecordTestAsync(MessageChannel.Email, to, subject, body, errors[0] == null, null, errors[0] ?? "Accepted by mail server");
            return errors[0] == null ? Ok(new { message = "Email sent to " + to }) : BadRequest(errors[0]);
        }

        [HttpGet("smsBalance")]
        [Authorize(Policy = "SuperAdminRole")]
        public async Task<IActionResult> SmsBalance()
        {
            var s = await _communications.GetSettingsAsync();
            var problem = TextSmsGateway.ConfigurationProblem(s);
            if (problem != null) return BadRequest(problem);
            try
            {
                return Ok(new { raw = await _sms.GetBalanceAsync(s, HttpContext.RequestAborted) });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "SMS balance check failed");
                return BadRequest("Could not read the balance: " + ex.Message);
            }
        }

        private async Task RecordTestAsync(MessageChannel channel, string destination, string? subject, string body,
            bool success, string? providerId, string? providerStatus)
        {
            var message = new OutboundMessage
            {
                MessageTypeCode = MessageTypeCodes.Custom,
                Channel = channel,
                RecipientType = RecipientType.Other,
                RecipientName = "Gateway test",
                Destination = destination,
                Subject = subject,
                Body = body,
                Status = success ? MessageStatus.Sent : MessageStatus.Failed,
                Attempts = 1,
                SentAt = success ? DateTime.UtcNow : null,
                ProviderMessageId = providerId,
                ProviderStatus = success ? providerStatus : null,
                ErrorMessage = success ? null : providerStatus,
                SmsParts = channel == MessageChannel.Sms ? MessageText.SmsParts(body) : 0,
                IsTest = true
            };
            _db.MessageBatches.Add(new MessageBatch
            {
                Title = channel == MessageChannel.Sms ? "SMS gateway test" : "Email gateway test",
                MessageTypeCode = MessageTypeCodes.Custom,
                Channel = channel,
                RecipientSummary = destination,
                TotalMessages = 1,
                IsTest = true,
                Messages = { message }
            });
            await _db.SaveChangesAsync();
        }
    }
}
