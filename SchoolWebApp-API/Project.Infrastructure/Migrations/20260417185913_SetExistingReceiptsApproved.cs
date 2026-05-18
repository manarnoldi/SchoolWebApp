using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SetExistingReceiptsApproved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Existing Receipts (PaymentType=0) should be marked Approved (ApprovalStatus=2)
            // since they were created before the approval workflow existed.
            migrationBuilder.Sql("UPDATE Payments SET ApprovalStatus = 2 WHERE PaymentType = 0 AND ApprovalStatus = 0;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Payments SET ApprovalStatus = 0 WHERE PaymentType = 0 AND ApprovalStatus = 2;");
        }
    }
}
