using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSchoolExamHeader : Migration
    {
        // Splits the exam "event" (type, term, schedule, release state) out
        // of each per-class-per-subject Exam row into a parent SchoolExam.
        // Existing Exam rows are folded into one header per (SessionId,
        // ExamTypeId) and re-pointed before the old columns are dropped.
        //
        // The auto-scaffolded Up/Down were rewritten by hand: EF tried to
        // rename Exams.SessionId -> SchoolExamId (which would carry session
        // ids into the FK) and drop the date/type columns before the data
        // could be migrated. The seed-data UpdateData churn EF emits on
        // every scaffold (including a regenerated admin password hash) was
        // also stripped, matching the convention in the other migrations.

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Create the new header table.
            migrationBuilder.CreateTable(
                name: "SchoolExams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ExamStartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ExamEndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ExamMarkEntryEndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExamTypeId = table.Column<int>(type: "int", nullable: false),
                    SessionId = table.Column<int>(type: "int", nullable: false),
                    IsReleased = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ReleasedBy = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReleasedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ParentsNotified = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ParentsNotifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolExams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchoolExams_ExamTypes_ExamTypeId",
                        column: x => x.ExamTypeId,
                        principalTable: "ExamTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SchoolExams_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolExams_ExamTypeId",
                table: "SchoolExams",
                column: "ExamTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolExams_SessionId",
                table: "SchoolExams",
                column: "SessionId");

            // 2. Back-fill: one header per (term, exam type). Dates that
            //    drifted across the detail rows collapse to the widest window.
            migrationBuilder.Sql(@"
                INSERT INTO `SchoolExams`
                    (`SessionId`, `ExamTypeId`, `ExamStartDate`, `ExamEndDate`, `ExamMarkEntryEndDate`,
                     `Description`, `IsReleased`, `ParentsNotified`, `Created`)
                SELECT `SessionId`, `ExamTypeId`,
                       MIN(`ExamStartDate`), MAX(`ExamEndDate`), MAX(`ExamMarkEntryEndDate`),
                       NULL, 0, 0, UTC_TIMESTAMP()
                FROM `Exams`
                GROUP BY `SessionId`, `ExamTypeId`;");

            // 3. Add the FK column, nullable while we populate it.
            migrationBuilder.AddColumn<int>(
                name: "SchoolExamId",
                table: "Exams",
                type: "int",
                nullable: true);

            // 4. Point each detail row at its header.
            migrationBuilder.Sql(@"
                UPDATE `Exams` e
                JOIN `SchoolExams` se
                  ON e.`SessionId` = se.`SessionId` AND e.`ExamTypeId` = se.`ExamTypeId`
                SET e.`SchoolExamId` = se.`Id`;");

            // 5. Every row is now populated, so make the FK required.
            migrationBuilder.AlterColumn<int>(
                name: "SchoolExamId",
                table: "Exams",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            // 6. Drop the old per-row columns now folded into the header.
            migrationBuilder.DropForeignKey(
                name: "FK_Exams_ExamTypes_ExamTypeId",
                table: "Exams");

            migrationBuilder.DropForeignKey(
                name: "FK_Exams_Sessions_SessionId",
                table: "Exams");

            migrationBuilder.DropIndex(
                name: "IX_Exams_ExamTypeId",
                table: "Exams");

            migrationBuilder.DropIndex(
                name: "IX_Exams_SessionId",
                table: "Exams");

            migrationBuilder.DropColumn(name: "ExamTypeId", table: "Exams");
            migrationBuilder.DropColumn(name: "SessionId", table: "Exams");
            migrationBuilder.DropColumn(name: "ExamStartDate", table: "Exams");
            migrationBuilder.DropColumn(name: "ExamEndDate", table: "Exams");
            migrationBuilder.DropColumn(name: "ExamMarkEntryEndDate", table: "Exams");

            // 7. Wire the new relationship.
            migrationBuilder.CreateIndex(
                name: "IX_Exams_SchoolExamId",
                table: "Exams",
                column: "SchoolExamId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exams_SchoolExams_SchoolExamId",
                table: "Exams",
                column: "SchoolExamId",
                principalTable: "SchoolExams",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 1. Re-add the old columns (FK columns nullable while we back-fill).
            migrationBuilder.AddColumn<int>(
                name: "ExamTypeId",
                table: "Exams",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SessionId",
                table: "Exams",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "ExamStartDate",
                table: "Exams",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateOnly>(
                name: "ExamEndDate",
                table: "Exams",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateOnly>(
                name: "ExamMarkEntryEndDate",
                table: "Exams",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            // 2. Copy the header values back down onto each detail row.
            migrationBuilder.Sql(@"
                UPDATE `Exams` e
                JOIN `SchoolExams` se ON e.`SchoolExamId` = se.`Id`
                SET e.`SessionId` = se.`SessionId`,
                    e.`ExamTypeId` = se.`ExamTypeId`,
                    e.`ExamStartDate` = se.`ExamStartDate`,
                    e.`ExamEndDate` = se.`ExamEndDate`,
                    e.`ExamMarkEntryEndDate` = se.`ExamMarkEntryEndDate`;");

            // 3. Restore NOT NULL on the FK columns.
            migrationBuilder.AlterColumn<int>(
                name: "ExamTypeId",
                table: "Exams",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SessionId",
                table: "Exams",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            // 4. Drop the new relationship and header table.
            migrationBuilder.DropForeignKey(
                name: "FK_Exams_SchoolExams_SchoolExamId",
                table: "Exams");

            migrationBuilder.DropIndex(
                name: "IX_Exams_SchoolExamId",
                table: "Exams");

            migrationBuilder.DropColumn(name: "SchoolExamId", table: "Exams");

            migrationBuilder.DropTable(name: "SchoolExams");

            // 5. Restore the old indexes and foreign keys.
            migrationBuilder.CreateIndex(
                name: "IX_Exams_ExamTypeId",
                table: "Exams",
                column: "ExamTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Exams_SessionId",
                table: "Exams",
                column: "SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exams_ExamTypes_ExamTypeId",
                table: "Exams",
                column: "ExamTypeId",
                principalTable: "ExamTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Exams_Sessions_SessionId",
                table: "Exams",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id");
        }
    }
}
