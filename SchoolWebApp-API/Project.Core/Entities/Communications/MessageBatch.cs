using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Communications
{
    /// <summary>
    /// One send action - a custom message to a group, or results / invoices /
    /// reminders for a selection of learners - and the messages it queued.
    /// </summary>
    public class MessageBatch : Base
    {
        [Required]
        [StringLength(255)]
        public required string Title { get; set; }

        [Required]
        [StringLength(50)]
        public required string MessageTypeCode { get; set; }

        public int? MessageTemplateId { get; set; }
        public MessageTemplate? MessageTemplate { get; set; }

        public MessageChannel Channel { get; set; }

        /// <summary>Who it went to, in words: "Parents of Grade 4 East", "12 selected teachers".</summary>
        [StringLength(500)]
        public string? RecipientSummary { get; set; }

        public int TotalMessages { get; set; }

        public bool IsTest { get; set; }

        public List<OutboundMessage> Messages { get; set; } = new();
    }
}
