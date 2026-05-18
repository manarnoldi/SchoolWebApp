namespace SchoolWebApp.Core.DTOs.Finance.JournalEntry
{
    public class JournalLineDto : CreateJournalLineDto
    {
        public string? AccountCode { get; set; }
        public string? AccountName { get; set; }
    }

    public class JournalEntryDto
    {
        public int Id { get; set; }
        public string? ReferenceNumber { get; set; }
        public DateTime EntryDate { get; set; }
        public string? Description { get; set; }
        public bool IsPosted { get; set; }
        public int Status { get; set; }
        public string? StatusLabel { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public List<JournalLineDto> Lines { get; set; } = new();
    }

    public class RejectJournalDto
    {
        public string Reason { get; set; } = string.Empty;
    }
}
