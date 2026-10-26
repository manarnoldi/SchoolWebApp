namespace SchoolWebApp.Core.DTOs.CBE
{
    // A subject's whole curriculum design for one grade, for reading on screen.

    public class CurriculumStatementDto
    {
        public string Name { get; set; } = "";
        public string? Description { get; set; }
    }

    public class CurriculumRubricDto
    {
        public string Indicator { get; set; } = "";
        public string? ExceedsExpectations { get; set; }
        public string? MeetsExpectations { get; set; }
        public string? ApproachesExpectations { get; set; }
        public string? BelowExpectations { get; set; }
    }

    public class CurriculumSubStrandViewDto
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string Name { get; set; } = "";
        public int Lessons { get; set; }
        public List<string> SpecificOutcomes { get; set; } = new();
        public List<string> LearningExperiences { get; set; } = new();
        public List<string> KeyQuestions { get; set; } = new();
        public List<CurriculumStatementDto> CoreCompetencies { get; set; } = new();
        public List<CurriculumStatementDto> Values { get; set; } = new();
        public List<CurriculumStatementDto> Pcis { get; set; } = new();
        public List<CurriculumStatementDto> LearningAreaLinks { get; set; } = new();
        public string? SubjectOutcome { get; set; }
        public string? LevelOutcome { get; set; }
        public string? SuggestedResources { get; set; }
        public string? SuggestedAssessmentMethods { get; set; }
        public string? SuggestedNonFormalActivities { get; set; }
        public List<CurriculumRubricDto> Rubrics { get; set; } = new();
    }

    public class CurriculumStrandViewDto
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string Name { get; set; } = "";
        public List<CurriculumSubStrandViewDto> SubStrands { get; set; } = new();
        // Rubrics for the whole strand.
        public List<CurriculumRubricDto> Rubrics { get; set; } = new();
    }

    public class CurriculumAppendixListDto
    {
        public string Label { get; set; } = "";
        public List<string> Items { get; set; } = new();
    }

    public class CurriculumSectionDto
    {
        public string Title { get; set; } = "";
        public string? Content { get; set; }
    }

    public class CurriculumViewDto
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = "";
        public int LearningLevelId { get; set; }
        public string LearningLevelName { get; set; } = "";
        public string? Title { get; set; }
        public string? Source { get; set; }
        public string? EssenceStatement { get; set; }
        public int? LessonsPerWeek { get; set; }
        public int TotalLessons { get; set; }
        public List<string> SubjectOutcomes { get; set; } = new();
        public List<CurriculumStrandViewDto> Strands { get; set; } = new();
        public List<CurriculumAppendixListDto> Appendix { get; set; } = new();
        public List<CurriculumSectionDto> Sections { get; set; } = new();
    }
}
