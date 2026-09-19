using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Payroll
{
    /// <summary>
    /// One NSSF contribution tier, as it stood from a given date. The NSSF Act's
    /// limits move most Februaries, so the bands are dated rather than held as flat
    /// settings: re-processing an old period then still uses that period's limits
    /// instead of whatever is current.
    ///
    /// Payroll uses the most recent set whose EffectiveDate is on or before the
    /// period being run, and applies every tier in that set.
    /// </summary>
    public class NssfBand : Base
    {
        [Required, StringLength(255)]
        public string Name { get; set; } = string.Empty;

        /// <summary>Tier number, applied in ascending order (I, then II).</summary>
        public int Tier { get; set; }

        /// <summary>Pay below this does not attract this tier.</summary>
        public decimal LowerLimit { get; set; }

        /// <summary>Pay above this is not charged by this tier.</summary>
        public decimal UpperLimit { get; set; }

        /// <summary>Percentage charged on the pay falling inside the tier.</summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// The date this set of tiers came into force. Every tier in a set shares
        /// the same date; that is what groups them into a set.
        /// </summary>
        public DateTime EffectiveDate { get; set; }

        public bool IsActive { get; set; } = true;

        [StringLength(500)]
        public string? Description { get; set; }
    }
}
