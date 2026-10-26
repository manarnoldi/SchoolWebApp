namespace SchoolWebApp.Core.DTOs.CBE
{
    // Editing the parts of a curriculum design that have no screen of their own.

    public class EditStatementDto
    {
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        // The core competency / national value / subject it is linked to, if any.
        public int? LinkId { get; set; }
    }

    public class EditRubricDto
    {
        public int? SubStrandId { get; set; }
        public string Indicator { get; set; } = "";
        public string? ExceedsExpectations { get; set; }
        public string? MeetsExpectations { get; set; }
        public string? ApproachesExpectations { get; set; }
        public string? BelowExpectations { get; set; }
    }

    public class EditSubStrandDto
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string Name { get; set; } = "";
        public List<EditStatementDto> CoreCompetencies { get; set; } = new();
        public List<EditStatementDto> Values { get; set; } = new();
        public List<EditStatementDto> LearningAreaLinks { get; set; } = new();
        public string? SuggestedResources { get; set; }
        public string? SuggestedAssessmentMethods { get; set; }
        public string? SuggestedNonFormalActivities { get; set; }
    }

    public class EditStrandDto
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string Name { get; set; } = "";
        public List<EditSubStrandDto> SubStrands { get; set; } = new();
        public List<EditRubricDto> Rubrics { get; set; } = new();
    }

    public class EditItemDto
    {
        public string Category { get; set; } = "";
        public string Name { get; set; } = "";
    }

    public class EditSectionDto
    {
        public string Title { get; set; } = "";
        public string? Content { get; set; }
    }

    public class LookupDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }

    public class CurriculumDesignEditDto
    {
        public int SubjectId { get; set; }
        public int LearningLevelId { get; set; }
        public string? Title { get; set; }
        public string? Source { get; set; }
        public string? EssenceStatement { get; set; }
        public int? LessonsPerWeek { get; set; }
        public List<EditItemDto> Items { get; set; } = new();
        public List<EditSectionDto> Sections { get; set; } = new();
        // Read with the design; ignored when it is saved.
        public List<EditStrandDto> Strands { get; set; } = new();
        public List<LookupDto> Competencies { get; set; } = new();
        public List<LookupDto> Values { get; set; } = new();
        public List<LookupDto> Subjects { get; set; } = new();
    }
}
