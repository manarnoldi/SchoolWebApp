using SchoolWebApp.Core.Entities.Finance;
using SchoolWebApp.Core.Entities.Sponsorships;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.DTOs.Sponsorships
{
    // ============ Sponsor ============
    public class SponsorDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public SponsorType SponsorType { get; set; }
        public string? ContactName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public int? ReceivableAccountId { get; set; }
        public string? ReceivableAccountName { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class CreateSponsorDto
    {
        [Required, StringLength(200)] public string Name { get; set; } = "";
        [StringLength(500)] public string? Description { get; set; }
        public SponsorType SponsorType { get; set; }
        [StringLength(100)] public string? ContactName { get; set; }
        [StringLength(100)] public string? Email { get; set; }
        [StringLength(30)] public string? Phone { get; set; }
        [StringLength(300)] public string? Address { get; set; }
        public int? ReceivableAccountId { get; set; }
        public bool IsActive { get; set; } = true;
    }

    // ============ Sponsorship ============
    public class SponsorshipDto
    {
        public int Id { get; set; }
        public int SponsorId { get; set; }
        public string? SponsorName { get; set; }
        public int? StudentId { get; set; }
        public string? StudentName { get; set; }
        public string? StudentUPI { get; set; }
        public int? SchoolClassId { get; set; }
        public string? SchoolClassName { get; set; }
        public int AcademicYearId { get; set; }
        public string? AcademicYearName { get; set; }
        public int? SessionId { get; set; }
        public string? SessionName { get; set; }
        public SponsorshipCoverageType CoverageType { get; set; }
        public decimal FixedAmount { get; set; }
        public decimal Percentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Notes { get; set; }
        public SponsorshipStatus Status { get; set; }
        public List<int> FeeCategoryIds { get; set; } = new();
    }

    public class CreateSponsorshipDto
    {
        [Required] public int SponsorId { get; set; }
        public int? StudentId { get; set; }
        public int? SchoolClassId { get; set; }
        [Required] public int AcademicYearId { get; set; }
        public int? SessionId { get; set; }
        public SponsorshipCoverageType CoverageType { get; set; }
        public decimal FixedAmount { get; set; }
        [Range(0, 100)] public decimal Percentage { get; set; }
        [Required] public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [StringLength(500)] public string? Notes { get; set; }
        public SponsorshipStatus Status { get; set; } = SponsorshipStatus.Active;
        public List<int> FeeCategoryIds { get; set; } = new();
    }

    // ============ Sponsor Payment ============
    public class SponsorPaymentDto
    {
        public int Id { get; set; }
        public int SponsorId { get; set; }
        public string? SponsorName { get; set; }
        public string ReferenceNumber { get; set; } = "";
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string? TransactionReference { get; set; }
        public int? BankAccountId { get; set; }
        public string? BankAccountName { get; set; }
        public string? Description { get; set; }
    }

    public class CreateSponsorPaymentDto
    {
        [Required] public int SponsorId { get; set; }
        [StringLength(50)] public string? ReferenceNumber { get; set; }
        [Required] public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        [StringLength(100)] public string? TransactionReference { get; set; }
        public int? BankAccountId { get; set; }
        [StringLength(500)] public string? Description { get; set; }
    }
}
