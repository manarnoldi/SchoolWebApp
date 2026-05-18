using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Finance
{
    public enum JournalEntryStatus { Draft = 0, Submitted = 1, Approved = 2, Rejected = 3 }

    public class JournalEntry : Base
    {
        [Required]
        [StringLength(50)]
        public required string ReferenceNumber { get; set; }

        public DateTime EntryDate { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsPosted { get; set; }

        public JournalEntryStatus Status { get; set; } = JournalEntryStatus.Draft;

        public List<JournalLine> Lines { get; set; } = new();
    }
}
