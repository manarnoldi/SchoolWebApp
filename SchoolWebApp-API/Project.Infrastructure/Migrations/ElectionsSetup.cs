using Microsoft.EntityFrameworkCore.Migrations;

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <summary>
    /// Settings and menu permissions the Elections module migrations seed. Every
    /// insert is skipped when the row exists, so re-running is harmless.
    /// </summary>
    internal static class ElectionsSetup
    {
        private static string Q(string s) => s.Replace("'", "''");

        // GlobalSettings Elections/*: the defaults a new election starts with.
        public static void SeedSettingsAndMenus(MigrationBuilder migrationBuilder)
        {
            var settings = new[]
            {
                ("VoterRegisterMode", "Frozen", "Frozen: the voter register is a snapshot taken when nominations close. Live: eligibility follows each student's current class until they vote."),
                ("DashboardRefreshSeconds", "10", "How often, in seconds, the live results screen refreshes.")
            };
            foreach (var (key, value, description) in settings) Setting(migrationBuilder, key, value, description);

            GrantMenu(migrationBuilder, "/elections/manage", "Elections > Manage Elections",
                "Administrator", "HeadTeacher", "DeputyHeadTeacher");
        }

        // How candidates are ordered on each ballot; alphabetical keeps what schools had.
        public static void SeedBallotOrderSetting(MigrationBuilder migrationBuilder) =>
            Setting(migrationBuilder, "BallotOrder", "Alphabetical",
                "Alphabetical: candidates appear in name order on every ballot. Random: each ballot issued shows them in a fresh random order.");

        private static void Setting(MigrationBuilder migrationBuilder, string key, string value, string description) =>
            migrationBuilder.Sql($@"
                INSERT INTO GlobalSettings (Module, SettingKey, SettingValue, Description, Created, CreatedBy, Modified, ModifiedBy)
                SELECT 'Elections', '{key}', '{value}', '{Q(description)}', NOW(), 'admin', NOW(), 'admin' FROM DUAL
                WHERE NOT EXISTS (SELECT 1 FROM GlobalSettings WHERE Module = 'Elections' AND SettingKey = '{key}');");

        // Polling Station is for returning officers (the API limits each to their own
        // stations); Results for whoever runs the elections.
        public static void GrantPollingAndResultsMenus(MigrationBuilder migrationBuilder)
        {
            GrantMenu(migrationBuilder, "/elections/polling", "Elections > Polling Station",
                "Administrator", "HeadTeacher", "DeputyHeadTeacher", "Teacher", "HeadOfDepartment");
            GrantMenu(migrationBuilder, "/elections/results", "Elections > Results",
                "Administrator", "HeadTeacher", "DeputyHeadTeacher");
        }

        // Only added to roles that already have menu permissions - a role with none
        // sees the whole menu, and giving it this alone would hide everything else.
        public static void GrantMenu(MigrationBuilder migrationBuilder, string path, string menuName, params string[] roles)
        {
            foreach (var role in roles)
            {
                migrationBuilder.Sql($@"
                    INSERT INTO MenuPermissions (RoleId, MenuPath, MenuName, Created, CreatedBy, Modified, ModifiedBy)
                    SELECT '{role}', '{path}', '{Q(menuName)}', NOW(), 'admin', NOW(), 'admin' FROM DUAL
                    WHERE NOT EXISTS (SELECT 1 FROM MenuPermissions WHERE RoleId = '{role}' AND MenuPath = '{path}')
                      AND EXISTS (SELECT 1 FROM MenuPermissions WHERE RoleId = '{role}');");
            }
        }
    }
}
