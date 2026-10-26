namespace SchoolWebApp.Core.DTOs.CBE
{
    public class CurriculumImportIssueDto
    {
        public string? Sheet { get; set; }
        // Excel row number, or null for the workbook as a whole.
        public int? Row { get; set; }
        public string Message { get; set; } = "";
    }

    /// <summary>What an uploaded curriculum workbook holds, and (once committed) what was saved.</summary>
    public class CurriculumImportResultDto
    {
        public bool Committed { get; set; }
        public bool Saved { get; set; }
        public string? SubjectName { get; set; }
        public string? LearningLevelName { get; set; }
        public int SubjectOutcomes { get; set; }
        public int Strands { get; set; }
        public int SubStrands { get; set; }
        public int Lessons { get; set; }
        public int SpecificOutcomes { get; set; }
        public int LearningExperiences { get; set; }
        public int KeyQuestions { get; set; }
        public int Pcis { get; set; }
        // Counts of rows created / updated / removed on save, e.g. "Strands created": 5.
        public Dictionary<string, int> Changes { get; set; } = new();
        public List<CurriculumImportIssueDto> Errors { get; set; } = new();
        public List<CurriculumImportIssueDto> Warnings { get; set; } = new();
    }
}
