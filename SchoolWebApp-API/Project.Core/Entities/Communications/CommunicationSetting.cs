using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolWebApp.Core.Entities.Communications
{
    /// <summary>
    /// The school's SMS gateway and SMTP account - a single row. Kept out of
    /// GlobalSettings because those are readable by every signed-in user, and this
    /// row holds the gateway API key and the SMTP password. Only a
    /// SuperAdministrator can read or change it.
    /// </summary>
    public class CommunicationSetting : Base
    {
        public bool SmsEnabled { get; set; }

        [StringLength(255)]
        public string? SmsApiUrl { get; set; }

        [StringLength(255)]
        public string? SmsApiKey { get; set; }

        [StringLength(100)]
        public string? SmsPartnerId { get; set; }

        [StringLength(50)]
        public string? SmsSenderId { get; set; }

        /// <summary>What the school is charged per SMS part, for the monthly SMS report.</summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal SmsUnitPrice { get; set; }

        public bool EmailEnabled { get; set; }

        [StringLength(255)]
        public string? SmtpHost { get; set; }

        public int SmtpPort { get; set; } = 587;

        public bool SmtpUseSsl { get; set; } = true;

        [StringLength(255)]
        public string? SmtpUsername { get; set; }

        [StringLength(255)]
        public string? SmtpPassword { get; set; }

        [StringLength(255)]
        public string? FromEmail { get; set; }

        [StringLength(255)]
        public string? FromName { get; set; }

        [StringLength(255)]
        public string? ReplyToEmail { get; set; }

        /// <summary>
        /// While on, every message is delivered to the test number / address below
        /// instead of its real recipient, and is flagged as a test so it stays out
        /// of the billable SMS count. With no test destination set, messages are
        /// marked sent without leaving the system.
        /// </summary>
        public bool TestMode { get; set; } = true;

        [StringLength(50)]
        public string? TestPhoneNumber { get; set; }

        [StringLength(255)]
        public string? TestEmail { get; set; }
    }
}
