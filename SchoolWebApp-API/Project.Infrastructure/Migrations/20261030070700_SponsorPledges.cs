using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <summary>
    /// Sponsorships become pledges on the student's invoice instead of discounts off it: each fee line records the
    /// part a sponsor has promised (SponsoredAmount) and what the sponsor has paid of it (SponsorPaidAmount), and
    /// a student receipt can come from a sponsor payment (SponsorPaymentId). Only the columns - the seed rows the
    /// scaffold re-stamped (and the admin password hash with them) are left alone.
    /// </summary>
    public partial class SponsorPledges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "SponsoredAmount", table: "StudentInvoiceItems", type: "decimal(65,30)", nullable: false, defaultValue: 0m);
            migrationBuilder.AddColumn<decimal>(
                name: "SponsorPaidAmount", table: "StudentInvoiceItems", type: "decimal(65,30)", nullable: false, defaultValue: 0m);
            migrationBuilder.AddColumn<int>(
                name: "SponsorPaymentId", table: "Payments", type: "int", nullable: true);
            migrationBuilder.CreateIndex(
                name: "IX_Payments_SponsorPaymentId", table: "Payments", column: "SponsorPaymentId");
            migrationBuilder.AddForeignKey(
                name: "FK_Payments_SponsorPayments_SponsorPaymentId", table: "Payments", column: "SponsorPaymentId",
                principalTable: "SponsorPayments", principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Payments_SponsorPayments_SponsorPaymentId", table: "Payments");
            migrationBuilder.DropIndex(name: "IX_Payments_SponsorPaymentId", table: "Payments");
            migrationBuilder.DropColumn(name: "SponsorPaymentId", table: "Payments");
            migrationBuilder.DropColumn(name: "SponsorPaidAmount", table: "StudentInvoiceItems");
            migrationBuilder.DropColumn(name: "SponsoredAmount", table: "StudentInvoiceItems");
        }
    }
}
