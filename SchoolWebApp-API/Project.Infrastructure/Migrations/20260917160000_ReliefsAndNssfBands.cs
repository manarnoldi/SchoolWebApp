using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Project.Infrastructure.Data;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    // Two more ideas from the legacy SchoolSoft payroll:
    //
    // 1. PayrollReliefs (legacy tblPayrollReliefs) - a relief is a row naming what
    //    it relieves, rather than a rule in code. Personal relief becomes a fixed
    //    row; insurance relief becomes a percentage row pointing at the Insurance
    //    Premium deduction. A new relief is then data entry.
    //
    //    This retires the PersonalRelief, InsuranceReliefRate and InsuranceReliefCap
    //    settings, and the QualifiesForInsuranceRelief flag on the deduction type -
    //    the relief's own link to a deduction says which one it applies to, so the
    //    flag would have been a second place to hold the same fact.
    //
    // 2. NssfBands (legacy tblPayrollNSSF) - the NSSF tiers as dated rows instead of
    //    four flat settings. The limits move most Februaries; dating them means
    //    re-processing an old period still uses that period's limits rather than
    //    whatever is current.
    //
    // The seeded rows reproduce the current figures exactly, so the next payroll run
    // produces the same numbers as the last one.
    //
    // Hand-written rather than scaffolded - see the note on UniquePayrollTypeCodes.
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260917160000_ReliefsAndNssfBands")]
    public partial class ReliefsAndNssfBands : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NssfBands",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    Tier = table.Column<int>(type: "int", nullable: false),
                    LowerLimit = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    UpperLimit = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table => table.PrimaryKey("PK_NssfBands", x => x.Id))
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PayrollReliefs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    Code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Basis = table.Column<int>(type: "int", nullable: false),
                    DeductionTypeId = table.Column<int>(type: "int", nullable: true),
                    Value = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    MonthlyCap = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    AppliesToAll = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollReliefs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayrollReliefs_DeductionTypes_DeductionTypeId",
                        column: x => x.DeductionTypeId,
                        principalTable: "DeductionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollReliefs_Code",
                table: "PayrollReliefs",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PayrollReliefs_DeductionTypeId",
                table: "PayrollReliefs",
                column: "DeductionTypeId");

            // Seed the NSSF tiers from the settings that held them. Dated well in the
            // past so every existing period resolves to a set; when the limits next
            // change, add a new pair dated from the day it takes effect.
            migrationBuilder.Sql(@"
                INSERT INTO NssfBands (Name, Tier, LowerLimit, UpperLimit, Rate, EffectiveDate, IsActive, Description, Created, CreatedBy)
                SELECT 'NSSF Tier I', 1, 0,
                       COALESCE((SELECT Value FROM PayrollSettings WHERE `Key` = 'NssfTier1Ceiling' LIMIT 1), 9000),
                       COALESCE((SELECT Value FROM PayrollSettings WHERE `Key` = 'NssfTier1Rate' LIMIT 1), 6),
                       '2000-01-01', 1,
                       'Migrated from the NssfTier1Ceiling / NssfTier1Rate settings.', NOW(), 'migration'
                FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM NssfBands WHERE Tier = 1);");

            migrationBuilder.Sql(@"
                INSERT INTO NssfBands (Name, Tier, LowerLimit, UpperLimit, Rate, EffectiveDate, IsActive, Description, Created, CreatedBy)
                SELECT 'NSSF Tier II', 2,
                       COALESCE((SELECT Value FROM PayrollSettings WHERE `Key` = 'NssfTier1Ceiling' LIMIT 1), 9000),
                       COALESCE((SELECT Value FROM PayrollSettings WHERE `Key` = 'NssfTier2Ceiling' LIMIT 1), 108000),
                       COALESCE((SELECT Value FROM PayrollSettings WHERE `Key` = 'NssfTier2Rate' LIMIT 1), 6),
                       '2000-01-01', 1,
                       'Migrated from the NssfTier2Ceiling / NssfTier2Rate settings.', NOW(), 'migration'
                FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM NssfBands WHERE Tier = 2);");

            // Personal relief: a flat figure given to everyone.
            migrationBuilder.Sql(@"
                INSERT INTO PayrollReliefs (Name, Code, Basis, DeductionTypeId, Value, MonthlyCap, AppliesToAll, IsActive, Description, Created, CreatedBy)
                SELECT 'Personal Relief', 'PERSONAL', 0, NULL,
                       COALESCE((SELECT Value FROM PayrollSettings WHERE `Key` = 'PersonalRelief' LIMIT 1), 2400),
                       NULL, 1, 1, 'Migrated from the PersonalRelief setting.', NOW(), 'migration'
                FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM PayrollReliefs WHERE Code = 'PERSONAL');");

            // Insurance relief: a percentage of what was paid towards the Insurance
            // Premium deduction, capped monthly.
            migrationBuilder.Sql(@"
                INSERT INTO PayrollReliefs (Name, Code, Basis, DeductionTypeId, Value, MonthlyCap, AppliesToAll, IsActive, Description, Created, CreatedBy)
                SELECT 'Insurance Relief', 'INSURANCE', 1,
                       (SELECT Id FROM DeductionTypes WHERE Code = 'INSURANCE' LIMIT 1),
                       COALESCE((SELECT Value FROM PayrollSettings WHERE `Key` = 'InsuranceReliefRate' LIMIT 1), 15),
                       COALESCE((SELECT Value FROM PayrollSettings WHERE `Key` = 'InsuranceReliefCap' LIMIT 1), 5000),
                       0, 1, 'Migrated from the InsuranceReliefRate / InsuranceReliefCap settings.', NOW(), 'migration'
                FROM DUAL
                WHERE EXISTS (SELECT 1 FROM DeductionTypes WHERE Code = 'INSURANCE')
                  AND NOT EXISTS (SELECT 1 FROM PayrollReliefs WHERE Code = 'INSURANCE');");

            // The settings and the flag these replace, so there is one place to look.
            migrationBuilder.Sql(@"
                DELETE FROM PayrollSettings WHERE `Key` IN
                ('NssfTier1Ceiling','NssfTier1Rate','NssfTier2Ceiling','NssfTier2Rate',
                 'PersonalRelief','InsuranceReliefRate','InsuranceReliefCap');");

            migrationBuilder.DropColumn(
                name: "QualifiesForInsuranceRelief",
                table: "DeductionTypes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "QualifiesForInsuranceRelief",
                table: "DeductionTypes",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql(@"
                UPDATE DeductionTypes d
                    JOIN PayrollReliefs r ON r.DeductionTypeId = d.Id AND r.Basis = 1
                SET d.QualifiesForInsuranceRelief = 1;");

            migrationBuilder.Sql(@"
                INSERT INTO PayrollSettings (`Key`, Name, Value, Category, Description, EffectiveDate, IsActive, Created, CreatedBy)
                SELECT 'NssfTier1Ceiling', 'NSSF Tier I Ceiling',
                       COALESCE((SELECT UpperLimit FROM NssfBands WHERE Tier = 1 ORDER BY EffectiveDate DESC LIMIT 1), 9000),
                       'NSSF', NULL, '2026-01-01', 1, NOW(), 'migration'
                FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM PayrollSettings WHERE `Key` = 'NssfTier1Ceiling');");

            migrationBuilder.Sql(@"
                INSERT INTO PayrollSettings (`Key`, Name, Value, Category, Description, EffectiveDate, IsActive, Created, CreatedBy)
                SELECT 'NssfTier1Rate', 'NSSF Tier I Rate (%)',
                       COALESCE((SELECT Rate FROM NssfBands WHERE Tier = 1 ORDER BY EffectiveDate DESC LIMIT 1), 6),
                       'NSSF', NULL, '2026-01-01', 1, NOW(), 'migration'
                FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM PayrollSettings WHERE `Key` = 'NssfTier1Rate');");

            migrationBuilder.Sql(@"
                INSERT INTO PayrollSettings (`Key`, Name, Value, Category, Description, EffectiveDate, IsActive, Created, CreatedBy)
                SELECT 'NssfTier2Ceiling', 'NSSF Tier II Ceiling',
                       COALESCE((SELECT UpperLimit FROM NssfBands WHERE Tier = 2 ORDER BY EffectiveDate DESC LIMIT 1), 108000),
                       'NSSF', NULL, '2026-01-01', 1, NOW(), 'migration'
                FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM PayrollSettings WHERE `Key` = 'NssfTier2Ceiling');");

            migrationBuilder.Sql(@"
                INSERT INTO PayrollSettings (`Key`, Name, Value, Category, Description, EffectiveDate, IsActive, Created, CreatedBy)
                SELECT 'NssfTier2Rate', 'NSSF Tier II Rate (%)',
                       COALESCE((SELECT Rate FROM NssfBands WHERE Tier = 2 ORDER BY EffectiveDate DESC LIMIT 1), 6),
                       'NSSF', NULL, '2026-01-01', 1, NOW(), 'migration'
                FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM PayrollSettings WHERE `Key` = 'NssfTier2Rate');");

            migrationBuilder.Sql(@"
                INSERT INTO PayrollSettings (`Key`, Name, Value, Category, Description, EffectiveDate, IsActive, Created, CreatedBy)
                SELECT 'PersonalRelief', 'Personal Relief (Monthly)',
                       COALESCE((SELECT Value FROM PayrollReliefs WHERE Code = 'PERSONAL' LIMIT 1), 2400),
                       'Relief', NULL, '2026-01-01', 1, NOW(), 'migration'
                FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM PayrollSettings WHERE `Key` = 'PersonalRelief');");

            migrationBuilder.Sql(@"
                INSERT INTO PayrollSettings (`Key`, Name, Value, Category, Description, EffectiveDate, IsActive, Created, CreatedBy)
                SELECT 'InsuranceReliefRate', 'Insurance Relief Rate (%)',
                       COALESCE((SELECT Value FROM PayrollReliefs WHERE Code = 'INSURANCE' LIMIT 1), 15),
                       'Relief', NULL, '2026-01-01', 1, NOW(), 'migration'
                FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM PayrollSettings WHERE `Key` = 'InsuranceReliefRate');");

            migrationBuilder.Sql(@"
                INSERT INTO PayrollSettings (`Key`, Name, Value, Category, Description, EffectiveDate, IsActive, Created, CreatedBy)
                SELECT 'InsuranceReliefCap', 'Insurance Relief Cap (Monthly)',
                       COALESCE((SELECT MonthlyCap FROM PayrollReliefs WHERE Code = 'INSURANCE' LIMIT 1), 5000),
                       'Relief', NULL, '2026-01-01', 1, NOW(), 'migration'
                FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM PayrollSettings WHERE `Key` = 'InsuranceReliefCap');");

            migrationBuilder.DropTable(name: "PayrollReliefs");
            migrationBuilder.DropTable(name: "NssfBands");
        }
    }
}
