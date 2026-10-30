using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <summary>
    /// Staff bank details (where net pay is sent), and the payroll approval workflow: a processed payroll is
    /// reviewed and then approved, by two different administrators, before it is locked and posted.
    /// </summary>
    public partial class PayrollApprovalAndBankDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BankAccountNumber",
                table: "Person",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "BankBranch",
                table: "Person",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "BankName",
                table: "Person",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            // Two steps, like the finance workflows: a review and the final approval, both by administrators.
            // Schools can change the roles under Approvals > Workflows.
            migrationBuilder.Sql(@"
                INSERT INTO ApprovalWorkflows (Name, FormKey, Description, IsMakerChecker, IsActive, Created, CreatedBy, Modified, ModifiedBy)
                SELECT 'Payroll Approval', 'Payroll', 'A processed payroll is reviewed, then approved, before it is locked and posted', 1, 1,
                    UTC_TIMESTAMP(), 'system', UTC_TIMESTAMP(), 'system'
                FROM DUAL
                WHERE NOT EXISTS (SELECT 1 FROM ApprovalWorkflows WHERE FormKey = 'Payroll')
                  AND EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'Administrator');

                INSERT INTO ApprovalWorkflowSteps (ApprovalWorkflowId, `Rank`, Name, RoleId, IsFinal, NotifyNextApprover, NotifyPreviousApprover, NotifyApplicant,
                    Created, CreatedBy, Modified, ModifiedBy)
                SELECT w.Id, s.r, s.n, (SELECT Id FROM AspNetRoles WHERE Name = 'Administrator' LIMIT 1), s.f, s.nn, s.np, 1,
                    UTC_TIMESTAMP(), 'system', UTC_TIMESTAMP(), 'system'
                FROM ApprovalWorkflows w
                JOIN (SELECT 1 r, 'Payroll Review' n, 0 f, 1 nn, 0 np UNION ALL SELECT 2, 'Payroll Approval', 1, 0, 1) s
                WHERE w.FormKey = 'Payroll' AND NOT EXISTS (SELECT 1 FROM ApprovalWorkflowSteps x WHERE x.ApprovalWorkflowId = w.Id);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE s FROM ApprovalWorkflowSteps s JOIN ApprovalWorkflows w ON w.Id = s.ApprovalWorkflowId WHERE w.FormKey = 'Payroll';
                DELETE FROM ApprovalWorkflows WHERE FormKey = 'Payroll';");

            migrationBuilder.DropColumn(
                name: "BankAccountNumber",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "BankBranch",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "BankName",
                table: "Person");
        }
    }
}
