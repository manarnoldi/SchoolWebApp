using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Project.Infrastructure.Data;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    // Clears the curriculum (strands, sub-strands, outcomes and what hangs off them)
    // entered before the KICD designs were loaded through Curriculum Upload; the
    // designs are loaded afresh at startup (CurriculumSeeder). Only on a database
    // where no design has been uploaded yet and no learner assessment points at
    // the curriculum - otherwise nothing is touched.
    //
    // Hand-written rather than scaffolded - see the note on UniquePayrollTypeCodes.
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20261027124500_ClearPreKicdCurriculum")]
    public partial class ClearPreKicdCurriculum : Migration
    {
        private const string Guard = @"
                  NOT EXISTS (SELECT 1 FROM CurriculumDesigns)
              AND NOT EXISTS (SELECT 1 FROM StudentAssessments
                              WHERE StrandId IS NOT NULL OR SubStrandId IS NOT NULL OR SpecificOutcomeId IS NOT NULL)";

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Children first; rubrics, sub-strand values/competencies/links cascade,
            // and scheme / lesson plan references are set to null by their keys.
            foreach (var table in new[]
            {
                "SpecificOutcomeCompetencies", "SpecificOutcomes", "LearningExperience", "KeyQuestion",
                "PCI", "LessonAllocation", "SubStrands", "Strands", "Themes", "BroadOutcomes"
            })
                migrationBuilder.Sql($"DELETE FROM {table} WHERE {Guard};");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Deleted rows are not restored.
        }
    }
}
