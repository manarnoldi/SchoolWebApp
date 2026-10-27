using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Project.Infrastructure.Data;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    // The Teaching Documents menu is now called Teaching Management; the stored
    // menu permission names follow.
    //
    // Hand-written rather than scaffolded - see the note on UniquePayrollTypeCodes.
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20261028150000_RenameTeachingManagementMenu")]
    public partial class RenameTeachingManagementMenu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE MenuPermissions
                SET MenuName = REPLACE(MenuName, 'Teaching Documents >', 'Teaching Management >')
                WHERE MenuName LIKE 'Teaching Documents >%';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE MenuPermissions
                SET MenuName = REPLACE(MenuName, 'Teaching Management >', 'Teaching Documents >')
                WHERE MenuName LIKE 'Teaching Management >%';");
        }
    }
}
