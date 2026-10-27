using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Project.Infrastructure.Data;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    // The optional foreign languages the KICD Upper primary designs cover - French
    // and Arabic - added like English (same group and department) and taken in
    // Upper primary, for every academic year the level already has subjects for.
    // Matched by name; anything already there is left alone.
    //
    // Hand-written rather than scaffolded - see the note on UniquePayrollTypeCodes.
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20261028090000_CbcForeignLanguages")]
    public partial class CbcForeignLanguages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            foreach (var (code, name) in new[] { ("FRE", "French"), ("ARA", "Arabic") })
                migrationBuilder.Sql($@"
                    INSERT INTO Subjects (Code, Name, Abbr, NumOfLessons, Description, Optional, `Rank`, SubjectGroupId, DepartmentId, Examinable, Created, CreatedBy)
                    SELECT '{code}', '{name}', '{code}', 2, '', 1, e.`Rank` + 1, e.SubjectGroupId, e.DepartmentId, 1, NOW(), 'admin'
                    FROM Subjects e
                    WHERE e.Code = 'ENG'
                      AND NOT EXISTS (SELECT 1 FROM Subjects s WHERE s.Name = '{name}')
                    ORDER BY e.Id
                    LIMIT 1;");

            migrationBuilder.Sql(@"
                INSERT INTO EducationLevelSubjects (Description, EducationLevelId, SubjectId, AcademicYearId, Created, CreatedBy)
                SELECT '', el.Id, s.Id, y.AcademicYearId, NOW(), 'admin'
                FROM EducationLevels el
                JOIN (SELECT DISTINCT EducationLevelId, AcademicYearId FROM EducationLevelSubjects) y ON y.EducationLevelId = el.Id
                JOIN Subjects s ON s.Name IN ('French', 'Arabic')
                WHERE el.Name = 'Upper primary'
                  AND NOT EXISTS (SELECT 1 FROM EducationLevelSubjects e
                                  WHERE e.EducationLevelId = el.Id AND e.SubjectId = s.Id AND e.AcademicYearId = y.AcademicYearId);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Subjects and their levels are school setup; left in place.
        }
    }
}
