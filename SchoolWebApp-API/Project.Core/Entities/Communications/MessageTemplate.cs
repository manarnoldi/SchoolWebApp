using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Communications
{
    /// <summary>
    /// Wording for a message, with {Placeholders} the system fills in per recipient.
    /// System templates (IsSystem, with a Code from <see cref="MessageTypeCodes"/>) are
    /// the standard message types - results, invoices, balance reminders - and their
    /// Channel is the school's choice of SMS, email or both for that type. Other
    /// templates are saved wording for custom messages.
    /// </summary>
    public class MessageTemplate : Base
    {
        [StringLength(50)]
        public string? Code { get; set; }

        [Required]
        [StringLength(150)]
        public required string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsSystem { get; set; }

        public MessageChannel Channel { get; set; } = MessageChannel.Sms;

        public bool IsActive { get; set; } = true;

        public string? SmsBody { get; set; }

        [StringLength(255)]
        public string? EmailSubject { get; set; }

        public string? EmailBody { get; set; }
    }
}
