namespace SchoolWebApp.Core.Entities.Payroll
{
    /// <summary>
    /// How payroll works out the value of an earning or deduction that is applied
    /// from its type rather than typed in per employee.
    /// </summary>
    public enum PayrollCalculationMethod
    {
        /// <summary>The type's DefaultValue is used as-is.</summary>
        FixedAmount = 0,

        /// <summary>
        /// DefaultValue is a percentage. Deductions are a percentage of gross pay;
        /// earnings are a percentage of basic salary, because an earning taken as a
        /// percentage of gross would be defined in terms of itself.
        /// </summary>
        Percentage = 1
    }
}
