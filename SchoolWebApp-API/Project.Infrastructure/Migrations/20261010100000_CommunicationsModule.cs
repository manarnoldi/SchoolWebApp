using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Project.Infrastructure.Data;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    // Communications module: SMS (TextSMS gateway) and email (SMTP) to parents and
    // staff, from templates with placeholders, through a queue the dispatch worker
    // drains in the background.
    //
    // - CommunicationSettings: one row - gateway / SMTP credentials and test mode.
    //   Seeded with test mode ON and both channels off, so nothing leaves a school
    //   until a SuperAdministrator configures and switches it on.
    // - MessageTemplates: the standard message types (results, invoice, balance
    //   reminder, payment) seeded as system templates; the school picks each one's
    //   channel and edits the wording.
    // - MessageBatches / OutboundMessages: each send and its per-recipient messages.
    // - GlobalSettings Communications/ParentContactSource: where parent contacts
    //   come from - the parent record, the student record, or parent first.
    //
    // Hand-written rather than scaffolded - see the note on UniquePayrollTypeCodes.
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20261010100000_CommunicationsModule")]
    public partial class CommunicationsModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommunicationSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SmsEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SmsApiUrl = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SmsApiKey = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SmsPartnerId = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SmsSenderId = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SmsUnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EmailEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SmtpHost = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SmtpPort = table.Column<int>(type: "int", nullable: false),
                    SmtpUseSsl = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SmtpUsername = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SmtpPassword = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FromEmail = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FromName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReplyToEmail = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TestMode = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TestPhoneNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TestEmail = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunicationSettings", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MessageTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsSystem = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Channel = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SmsBody = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmailSubject = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmailBody = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageTemplates", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MessageBatches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MessageTypeCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MessageTemplateId = table.Column<int>(type: "int", nullable: true),
                    Channel = table.Column<int>(type: "int", nullable: false),
                    RecipientSummary = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TotalMessages = table.Column<int>(type: "int", nullable: false),
                    IsTest = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageBatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageBatches_MessageTemplates_MessageTemplateId",
                        column: x => x.MessageTemplateId,
                        principalTable: "MessageTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "OutboundMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MessageBatchId = table.Column<int>(type: "int", nullable: false),
                    MessageTypeCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Channel = table.Column<int>(type: "int", nullable: false),
                    RecipientType = table.Column<int>(type: "int", nullable: false),
                    PersonId = table.Column<int>(type: "int", nullable: true),
                    StudentId = table.Column<int>(type: "int", nullable: true),
                    RecipientName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Destination = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OriginalDestination = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Subject = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Body = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Attempts = table.Column<int>(type: "int", nullable: false),
                    NextAttemptAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    SentAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeliveredAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ProviderMessageId = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProviderStatus = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ErrorMessage = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SmsParts = table.Column<int>(type: "int", nullable: false),
                    IsTest = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeliveryChecks = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboundMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutboundMessages_MessageBatches_MessageBatchId",
                        column: x => x.MessageBatchId,
                        principalTable: "MessageBatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_MessageBatches_MessageTemplateId",
                table: "MessageBatches",
                column: "MessageTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageTemplates_Code",
                table: "MessageTemplates",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OutboundMessages_Channel_Created",
                table: "OutboundMessages",
                columns: new[] { "Channel", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_OutboundMessages_MessageBatchId",
                table: "OutboundMessages",
                column: "MessageBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboundMessages_ProviderMessageId",
                table: "OutboundMessages",
                column: "ProviderMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboundMessages_Status_NextAttemptAt",
                table: "OutboundMessages",
                columns: new[] { "Status", "NextAttemptAt" });

            migrationBuilder.Sql(@"
                INSERT INTO CommunicationSettings
                    (SmsEnabled, SmsApiUrl, SmsUnitPrice, EmailEnabled, SmtpPort, SmtpUseSsl, TestMode, Created, CreatedBy, Modified, ModifiedBy)
                VALUES
                    (0, 'https://sms.textsms.co.ke/api/services/', 0, 0, 587, 1, 1, UTC_TIMESTAMP(), 'system', UTC_TIMESTAMP(), 'system');");

            migrationBuilder.Sql(@"
                INSERT INTO MessageTemplates
                    (Code, Name, Description, IsSystem, Channel, IsActive, SmsBody, EmailSubject, EmailBody, Created, CreatedBy, Modified, ModifiedBy)
                VALUES
                ('ExamResults', 'Exam results', 'Sent to parents when exam results are released.', 1, 1, 1,
                 'Dear {ParentName}, {StudentName} ({ClassName}) {ExamName} results: {SubjectScores}. Mean: {MeanScore} {MeanGrade}. Position {Position} of {ClassSize}. {SchoolName}',
                 '{ExamName} results for {StudentName}',
                 'Dear {ParentName},\n\nPlease find below the {ExamName} results for {StudentName} ({AdmissionNo}), {ClassName}, {TermName}.\n\n{SubjectScores}\n\nTotal: {TotalMarks}\nMean: {MeanScore} ({MeanGrade})\nPosition: {Position} of {ClassSize}\n\nRegards,\n{SchoolName}\n{SchoolPhone}',
                 UTC_TIMESTAMP(), 'system', UTC_TIMESTAMP(), 'system'),
                ('FeeInvoice', 'New fee invoice', 'Sent to parents when a fee invoice is raised.', 1, 1, 1,
                 'Dear {ParentName}, invoice {InvoiceNumber} of KES {InvoiceAmount} has been raised for {StudentName} for {TermName}, due {DueDate}. Fee balance: KES {Balance}. {SchoolName}',
                 'Fee invoice {InvoiceNumber} for {StudentName}',
                 'Dear {ParentName},\n\nInvoice {InvoiceNumber} of KES {InvoiceAmount} has been raised for {StudentName} ({AdmissionNo}), {ClassName}, for {TermName}.\n\n{InvoiceItems}\n\nDue date: {DueDate}\nTotal fee balance: KES {Balance}\n\nRegards,\n{SchoolName}\n{SchoolPhone}',
                 UTC_TIMESTAMP(), 'system', UTC_TIMESTAMP(), 'system'),
                ('FeeBalance', 'Fee balance reminder', 'Reminds parents of an outstanding fee balance.', 1, 1, 1,
                 'Dear {ParentName}, this is a reminder that the fee balance for {StudentName} ({AdmissionNo}) is KES {Balance}. Kindly clear it at your earliest convenience. {SchoolName}',
                 'Fee balance reminder for {StudentName}',
                 'Dear {ParentName},\n\nThis is a reminder that the outstanding fee balance for {StudentName} ({AdmissionNo}), {ClassName}, is KES {Balance}.\n\nKindly clear it at your earliest convenience.\n\nRegards,\n{SchoolName}\n{SchoolPhone}',
                 UTC_TIMESTAMP(), 'system', UTC_TIMESTAMP(), 'system'),
                ('FeePayment', 'Fee payment received', 'Acknowledges a fee payment.', 1, 1, 0,
                 'Dear {ParentName}, we have received KES {AmountPaid} for {StudentName}, receipt {ReceiptNumber}. Fee balance: KES {Balance}. Thank you. {SchoolName}',
                 'Payment received for {StudentName}',
                 'Dear {ParentName},\n\nWe have received KES {AmountPaid} for {StudentName} ({AdmissionNo}), receipt {ReceiptNumber}.\n\nFee balance: KES {Balance}\n\nThank you,\n{SchoolName}',
                 UTC_TIMESTAMP(), 'system', UTC_TIMESTAMP(), 'system');");

            migrationBuilder.Sql(@"
                INSERT INTO GlobalSettings (Module, SettingKey, SettingValue, Description, Created, CreatedBy, Modified, ModifiedBy)
                SELECT 'Communications', 'ParentContactSource', 'ParentThenStudent',
                       'Where parent phone numbers and emails are taken from: ParentRecord, StudentRecord or ParentThenStudent.',
                       UTC_TIMESTAMP(), 'system', UTC_TIMESTAMP(), 'system'
                WHERE NOT EXISTS (SELECT 1 FROM GlobalSettings WHERE Module = 'Communications' AND SettingKey = 'ParentContactSource');");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM GlobalSettings WHERE Module = 'Communications';");

            migrationBuilder.DropTable(
                name: "OutboundMessages");

            migrationBuilder.DropTable(
                name: "MessageBatches");

            migrationBuilder.DropTable(
                name: "MessageTemplates");

            migrationBuilder.DropTable(
                name: "CommunicationSettings");
        }
    }
}
