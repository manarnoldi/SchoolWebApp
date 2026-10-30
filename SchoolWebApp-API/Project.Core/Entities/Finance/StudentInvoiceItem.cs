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

        // The sponsorship that has pledged part of this fee. The student is invoiced the whole fee and stays
        // responsible for it; SponsoredAmount is the part the sponsor has promised to pay, and SponsorPaidAmount
        // how much of it the sponsor has paid so far (also counted in PaidAmount). Discount is a real discount only.
        public int? SponsorshipId { get; set; }
        public Sponsorship? Sponsorship { get; set; }
        public decimal SponsoredAmount { get; set; }
        public decimal SponsorPaidAmount { get; set; }

        public string? Description { get; set; }
    }
}
