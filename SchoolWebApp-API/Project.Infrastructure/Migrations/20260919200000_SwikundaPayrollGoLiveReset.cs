using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Project.Infrastructure.Data;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    // One-off payroll go-live reset for Swikunda Comprehensive School. Everything
    // in payroll up to now was set-up and test data; this clears it so the school
    // starts payroll from a clean slate, and tidies the set-up.
    //
    // Deletes ALL payroll data:
    //   - payroll periods, payslips and their lines
    //   - the journals payroll posted to the ledger, their lines and any approval
    //     requests on them. Matched on the full reference shape
    //     (PAY-JNL-April-2026 / PRL-JNL-April-2026), never by prefix alone: fee
    //     payment journals are PAY-JNL-<receipt> and are kept.
    //   - loans and advances
    //   - every employee salary structure and its lines
    // Tidies the set-up:
    //   - NITA: fixed KES 50 for all staff, so no per-person lines are needed
    //   - removes the unused deduction types NSSFER (employer NSSF is a payslip
    //     figure, not a deduction line) and ADVANCE (advances are handled under
    //     Loans & Advances)
    //   - sets the school's KRA PIN printed in the payslip header
    // Keeps earning and other deduction types, tax and NSSF bands, reliefs, rates,
    // Global Settings, GL accounts and staff.
    //
    // Runs only on a Swikunda database, identified by the school name: migrations
    // run on every database the app starts against, and another school's payroll
    // must never be touched. Runs once, like any migration - it goes live with the
    // deployment and is not repeated.
    //
    // Data only, so the model snapshot is unchanged. Not reversible: the deleted
    // data was test data. Hand-written rather than scaffolded - see the note on
    // UniquePayrollTypeCodes.
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260919200000_SwikundaPayrollGoLiveReset")]
    public partial class SwikundaPayrollGoLiveReset : Migration
    {
        private const string IsSwikunda =
            "EXISTS (SELECT 1 FROM SchoolDetails WHERE UPPER(Name) LIKE '%SWIKUNDA%')";

        // A payroll journal: PAY-JNL- (old) or PRL-JNL- (current) followed by the
        // period - "April-2026", or "4-2026" for a period with no name. Matched on
        // the whole reference, so fee payment journals (PAY-JNL-RCP-<receipt>) never
        // qualify, and journals whose period was already deleted are still caught.
        private const string IsPayrollJournal =
            "j.ReferenceNumber REGEXP '^(PAY|PRL)-JNL-(January|February|March|April|May|June|July|August|September|October|November|December|[0-9]{1,2})-[0-9]{4}$'";

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Payroll journals, children first.
            migrationBuilder.Sql($@"
                DELETE a FROM ApprovalStepActions a
                JOIN ApprovalRequests r ON r.Id = a.ApprovalRequestId
                JOIN JournalEntries j ON j.Id = r.EntityId
                WHERE r.EntityType = 'JournalEntry' AND {IsPayrollJournal} AND {IsSwikunda};");

            migrationBuilder.Sql($@"
                DELETE r FROM ApprovalRequests r
                JOIN JournalEntries j ON j.Id = r.EntityId
                WHERE r.EntityType = 'JournalEntry' AND {IsPayrollJournal} AND {IsSwikunda};");

            migrationBuilder.Sql($@"
                DELETE l FROM JournalLines l
                JOIN JournalEntries j ON j.Id = l.JournalEntryId
                WHERE {IsPayrollJournal} AND {IsSwikunda};");

            migrationBuilder.Sql($@"
                DELETE j FROM JournalEntries j
                WHERE {IsPayrollJournal} AND {IsSwikunda};");

            // 2. Payslips and periods (deduction lines first: they point at loans),
            //    loans, then salary structures.
            foreach (var table in new[]
                     {
                         "PayslipDeductions", "PayslipEarnings", "Payslips", "PayrollPeriods",
                         "LoanAdvances", "EmployeeSalaryItems", "EmployeeSalaries"
                     })
                migrationBuilder.Sql($"DELETE FROM {table} WHERE {IsSwikunda};");

            // 3. NITA: fixed 50 for every staff member on the payroll.
            migrationBuilder.Sql($@"
                UPDATE DeductionTypes
                SET CalculationMethod = 0,
                    DefaultValue      = 50,
                    AppliesToAll      = 1,
                    IsTaxDeductible   = 0,
                    TaxDeductibleCap  = NULL,
                    Description       = 'National Industrial Training Authority levy - fixed KES 50 a month for all staff.',
                    Modified          = NOW(),
                    ModifiedBy        = 'migration'
                WHERE Code = 'NITA' AND {IsSwikunda};");

            // 4. Unused deduction types - nothing refers to them once the data
            //    above is gone.
            migrationBuilder.Sql($@"
                DELETE FROM DeductionTypes
                WHERE Code IN ('NSSFER', 'ADVANCE') AND {IsSwikunda};");

            // 5. The school's KRA PIN for the payslip header.
            migrationBuilder.Sql($@"
                UPDATE GlobalSettings
                SET SettingValue = 'P0513791881', Modified = NOW(), ModifiedBy = 'migration'
                WHERE Module = 'Payroll' AND SettingKey = 'EmployerKraPin' AND {IsSwikunda};");

            migrationBuilder.Sql($@"
                INSERT INTO GlobalSettings (Module, SettingKey, SettingValue, Description, Created, CreatedBy)
                SELECT 'Payroll', 'EmployerKraPin', 'P0513791881',
                       'The school''s KRA PIN as employer, printed in the payslip header.', NOW(), 'migration'
                FROM DUAL
                WHERE {IsSwikunda}
                  AND NOT EXISTS (SELECT 1 FROM GlobalSettings WHERE Module = 'Payroll' AND SettingKey = 'EmployerKraPin');");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Not reversible: the deleted payroll data was test data. Restore from
            // the pre-deployment backup if it is ever needed.
        }
    }
}
