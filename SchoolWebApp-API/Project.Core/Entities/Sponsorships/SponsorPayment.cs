using SchoolWebApp.Core.Entities.Finance;
using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Sponsorships
{
    /// <summary>
    /// Records money actually received from a sponsor against their outstanding receivable.
    /// Journal: Debit Bank/Cash, Credit Sponsor Receivable.
    /// </summary>
    public class SponsorPayment : Base
    {
        public int SponsorId { get; set; }
        public Sponsor? Sponsor { get; set; }

        [Required]
        [StringLength(50)]
        public required string ReferenceNumber { get; set; }

        public DateTime PaymentDate { get; set; }

        public decimal Amount { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        [StringLength(100)]
        public string? TransactionReference { get; set; }

        public int? BankAccountId { get; set; }
        public Account? BankAccount { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }
    }
}
