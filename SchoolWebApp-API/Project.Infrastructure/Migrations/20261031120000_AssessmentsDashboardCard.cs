using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Project.Infrastructure.Data;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    // A dashboard card for CBE assessments (Security > Menu Permissions > Dashboard),
    // given to the roles that oversee assessments - those holding the formative
    // assessment report or the assessments admin page.
    //
    // Hand-written rather than scaffolded - see the note on UniquePayrollTypeCodes.
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20261031120000_AssessmentsDashboardCard")]
    public partial class AssessmentsDashboardCard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO MenuPermissions (RoleId, MenuPath, MenuName, Created, CreatedBy, Modified, ModifiedBy)
                SELECT DISTINCT m.RoleId, '/dashboard/cards/assessments', 'Dashboard > CBE assessments', NOW(), 'admin', NOW(), 'admin'
                FROM MenuPermissions m
                WHERE m.MenuPath IN ('/reports/academics/assessment-report', '/cbe/assessments/admin')
                  AND NOT EXISTS (SELECT 1 FROM MenuPermissions x WHERE x.RoleId = m.RoleId AND x.MenuPath = '/dashboard/cards/assessments');");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM MenuPermissions WHERE MenuPath = '/dashboard/cards/assessments';");
        }
    }
}
