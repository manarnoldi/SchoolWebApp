using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.DTOs.Finance.JournalEntry
{
    public class CreateJournalLineDto
    {
        public int? Id { get; set; }
        public int AccountId { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string? Description { get; set; }
    }

    public class CreateJournalEntryDto
    {
        [StringLength(50)]
        public string? ReferenceNumber { get; set; }

        public DateTime EntryDate { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsPosted { get; set; } = true;

        public List<CreateJournalLineDto> Lines { get; set; } = new();
    }
}
