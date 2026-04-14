using SchoolWebApp.Core.DTOs.Academics.Curriculum;
using SchoolWebApp.Core.DTOs.Academics.Subject;
using SchoolWebApp.Core.DTOs.CBE.Assessments.Theme;
using SchoolWebApp.Core.DTOs.Class.LearningLevel;
using SchoolWebApp.Core.DTOs.Settings;

namespace SchoolWebApp.Core.DTOs.CBE.Assessments.Strand
{
    public class CreateStrandDto: BaseSettinsDto
    {
        public string? Code { get; set; }

        public int? CurriculumId { get; set; }
        public CurriculumDto? Curriculum { get; set; } = null;

        public int SubjectId { get; set; }
        public SubjectDto? Subject { get; set; }

        public int LearningLevelId { get; set; }
        public LearningLevelDto? LearningLevel { get; set; } = null;

        public int? ThemeId { get; set; }
        public ThemeDto? Theme { get; set; } = null;
    }
}
