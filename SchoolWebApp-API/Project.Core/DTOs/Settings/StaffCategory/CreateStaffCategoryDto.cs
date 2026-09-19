namespace SchoolWebApp.Core.DTOs.Settings.StaffCategory
{
    public class CreateStaffCategoryDto : BaseSettinsDto
    {
        public required string Code { get; set; }
        public bool ForTeaching { get; set; }

        /// <summary>
        /// Expense account this category's salaries are charged to in the payroll
        /// journal. Null uses the SalaryExpenseAccountId finance setting.
        /// </summary>
        public int? SalaryExpenseAccountId { get; set; }
    }
}
