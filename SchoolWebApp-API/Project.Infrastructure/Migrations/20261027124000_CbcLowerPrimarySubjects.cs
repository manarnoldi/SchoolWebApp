using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Project.Infrastructure.Data;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    // Subjects the KICD Pre-primary and Lower primary designs need:
    // - Hindu Religious Education (added like CRE: same group and department),
    //   taken in Pre-primary and Lower primary;
    // - Indigenous Language Activities, taken in Lower primary.
    // Linked for every academic year the level already has subjects for. Matched
    // by name; anything already there is left alone. Runs before
    // RemoveSubjectsNotOnClassLevel so classes already taught these keep them.
    //
    // Hand-written rather than scaffolded - see the note on UniquePayrollTypeCodes.
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20261027124000_CbcLowerPrimarySubjects")]
    public partial class CbcLowerPrimarySubjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO Subjects (Code, Name, Abbr, NumOfLessons, Description, Optional, `Rank`, SubjectGroupId, DepartmentId, Examinable, Created, CreatedBy)
                SELECT 'HRE', 'Hindu Religious Education', 'HRE', 3, '', 0, c.`Rank`, c.SubjectGroupId, c.DepartmentId, 1, NOW(), 'admin'
                FROM Subjects c
                WHERE c.Code = 'CRE'
                  AND NOT EXISTS (SELECT 1 FROM Subjects s WHERE s.Name = 'Hindu Religious Education')
                ORDER BY c.Id
                LIMIT 1;");

            migrationBuilder.Sql(@"
                INSERT INTO EducationLevelSubjects (Description, EducationLevelId, SubjectId, AcademicYearId, Created, CreatedBy)
                SELECT '', el.Id, s.Id, y.AcademicYearId, NOW(), 'admin'
                FROM EducationLevels el
                JOIN (SELECT DISTINCT EducationLevelId, AcademicYearId FROM EducationLevelSubjects) y ON y.EducationLevelId = el.Id
                JOIN Subjects s ON (el.Name = 'Pre-primary' AND s.Name = 'Hindu Religious Education')
                                OR (el.Name = 'Lower primary' AND s.Name IN ('Hindu Religious Education', 'Indigenous Language Activities'))
                WHERE NOT EXISTS (SELECT 1 FROM EducationLevelSubjects e
                                  WHERE e.EducationLevelId = el.Id AND e.SubjectId = s.Id AND e.AcademicYearId = y.AcademicYearId);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Subjects and their levels are school setup; left in place.
        }
    }
}
