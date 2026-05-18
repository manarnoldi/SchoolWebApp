using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.Finance
{
    public class JournalLine : Base
    {
        public int JournalEntryId { get; set; }
        public JournalEntry? JournalEntry { get; set; }

        public int AccountId { get; set; }
        public Account? Account { get; set; }

        public decimal Debit { get; set; }
        public decimal Credit { get; set; }

        public string? Description { get; set; }
    }
}
