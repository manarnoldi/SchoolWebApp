using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Project.Infrastructure.Data;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    // Junior School (Grades 7-9) for the KICD designs:
    // - its own level learning outcomes, the nine printed in every Junior School
    //   design ("Learning outcomes for Junior School"), word for word;
    // - German added like French (optional, same group and department as English),
    //   and French, German and Indigenous Language taken in Junior School for every academic year the
    //   level already has subjects for.
    // Matched by name; anything already there is left alone.
    //
    // Hand-written rather than scaffolded - see the note on UniquePayrollTypeCodes.
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20261028120000_CbcJuniorSchool")]
    public partial class CbcJuniorSchool : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO GeneralOutcomes (EducationLevelTypeId, EducationLevelId, Name, Description, `Rank`, Created, CreatedBy)
                SELECT el.EducationLevelTypeId, el.Id, o.Name, '', o.r, NOW(), 'admin'
                FROM EducationLevels el
                CROSS JOIN (
                    SELECT 1 r, 'Apply literacy, numeracy and logical thinking skills for appropriate self-expression' Name
                    UNION ALL SELECT 2, 'Communicate effectively, verbally and non-verbally, in diverse contexts'
                    UNION ALL SELECT 3, 'Demonstrate social skills, spiritual and moral values for peaceful co-existence'
                    UNION ALL SELECT 4, 'Explore, manipulate, manage and conserve the environment effectively for learning and sustainable development'
                    UNION ALL SELECT 5, 'Practise relevant hygiene, sanitation and nutrition skills to promote health'
                    UNION ALL SELECT 6, 'Demonstrate ethical behaviour and exhibit good citizenship as a civic responsibility'
                    UNION ALL SELECT 7, 'Appreciate the country''s rich and diverse cultural heritage for harmonious co-existence'
                    UNION ALL SELECT 8, 'Manage pertinent and contemporary issues in society effectively'
                    UNION ALL SELECT 9, 'Apply digital literacy skills for communication and learning'
                ) o
                WHERE el.Name = 'Junior School'
                  AND NOT EXISTS (SELECT 1 FROM GeneralOutcomes g WHERE g.EducationLevelId = el.Id AND g.Name = o.Name);");

            migrationBuilder.Sql(@"
                INSERT INTO Subjects (Code, Name, Abbr, NumOfLessons, Description, Optional, `Rank`, SubjectGroupId, DepartmentId, Examinable, Created, CreatedBy)
                SELECT 'GER', 'German', 'GER', 2, '', 1, e.`Rank` + 1, e.SubjectGroupId, e.DepartmentId, 1, NOW(), 'admin'
                FROM Subjects e
                WHERE e.Code = 'ENG'
                  AND NOT EXISTS (SELECT 1 FROM Subjects s WHERE s.Name = 'German')
                ORDER BY e.Id
                LIMIT 1;");

            migrationBuilder.Sql(@"
                INSERT INTO EducationLevelSubjects (Description, EducationLevelId, SubjectId, AcademicYearId, Created, CreatedBy)
                SELECT '', el.Id, s.Id, y.AcademicYearId, NOW(), 'admin'
                FROM EducationLevels el
                JOIN (SELECT DISTINCT EducationLevelId, AcademicYearId FROM EducationLevelSubjects) y ON y.EducationLevelId = el.Id
                JOIN Subjects s ON s.Name IN ('French', 'German', 'Indigenous Language Activities')
                WHERE el.Name = 'Junior School'
                  AND NOT EXISTS (SELECT 1 FROM EducationLevelSubjects e
                                  WHERE e.EducationLevelId = el.Id AND e.SubjectId = s.Id AND e.AcademicYearId = y.AcademicYearId);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Outcomes and subjects are school setup; left in place.
        }
    }
}
