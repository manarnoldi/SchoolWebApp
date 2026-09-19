using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Project.Infrastructure.Data;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    // Not everyone on the staff list is paid through the payroll - suppliers,
    // contractors and others invoiced separately still need a staff record for
    // contact and reporting purposes. This flag keeps them on the staff list but
    // out of payroll: payroll processing skips them and they are not offered a
    // salary structure.
    //
    // Defaults to false so every existing staff member stays on the payroll,
    // which is the behaviour before this change.
    //
    // Hand-written rather than scaffolded - see the note on UniquePayrollTypeCodes.
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260912140000_AddExcludeFromPayrollToStaffDetails")]
    public partial class AddExcludeFromPayrollToStaffDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // StaffDetails is a TPH discriminator on the shared Person table, so
            // the column has to be nullable at the database level for the Student
            // and Parent rows that never carry it. EF maps it as a non-nullable
            // bool on StaffDetails, hence the backfill below.
            migrationBuilder.AddColumn<bool>(
                name: "ExcludeFromPayroll",
                table: "Person",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.Sql(
                "UPDATE Person SET ExcludeFromPayroll = 0 WHERE Discriminator = 'StaffDetails';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExcludeFromPayroll",
                table: "Person");
        }
    }
}
