using SchoolWebApp.Core.Entities.Finance;
using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Sponsorships
{
    public enum SponsorType
    {
        External = 0,   // Outside body (NGO, foundation, company)
        School = 1,     // Internal (school bursary fund)
        Government = 2,
        Parent = 3,
        Other = 4
    }

    public class Sponsor : Base
    {
        [Required]
        [StringLength(200)]
        public required string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public SponsorType SponsorType { get; set; } = SponsorType.External;

        [StringLength(100)]
        public string? ContactName { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(30)]
        public string? Phone { get; set; }

        [StringLength(300)]
        public string? Address { get; set; }

        // Asset / receivable account used to track AR from this sponsor.
        public int? ReceivableAccountId { get; set; }
        public Account? ReceivableAccount { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
