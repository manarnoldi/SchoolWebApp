using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Project.Infrastructure.Data;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    // The assessment tracker (Academics > Assessments > Assessment Tracker) goes to
    // the roles that enter or oversee assessments. What each sees is scoped by the
    // page itself: their own subjects, their departments, or the whole school.
    //
    // Hand-written rather than scaffolded - see the note on UniquePayrollTypeCodes.
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20261031150000_AssessmentTrackerMenu")]
    public partial class AssessmentTrackerMenu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO MenuPermissions (RoleId, MenuPath, MenuName, Created, CreatedBy, Modified, ModifiedBy)
                SELECT DISTINCT m.RoleId, '/cbe/assessments/tracker', 'Academics > Assessments > Assessment Tracker', NOW(), 'admin', NOW(), 'admin'
                FROM MenuPermissions m
                WHERE m.MenuPath IN ('/cbe/assessments/assessments', '/cbe/assessments/admin', '/reports/academics/assessment-report')
                  AND NOT EXISTS (SELECT 1 FROM MenuPermissions x WHERE x.RoleId = m.RoleId AND x.MenuPath = '/cbe/assessments/tracker');");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM MenuPermissions WHERE MenuPath = '/cbe/assessments/tracker';");
        }
    }
}
