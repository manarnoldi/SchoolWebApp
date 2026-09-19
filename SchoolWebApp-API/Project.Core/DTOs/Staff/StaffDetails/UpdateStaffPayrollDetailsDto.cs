using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.DTOs.Staff.StaffDetails
{
    /// <summary>
    /// The subset of staff details the payroll screens are allowed to change: the
    /// statutory numbers that print on a payslip, plus whether the person is paid
    /// through the payroll at all. Kept separate from StaffDetailDto so payroll
    /// can correct these without submitting - and therefore overwriting - the rest
    /// of the staff record.
    /// </summary>
    public class UpdateStaffPayrollDetailsDto
    {
        [StringLength(255)] public string? KraPinNo { get; set; }
        [StringLength(255)] public string? IdNumber { get; set; }
        [StringLength(255)] public string? NssfNo { get; set; }

        /// <summary>SHA number - still stored in the legacy NhifNo column.</summary>
        [StringLength(255)] public string? NhifNo { get; set; }

        /// <summary>
        /// True for people held on the staff list but not paid through payroll,
        /// such as suppliers and contractors.
        /// </summary>
        public bool ExcludeFromPayroll { get; set; }
    }
}
