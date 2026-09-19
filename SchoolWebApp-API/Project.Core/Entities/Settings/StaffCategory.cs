using SchoolWebApp.Core.Entities.Finance;
using SchoolWebApp.Core.Entities.Shared;
using SchoolWebApp.Core.Entities.Staff;

namespace SchoolWebApp.Core.Entities.Settings
{
    public class StaffCategory : SettingsBase
    {
        public required string Code { get; set; }
        public bool ForTeaching { get; set; }
        public List<StaffDetails> StaffDetails { get; set; } = new();

        /// <summary>
        /// The expense account the salaries of staff in this category are charged to
        /// when a payroll journal is created, so teaching and non-teaching pay land
        /// in their own accounts. Null falls back to the SalaryExpenseAccountId
        /// finance setting.
        /// </summary>
        public int? SalaryExpenseAccountId { get; set; }
        public Account? SalaryExpenseAccount { get; set; }
    }
}
