using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.Finance;
using SchoolWebApp.Core.Entities.Shared;
using SchoolWebApp.Core.Entities.Students;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Sponsorships
{
    public enum SponsorshipCoverageType
    {
        FixedAmount = 0,          // Cover a flat amount per invoice
        Percentage = 1,           // Cover a % of eligible items
        FullCoverage = 2          // Cover 100% of eligible items
    }

    public enum SponsorshipStatus
    {
        Active = 0,
        Ended = 1,
        Cancelled = 2
    }

    public class Sponsorship : Base
    {
        public int SponsorId { get; set; }
        public Sponsor? Sponsor { get; set; }

        // Target: a specific student OR a whole class. One of these must be set.
        public int? StudentId { get; set; }
        public Student? Student { get; set; }

        public int? SchoolClassId { get; set; }
        public SchoolClass? SchoolClass { get; set; }

        public int AcademicYearId { get; set; }
        public AcademicYear? AcademicYear { get; set; }

        public int? SessionId { get; set; }
        public Session? Session { get; set; }

        public SponsorshipCoverageType CoverageType { get; set; } = SponsorshipCoverageType.Percentage;

        // Used when CoverageType = FixedAmount — flat cap per invoice.
        public decimal FixedAmount { get; set; }

        // Used when CoverageType = Percentage — 0..100 (100 == full coverage).
        public decimal Percentage { get; set; }

        // If any fee category ids are listed here, the sponsorship only covers those categories.
        // Empty / null = cover all fee categories.
        public List<SponsorshipFeeCategory> FeeCategories { get; set; } = new();

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public SponsorshipStatus Status { get; set; } = SponsorshipStatus.Active;
    }

    public class SponsorshipFeeCategory : Base
    {
        public int SponsorshipId { get; set; }
        public Sponsorship? Sponsorship { get; set; }

        public int FeeCategoryId { get; set; }
        public FeeCategory? FeeCategory { get; set; }
    }
}
