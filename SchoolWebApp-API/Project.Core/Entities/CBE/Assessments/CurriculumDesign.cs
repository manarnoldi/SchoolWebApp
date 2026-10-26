using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.CBE.Values;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.CBE.Assessments
{
    /// <summary>
    /// The KICD curriculum design for a subject at one grade: what is not held
    /// strand by strand - the essence statement, time allocation, the appendix
    /// lists and every other section of the document, kept whole.
    /// </summary>
    public class CurriculumDesign : Base
    {
        public int SubjectId { get; set; }
        public Subject? Subject { get; set; }
        public int LearningLevelId { get; set; }
        public LearningLevel? LearningLevel { get; set; }
        [StringLength(255)]
        public string? Title { get; set; }
        // e.g. "KICD, first published 2017, revised 2024".
        [StringLength(255)]
        public string? Source { get; set; }
        public string? EssenceStatement { get; set; }
        public int? LessonsPerWeek { get; set; }
        public List<CurriculumDesignItem> Items { get; set; } = new();
        public List<CurriculumDesignSection> Sections { get; set; } = new();
    }

    /// <summary>Appendix lists: suggested resources, assessment methods and tools, non-formal programmes.</summary>
    public class CurriculumDesignItem : Base
    {
        public const string Resource = "Resource", AssessmentMethod = "AssessmentMethod",
            AssessmentTool = "AssessmentTool", NonFormalProgramme = "NonFormalProgramme";

        public int CurriculumDesignId { get; set; }
        public CurriculumDesign? CurriculumDesign { get; set; }
        [StringLength(50)]
        public required string Category { get; set; }
        [StringLength(1000)]
        public required string Name { get; set; }
        public int Rank { get; set; }
    }

    /// <summary>Any other part of the design, word for word: national goals, level outcomes, CSL appendix...</summary>
    public class CurriculumDesignSection : Base
    {
        public int CurriculumDesignId { get; set; }
        public CurriculumDesign? CurriculumDesign { get; set; }
        [StringLength(255)]
        public required string Title { get; set; }
        public string? Content { get; set; }
        public int Rank { get; set; }
    }

    /// <summary>
    /// "Core Competencies to be developed" under a sub-strand, e.g.
    /// "Communication and Collaboration: Speaking engagingly" with the statement
    /// that follows. Linked to the core competency when the name matches.
    /// </summary>
    public class SubStrandCompetency : Base
    {
        public int SubStrandId { get; set; }
        public SubStrand? SubStrand { get; set; }
        public int? CompetencyId { get; set; }
        public Competency? Competency { get; set; }
        [StringLength(255)]
        public required string Name { get; set; }
        public string? Description { get; set; }
        public int Rank { get; set; }
    }

    /// <summary>"Values" under a sub-strand, e.g. "Unity: Cooperation" and its statement; linked to the national value.</summary>
    public class SubStrandValue : Base
    {
        public int SubStrandId { get; set; }
        public SubStrand? SubStrand { get; set; }
        public int? ValueId { get; set; }
        public Value? Value { get; set; }
        [StringLength(255)]
        public required string Name { get; set; }
        public string? Description { get; set; }
        public int Rank { get; set; }
    }

    /// <summary>"Link to other learning areas / activity areas" under a sub-strand.</summary>
    public class SubStrandLearningAreaLink : Base
    {
        public int SubStrandId { get; set; }
        public SubStrand? SubStrand { get; set; }
        // The learning area named, when it matches a subject.
        public int? SubjectId { get; set; }
        public Subject? Subject { get; set; }
        [StringLength(255)]
        public required string LearningArea { get; set; }
        public string? Description { get; set; }
        public int Rank { get; set; }
    }

    /// <summary>
    /// An assessment rubric indicator with its four performance levels. A
    /// design gives rubrics for a whole strand (SubStrandId null) or for one
    /// sub-strand.
    /// </summary>
    public class AssessmentRubric : Base
    {
        public int StrandId { get; set; }
        public Strand? Strand { get; set; }
        public int? SubStrandId { get; set; }
        public SubStrand? SubStrand { get; set; }
        public int Rank { get; set; }
        public required string Indicator { get; set; }
        public string? ExceedsExpectations { get; set; }
        public string? MeetsExpectations { get; set; }
        public string? ApproachesExpectations { get; set; }
        public string? BelowExpectations { get; set; }
    }
}
