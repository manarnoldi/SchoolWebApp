using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Project.Infrastructure.Data;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    // Payroll reference data (EarningTypes / DeductionTypes) is seeded from two
    // places - the SeedPayrollAndAutoPostAccounts migration and the manually run
    // Data/payroll-seed.sql - and both use INSERT IGNORE. IGNORE only skips rows
    // that violate a unique constraint, and Code had no unique key, so running
    // both seeds inserted the full set twice: 10 earning types became 20 and 12
    // deduction types became 24, every code duplicated.
    //
    // This deduplicates first (lowest Id per Code wins, and any references are
    // repointed to it) and then adds the unique index that makes the existing
    // INSERT IGNORE seeds genuinely idempotent from here on.
    //
    // Hand-written rather than scaffolded: `dotnet ef migrations add` on this
    // project also emits unrelated drift - it re-stamps every seeded Created /
    // Modified timestamp and the admin password hash (the seed data uses
    // non-deterministic values), and drops the SubjectAnalysisNotes unique index
    // that exists in the database but not in the model configuration.
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260909150000_UniquePayrollTypeCodes")]
    public partial class UniquePayrollTypeCodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Repoint anything referencing a duplicate onto the surviving row, so
            // the deletes below cannot fail on a foreign key or orphan real data.
            migrationBuilder.Sql(@"
                UPDATE EmployeeSalaryItems i
                    JOIN EarningTypes dup ON dup.Id = i.EarningTypeId
                    JOIN (SELECT Code, MIN(Id) AS KeepId FROM EarningTypes GROUP BY Code) k
                      ON k.Code = dup.Code
                SET i.EarningTypeId = k.KeepId
                WHERE i.EarningTypeId <> k.KeepId;");

            migrationBuilder.Sql(@"
                UPDATE EmployeeSalaryItems i
                    JOIN DeductionTypes dup ON dup.Id = i.DeductionTypeId
                    JOIN (SELECT Code, MIN(Id) AS KeepId FROM DeductionTypes GROUP BY Code) k
                      ON k.Code = dup.Code
                SET i.DeductionTypeId = k.KeepId
                WHERE i.DeductionTypeId <> k.KeepId;");

            migrationBuilder.Sql(@"
                UPDATE PayslipEarnings p
                    JOIN EarningTypes dup ON dup.Id = p.EarningTypeId
                    JOIN (SELECT Code, MIN(Id) AS KeepId FROM EarningTypes GROUP BY Code) k
                      ON k.Code = dup.Code
                SET p.EarningTypeId = k.KeepId
                WHERE p.EarningTypeId <> k.KeepId;");

            migrationBuilder.Sql(@"
                UPDATE PayslipDeductions p
                    JOIN DeductionTypes dup ON dup.Id = p.DeductionTypeId
                    JOIN (SELECT Code, MIN(Id) AS KeepId FROM DeductionTypes GROUP BY Code) k
                      ON k.Code = dup.Code
                SET p.DeductionTypeId = k.KeepId
                WHERE p.DeductionTypeId <> k.KeepId;");

            // Now drop the duplicates, keeping the lowest Id for each code.
            migrationBuilder.Sql(@"
                DELETE e FROM EarningTypes e
                    JOIN (SELECT Code, MIN(Id) AS KeepId FROM EarningTypes GROUP BY Code) k
                      ON k.Code = e.Code
                WHERE e.Id > k.KeepId;");

            migrationBuilder.Sql(@"
                DELETE d FROM DeductionTypes d
                    JOIN (SELECT Code, MIN(Id) AS KeepId FROM DeductionTypes GROUP BY Code) k
                      ON k.Code = d.Code
                WHERE d.Id > k.KeepId;");

            migrationBuilder.CreateIndex(
                name: "IX_EarningTypes_Code",
                table: "EarningTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeductionTypes_Code",
                table: "DeductionTypes",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Only the index is reversible - the deleted duplicates are not restored.
            migrationBuilder.DropIndex(
                name: "IX_EarningTypes_Code",
                table: "EarningTypes");

            migrationBuilder.DropIndex(
                name: "IX_DeductionTypes_Code",
                table: "DeductionTypes");
        }
    }
}
