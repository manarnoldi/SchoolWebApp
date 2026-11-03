using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Project.Infrastructure.Data;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    // The Viewer role, given on top of another role: the user sees what that role
    // sees and the API refuses every change (ViewerReadOnlyFilter). It needs no
    // menus of its own - the other role brings them.
    //
    // Hand-written rather than scaffolded - see the note on UniquePayrollTypeCodes.
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20261103090000_ViewerRole")]
    public partial class ViewerRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO AspNetRoles (Name, NormalizedName, ConcurrencyStamp, Created, CreatedBy, Modified, ModifiedBy)
                SELECT 'Viewer', 'VIEWER', UUID(), NOW(), 'admin', NOW(), 'admin'
                WHERE NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE NormalizedName = 'VIEWER');");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE ur FROM AspNetUserRoles ur JOIN AspNetRoles r ON r.Id = ur.RoleId WHERE r.NormalizedName = 'VIEWER';
                DELETE FROM AspNetRoles WHERE NormalizedName = 'VIEWER';");
        }
    }
}
