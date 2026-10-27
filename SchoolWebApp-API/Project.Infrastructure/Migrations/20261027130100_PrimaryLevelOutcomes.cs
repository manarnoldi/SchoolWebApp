using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Project.Infrastructure.Data;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    // Stores the KICD "Level learning outcomes for Primary Education" (Grade 1-6
    // curriculum designs, revised 2024) for the Lower primary and Upper primary
    // education levels on their own. Levels are matched by name; a school without
    // them gets nothing. Pre-primary and Junior School keep their type's outcomes.
    //
    // Hand-written rather than scaffolded - see the note on UniquePayrollTypeCodes.
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20261027130100_PrimaryLevelOutcomes")]
    public partial class PrimaryLevelOutcomes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO GeneralOutcomes (EducationLevelTypeId, EducationLevelId, Name, Description, `Rank`, Created, CreatedBy)
                SELECT el.EducationLevelTypeId, el.Id, o.Name, '', o.r, NOW(), 'admin'
                FROM EducationLevels el
                CROSS JOIN (
                    SELECT 1 r, 'Communicate appropriately using verbal and or non-verbal modes in a variety of contexts' Name
                    UNION ALL SELECT 2, 'Demonstrate mastery of number concepts to solve problems in day to day life'
                    UNION ALL SELECT 3, 'Demonstrate social skills, moral and religious values for positive contribution to society'
                    UNION ALL SELECT 4, 'Develop one''s interests and talents for personal fulfilment'
                    UNION ALL SELECT 5, 'Make informed decisions as local and global citizens of a diverse, democratic society in an interdependent world'
                    UNION ALL SELECT 6, 'Explore, manipulate, manage and conserve the environment effectively for learning and sustainable development'
                    UNION ALL SELECT 7, 'Acquire digital literacy skills for learning and enjoyment'
                    UNION ALL SELECT 8, 'Appreciate the country''s rich, diverse cultural heritage for harmonious living'
                ) o
                WHERE el.Name IN ('Lower primary', 'Upper primary')
                  AND NOT EXISTS (SELECT 1 FROM GeneralOutcomes g WHERE g.EducationLevelId = el.Id AND g.Name = o.Name);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE g FROM GeneralOutcomes g
                JOIN EducationLevels el ON el.Id = g.EducationLevelId
                WHERE el.Name IN ('Lower primary', 'Upper primary')
                  AND NOT EXISTS (SELECT 1 FROM SpecificOutcomes s WHERE s.GeneralOutcomeId = g.Id);");
        }
    }
}
