using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Project.Infrastructure.Data;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    // One-time data cleanup. Previously, deleting a student's co-curricular
    // scores removed the score rows but left the enrolment
    // (StudentCoCurriculumActivity) behind. The report form lists enrolments, so
    // those "deleted" activities kept appearing. This removes the orphaned
    // enrolments — ones with no scores AND no remark (description). Enrolments
    // that still have scores or a remark are kept. Data-only; no schema change.
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260625120000_CleanupOrphanCoCurriculumEnrolments")]
    public partial class CleanupOrphanCoCurriculumEnrolments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM `StudentCoCurriculumActivities`
                WHERE (`Description` IS NULL OR `Description` = '')
                  AND `Id` NOT IN (
                      SELECT DISTINCT `StudentCoCurriculumActivityId`
                      FROM `StudentCoCurriculumScores`
                  );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Deleted orphan data cannot be restored.
        }
    }
}
