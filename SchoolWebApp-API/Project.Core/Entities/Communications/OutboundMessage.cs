using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Communications
{
    /// <summary>
    /// A single SMS or email to one recipient, already rendered. The dispatch
    /// worker picks up Queued rows whose NextAttemptAt has passed.
    /// </summary>
    public class OutboundMessage : Base
    {
        public int MessageBatchId { get; set; }
        public MessageBatch? MessageBatch { get; set; }

        [Required]
        [StringLength(50)]
        public required string MessageTypeCode { get; set; }

        public MessageChannel Channel { get; set; }

        public RecipientType RecipientType { get; set; }

        public int? PersonId { get; set; }

        public int? StudentId { get; set; }

        [StringLength(255)]
        public string? RecipientName { get; set; }

        /// <summary>The phone number (2547XXXXXXXX) or email address it is sent to.</summary>
        [Required]
        [StringLength(255)]
        public required string Destination { get; set; }

        /// <summary>The real recipient's address when test mode redirected the message.</summary>
        [StringLength(255)]
        public string? OriginalDestination { get; set; }

        [StringLength(255)]
        public string? Subject { get; set; }

        [Required]
        public required string Body { get; set; }

        public MessageStatus Status { get; set; } = MessageStatus.Queued;

        public int Attempts { get; set; }

        public DateTime? NextAttemptAt { get; set; }

        public DateTime? SentAt { get; set; }

        public DateTime? DeliveredAt { get; set; }

        [StringLength(100)]
        public string? ProviderMessageId { get; set; }

        [StringLength(255)]
        public string? ProviderStatus { get; set; }

        [StringLength(1000)]
        public string? ErrorMessage { get; set; }

        /// <summary>Billable SMS parts (160 GSM-7 / 70 Unicode characters each); 0 for email.</summary>
        public int SmsParts { get; set; }

        public bool IsTest { get; set; }

        public int DeliveryChecks { get; set; }
    }
}
