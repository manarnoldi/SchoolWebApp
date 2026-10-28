using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Project.Infrastructure.Data;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    // The dashboard's cards are now permissions of their own (Security > Menu
    // Permissions > Dashboard). Each goes to the roles that saw it until now -
    // those holding one of the pages it summarises - so nobody gains or loses a card.
    //
    // Hand-written rather than scaffolded - see the note on UniquePayrollTypeCodes.
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20261028170000_DashboardCardPermissions")]
    public partial class DashboardCardPermissions : Migration
    {
        private static readonly (string Path, string Name, string[] From)[] Cards =
        {
            ("/dashboard/cards/exam-performance", "Dashboard > Class exam performance",
                new[] { "/reports/academics/exam-results", "/reports/academics/class-performance", "/cbe/exams/exam-results-bulk" }),
            ("/dashboard/cards/fees", "Dashboard > Fees (collections, arrears, spending)",
                new[] { "/finance/reports/fees", "/finance/payments" }),
            ("/dashboard/cards/teaching", "Dashboard > Teaching documents", new[] { "/teaching/tracker" }),
            ("/dashboard/cards/payroll", "Dashboard > Payroll", new[] { "/payroll/periods", "/payroll/reports" }),
            ("/dashboard/cards/elections", "Dashboard > Elections", new[] { "/elections/manage" }),
            ("/dashboard/cards/messages", "Dashboard > Messages", new[] { "/communications/queue", "/communications/sms-report" }),
        };

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            foreach (var (path, name, from) in Cards)
            {
                migrationBuilder.Sql($@"
                    INSERT INTO MenuPermissions (RoleId, MenuPath, MenuName, Created, CreatedBy, Modified, ModifiedBy)
                    SELECT DISTINCT m.RoleId, '{path}', '{name}', NOW(), 'admin', NOW(), 'admin'
                    FROM MenuPermissions m
                    WHERE m.MenuPath IN ('{string.Join("', '", from)}')
                      AND NOT EXISTS (SELECT 1 FROM MenuPermissions x WHERE x.RoleId = m.RoleId AND x.MenuPath = '{path}');");
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM MenuPermissions WHERE MenuPath LIKE '/dashboard/cards/%';");
        }
    }
}
