using SchoolWebApp.Core.DTOs.Academics.AcademicYear;
using SchoolWebApp.Core.DTOs.Class.LearningLevel;
using SchoolWebApp.Core.DTOs.School.SchoolStream;

namespace SchoolWebApp.Core.DTOs.Class.SchoolClass
{
    public class CreateSchoolClassDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int LearningLevelId { get; set; }
        public LearningLevelDto? LearningLevel { get; set; }
        public int SchoolStreamId { get; set; }
        public SchoolStreamDto? SchoolStream { get; set; }
        public int AcademicYearId { get; set; }
        public AcademicYearDto? AcademicYear { get; set; }
        
    }
}
