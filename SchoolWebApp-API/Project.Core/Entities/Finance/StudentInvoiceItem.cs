using SchoolWebApp.Core.Entities.Shared;
using SchoolWebApp.Core.Entities.Sponsorships;

namespace SchoolWebApp.Core.Entities.Finance
{
    public class StudentInvoiceItem : Base
    {
        public int StudentInvoiceId { get; set; }
        public StudentInvoice? StudentInvoice { get; set; }

        public int FeeCategoryId { get; set; }
        public FeeCategory? FeeCategory { get; set; }

        public decimal Amount { get; set; }
        public decimal Discount { get; set; }
        public decimal PaidAmount { get; set; }

        // When the Discount on this item is funded by a sponsor, the Sponsorship that covered it
        // is recorded here. Drives sponsor AR postings and reporting.
        public int? SponsorshipId { get; set; }
        public Sponsorship? Sponsorship { get; set; }

        public string? Description { get; set; }
    }
}
