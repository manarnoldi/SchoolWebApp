using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.CBE.Assessments
{
    public class SubStrand: SettingsBase
    {
        public string? Code { get; set; }

        public int Version { get; set; }
        public int LessonNo { get; set; }

        public int StrandId { get; set; }
        public Strand? Strand { get; set; } = null;

        public int? CurriculumId { get; set; }
        public Curriculum? Curriculum { get; set; } = null;
        public int? AcademicYearId { get; set; }
        public AcademicYear? AcademicYear { get; set; } = null;

        public int? SubjectId { get; set; }
        public Subject? Subject { get; set; } = null;

        public int? LearningLevelId { get; set; }
        public LearningLevel? LearningLevel { get; set; } = null;

        public List<SpecificOutcome> SpecificOutcomes { get; set; } = new();
        public List<KeyQuestion> Questions { get; set; } = new();
        public List<LessonAllocation> LessonAllocations { get; set; } = new(); 
        public List<LearningExperience> LearningExperiences { get; set; } = new();
        public List<PCI> PCIs { get; set; } = new();
    }
}
