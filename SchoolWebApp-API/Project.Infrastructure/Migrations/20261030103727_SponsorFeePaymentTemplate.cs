using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <summary>
    /// The receipt message for a sponsor's payment: the parents are told the sponsor paid. It starts with the
    /// fee payment message's channel and on/off state, so a school that has that one switched off is not affected.
    /// </summary>
    public partial class SponsorFeePaymentTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO MessageTemplates
                    (Code, Name, Description, IsSystem, Channel, IsActive, SmsBody, EmailSubject, EmailBody, Created, CreatedBy, Modified, ModifiedBy)
                SELECT 'SponsorFeePayment', 'Fee payment by a sponsor', 'Tells the parents a sponsor has paid towards the learner''s fees.', 1,
                    COALESCE((SELECT Channel FROM MessageTemplates WHERE Code = 'FeePayment' LIMIT 1), 1),
                    COALESCE((SELECT IsActive FROM MessageTemplates WHERE Code = 'FeePayment' LIMIT 1), 0),
                    'Dear {ParentName}, {SponsorName} paid KES {AmountPaid} for {StudentName}''s fees, receipt {ReceiptNumber}. Balance: KES {Balance}. {SchoolName}',
                    'Sponsor payment received for {StudentName}',
                    'Dear {ParentName},\n\n{SponsorName} has paid KES {AmountPaid} towards the fees of {StudentName} ({AdmissionNo}), receipt {ReceiptNumber}.\n\nFee balance: KES {Balance}\n\nRegards,\n{SchoolName}',
                    UTC_TIMESTAMP(), 'system', UTC_TIMESTAMP(), 'system'
                FROM DUAL
                WHERE NOT EXISTS (SELECT 1 FROM MessageTemplates WHERE Code = 'SponsorFeePayment');");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM MessageTemplates WHERE Code = 'SponsorFeePayment';");
        }
    }
}
