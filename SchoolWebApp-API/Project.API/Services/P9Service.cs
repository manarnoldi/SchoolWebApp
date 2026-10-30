using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.Entities.Payroll;

namespace SchoolWebApp.API.Services
{
    /// <summary>
    /// The P9A tax deduction card: an employee's pay, deductions and PAYE month by month for a year, in KRA's
    /// columns, from approved payroll only. Used by Payroll Reports (any year, everyone) and by the staff
    /// member's own profile (a closed year, themselves).
    /// </summary>
    public class P9Service
    {
        private readonly ApplicationDbContext _db;

        public P9Service(ApplicationDbContext db) => _db = db;

        public class P9Row
        {
            public int Month { get; set; }
            public decimal Basic { get; set; }
            // Taxable earnings other than basic pay: allowances and benefits.
            public decimal Benefits { get; set; }
            public decimal Quarters { get; set; }
            public decimal Gross { get; set; }
            // E: retirement contributions - the limit as a share of basic, what was paid, the fixed cap.
            public decimal E1 { get; set; }
            public decimal E2 { get; set; }
            public decimal E3 { get; set; }
            public decimal OwnerOccupiedInterest { get; set; }
            public decimal Retirement { get; set; }
            public decimal Ahl { get; set; }
            public decimal Shif { get; set; }
            public decimal OtherDeductible { get; set; }
            public decimal Chargeable { get; set; }
            public decimal TaxCharged { get; set; }
            public decimal PersonalRelief { get; set; }
            public decimal InsuranceRelief { get; set; }
            public decimal Paye { get; set; }
        }

        public class P9Employee
        {
            public int StaffId { get; set; }
            public string? Name { get; set; }
            public string? StaffNo { get; set; }
            public string? KraPin { get; set; }
            public string? IdNumber { get; set; }
            public List<P9Row> Rows { get; set; } = new();
        }

        /// <summary>The cards for a year: everyone paid that year, or one employee.</summary>
        public async Task<List<P9Employee>> BuildAsync(int year, int? staffId)
        {
            var settings = await _db.PayrollSettings.AsNoTracking().Where(s => s.IsActive).ToListAsync();
            decimal Setting(string key) => settings.FirstOrDefault(s => s.Key == key)?.Value ?? 0m;
            var percent = Setting("RetirementReliefPercent");
            var cap = Setting("RetirementReliefCap");
            var retirementTypeIds = (await _db.DeductionTypes.AsNoTracking()
                .Where(d => d.IsRetirementContribution).Select(d => d.Id).ToListAsync()).ToHashSet();

            var slips = await _db.Payslips.AsNoTracking()
                .Include(p => p.PayrollPeriod).Include(p => p.StaffDetails).Include(p => p.Deductions)
                .Where(p => p.PayrollPeriod!.Year == year
                            && (p.PayrollPeriod.Status == PayrollPeriodStatus.Approved || p.PayrollPeriod.Status == PayrollPeriodStatus.Posted)
                            && (staffId == null || p.StaffDetailsId == staffId))
                .ToListAsync();

            return slips.GroupBy(p => p.StaffDetailsId).Select(g =>
            {
                var staff = g.First().StaffDetails!;
                return new P9Employee
                {
                    StaffId = staff.Id,
                    Name = staff.FullName,
                    StaffNo = staff.UPI,
                    KraPin = staff.KraPinNo,
                    IdNumber = staff.IdNumber,
                    Rows = g.OrderBy(p => p.PayrollPeriod!.Month).Select(p => new P9Row
                    {
                        Month = p.PayrollPeriod!.Month,
                        Basic = p.BasicSalary,
                        Benefits = p.TaxablePay - p.BasicSalary,
                        Quarters = 0m,
                        Gross = p.TaxablePay,
                        E1 = Math.Round(p.BasicSalary * percent / 100m, 2),
                        E2 = p.NssfEmployee + p.Deductions.Where(d => retirementTypeIds.Contains(d.DeductionTypeId)).Sum(d => d.Amount),
                        E3 = cap,
                        OwnerOccupiedInterest = 0m,
                        Retirement = p.RetirementRelief,
                        Ahl = p.Ahl,
                        Shif = p.Shif,
                        OtherDeductible = p.OtherTaxDeductible,
                        Chargeable = p.TaxableIncome,
                        TaxCharged = p.GrossTax,
                        PersonalRelief = p.PersonalRelief,
                        InsuranceRelief = p.InsuranceRelief,
                        Paye = p.Paye
                    }).ToList()
                };
            }).OrderBy(e => e.Name).ToList();
        }

        /// <summary>
        /// The years whose P9 a staff member may take from their profile: years that have ended, with every
        /// payroll run in them signed off (none still a draft or awaiting approval), in which they were paid.
        /// Newest first.
        /// </summary>
        public async Task<List<int>> ClosedYearsAsync(int staffId)
        {
            var thisYear = DateTime.Today.Year;
            var periods = await _db.PayrollPeriods.AsNoTracking()
                .Where(p => p.Year < thisYear)
                .Select(p => new { p.Year, p.Status, Paid = p.Payslips.Any(s => s.StaffDetailsId == staffId) })
                .ToListAsync();
            return periods.GroupBy(p => p.Year)
                .Where(g => g.All(p => p.Status == PayrollPeriodStatus.Approved || p.Status == PayrollPeriodStatus.Posted)
                            && g.Any(p => p.Paid))
                .Select(g => g.Key).OrderByDescending(y => y).ToList();
        }
    }
}
