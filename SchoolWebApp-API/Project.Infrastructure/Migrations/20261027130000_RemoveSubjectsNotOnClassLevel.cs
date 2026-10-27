using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Project.Infrastructure.Data;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    // Removes teacher allocations, and exams without results, for subjects that
    // are not set for the class's education level in the class's academic year.
    // They were left behind when subjects were unticked on a level after classes
    // had been allocated them; the API now refuses both (SubjectLevelGuard).
    // Student subjects and exams with results are left alone for a decision by hand.
    //
    // Hand-written rather than scaffolded - see the note on UniquePayrollTypeCodes.
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20261027130000_RemoveSubjectsNotOnClassLevel")]
    public partial class RemoveSubjectsNotOnClassLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE ss FROM StaffSubjects ss
                JOIN SchoolClasses c ON c.Id = ss.SchoolClassId
                JOIN LearningLevels ll ON ll.Id = c.LearningLevelId
                WHERE NOT EXISTS (SELECT 1 FROM EducationLevelSubjects e
                                  WHERE e.SubjectId = ss.SubjectId AND e.EducationLevelId = ll.EducationLevelId
                                    AND e.AcademicYearId = c.AcademicYearId);");

            migrationBuilder.Sql(@"
                DELETE x FROM Exams x
                JOIN SchoolClasses c ON c.Id = x.SchoolClassId
                JOIN LearningLevels ll ON ll.Id = c.LearningLevelId
                WHERE NOT EXISTS (SELECT 1 FROM EducationLevelSubjects e
                                  WHERE e.SubjectId = x.SubjectId AND e.EducationLevelId = ll.EducationLevelId
                                    AND e.AcademicYearId = c.AcademicYearId)
                  AND NOT EXISTS (SELECT 1 FROM ExamResults r WHERE r.ExamId = x.Id);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Deleted rows are not restored.
        }
    }
}
