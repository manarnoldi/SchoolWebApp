using SchoolWebApp.Core.DTOs.Academics.AcademicYear;
using SchoolWebApp.Core.DTOs.Academics.Subject;
using SchoolWebApp.Core.DTOs.Class.LearningLevel;
using SchoolWebApp.Core.DTOs.Settings;

namespace SchoolWebApp.Core.DTOs.CBE.Assessments.Strand
{
    public class CreateStrandDto: BaseSettinsDto
    {
        public int AcademicYearId { get; set; }
        public AcademicYearDto? AcademicYear { get; set; } = null;

        public int SubjectId { get; set; }
        public SubjectDto? Subject { get; set; }

        public int LearningLevelId { get; set; }
        public LearningLevelDto? LearningLevel { get; set; }= null;
    }
}
