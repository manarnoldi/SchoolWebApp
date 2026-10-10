using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using SchoolWebApp.Core.Entities.Communications;

namespace SchoolWebApp.API.Services.Communications
{
    public record EmailItem(string To, string? ToName, string Subject, string HtmlBody);

    /// <summary>
    /// Sends email through the school's SMTP account. MailKit rather than
    /// System.Net.Mail: the latter cannot do implicit TLS, which port 465 (the
    /// usual cPanel / hosting setup) requires.
    /// </summary>
    public class SmtpEmailGateway
    {
        public static string? ConfigurationProblem(CommunicationSetting s)
        {
            if (string.IsNullOrWhiteSpace(s.SmtpHost)) return "The SMTP host is not set.";
            if (s.SmtpPort <= 0) return "The SMTP port is not set.";
            if (string.IsNullOrWhiteSpace(s.FromEmail)) return "The sender (from) email is not set.";
            return null;
        }

        /// <summary>
        /// Opens one connection for the whole set. Returns, per item, null on
        /// success or the error. A connection / login failure fails them all.
        /// </summary>
        public async Task<List<string?>> SendAsync(CommunicationSetting s, IReadOnlyList<EmailItem> items, CancellationToken ct)
        {
            var results = new List<string?>();
            using var client = new SmtpClient { Timeout = 30000 };
            try
            {
                var security = !s.SmtpUseSsl ? SecureSocketOptions.None
                    : s.SmtpPort == 465 ? SecureSocketOptions.SslOnConnect
                    : SecureSocketOptions.StartTls;
                await client.ConnectAsync(s.SmtpHost, s.SmtpPort, security, ct);
                if (!string.IsNullOrWhiteSpace(s.SmtpUsername))
                    await client.AuthenticateAsync(s.SmtpUsername, s.SmtpPassword ?? "", ct);
            }
            catch (Exception ex) when (ex is not OperationCanceledException || !ct.IsCancellationRequested)
            {
                return items.Select(_ => (string?)("SMTP connection failed: " + ex.Message)).ToList();
            }

            foreach (var item in items)
            {
                try
                {
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress(s.FromName ?? s.FromEmail, s.FromEmail));
                    message.To.Add(new MailboxAddress(item.ToName ?? item.To, item.To));
                    if (!string.IsNullOrWhiteSpace(s.ReplyToEmail))
                        message.ReplyTo.Add(MailboxAddress.Parse(s.ReplyToEmail));
                    message.Subject = item.Subject;
                    message.Body = new BodyBuilder { HtmlBody = item.HtmlBody }.ToMessageBody();
                    await client.SendAsync(message, ct);
                    results.Add(null);
                }
                catch (Exception ex) when (ex is not OperationCanceledException || !ct.IsCancellationRequested)
                {
                    results.Add(ex.Message);
                }
            }

            try { await client.DisconnectAsync(true, ct); } catch { /* the messages are already handed over */ }
            return results;
        }
    }
}
