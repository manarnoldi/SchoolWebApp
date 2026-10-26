using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Project.Infrastructure.Data;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    // Teaching Documents > Curriculum: the subject's full curriculum design,
    // for reading. Given to every role that already has Schemes of Work.
    // Data only - no model change.
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20261026130000_CurriculumMenu")]
    public partial class CurriculumMenu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO MenuPermissions (RoleId, MenuPath, MenuName, Created, CreatedBy, Modified, ModifiedBy)
                SELECT p.RoleId, '/teaching/curriculum', 'Teaching Documents > Curriculum', NOW(), 'admin', NOW(), 'admin'
                FROM MenuPermissions p
                WHERE p.MenuPath = '/teaching/schemes'
                  AND NOT EXISTS (SELECT 1 FROM MenuPermissions x WHERE x.RoleId = p.RoleId AND x.MenuPath = '/teaching/curriculum');");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM MenuPermissions WHERE MenuPath = '/teaching/curriculum';");
        }
    }
}
