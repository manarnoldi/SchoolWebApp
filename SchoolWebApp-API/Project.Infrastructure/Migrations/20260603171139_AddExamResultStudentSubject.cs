using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExamResultStudentSubject : Migration
    {
        // Ties every ExamResult to the StudentSubject (allocation) it belongs to,
        // with ON DELETE CASCADE so removing an allocation removes its results -
        // results can no longer be orphaned by a deallocation.
        //
        // The auto-scaffolded Up was rewritten by hand: it added a NOT NULL
        // column + cascade FK in one step, which would fail on existing rows
        // (no StudentSubjects.Id = 0). Instead we add the column nullable,
        // back-fill it, DELETE the orphans (results whose allocation no longer
        // exists - the records that were previously unreachable/undeletable),
        // then make it NOT NULL and add the FK. The seed-data UpdateData churn
        // EF emits on every scaffold was stripped (matches the other migrations).

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Add the FK column, nullable while we populate it.
            migrationBuilder.AddColumn<int>(
                name: "StudentSubjectId",
                table: "ExamResults",
                type: "int",
                nullable: true);

            // 2. Back-fill: match each result to the allocation of its student to
            //    the exam's subject in the exam's class.
            migrationBuilder.Sql(@"
                UPDATE `ExamResults` er
                JOIN `Exams` e ON er.`ExamId` = e.`Id`
                JOIN `StudentClasses` sc
                  ON sc.`StudentId` = er.`StudentId` AND sc.`SchoolClassId` = e.`SchoolClassId`
                JOIN `StudentSubjects` ss
                  ON ss.`StudentClassId` = sc.`Id` AND ss.`SubjectId` = e.`SubjectId`
                SET er.`StudentSubjectId` = ss.`Id`;");

            // 3. Delete orphans: results whose allocation no longer exists. These
            //    are exactly the rows that could not be reached/deleted from the
            //    UI after a subject was deallocated.
            migrationBuilder.Sql(@"
                DELETE FROM `ExamResults` WHERE `StudentSubjectId` IS NULL;");

            // 4. Now every remaining row is populated - make it required.
            migrationBuilder.AlterColumn<int>(
                name: "StudentSubjectId",
                table: "ExamResults",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            // 5. Index + cascade FK.
            migrationBuilder.CreateIndex(
                name: "IX_ExamResults_StudentSubjectId",
                table: "ExamResults",
                column: "StudentSubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamResults_StudentSubjects_StudentSubjectId",
                table: "ExamResults",
                column: "StudentSubjectId",
                principalTable: "StudentSubjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamResults_StudentSubjects_StudentSubjectId",
                table: "ExamResults");

            migrationBuilder.DropIndex(
                name: "IX_ExamResults_StudentSubjectId",
                table: "ExamResults");

            migrationBuilder.DropColumn(
                name: "StudentSubjectId",
                table: "ExamResults");
        }
    }
}
