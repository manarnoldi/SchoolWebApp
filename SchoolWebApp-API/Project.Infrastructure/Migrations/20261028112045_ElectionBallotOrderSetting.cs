using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ElectionBallotOrderSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Elections/BallotOrder: alphabetical (as before) unless a school picks random.
            ElectionsSetup.SeedBallotOrderSetting(migrationBuilder);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM GlobalSettings WHERE Module = 'Elections' AND SettingKey = 'BallotOrder';");
        }
    }
}
