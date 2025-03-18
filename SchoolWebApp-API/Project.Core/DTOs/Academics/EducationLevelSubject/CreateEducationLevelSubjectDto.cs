using SchoolWebApp.Core.DTOs.Academics.AcademicYear;
using SchoolWebApp.Core.DTOs.Academics.Subject;
using SchoolWebApp.Core.DTOs.School.EducationLevel;

namespace SchoolWebApp.Core.DTOs.Academics.EducationLevelSubject
{
    public class CreateEducationLevelSubjectDto
    {
        public string? Description { get; set; }
        public int EducationLevelId { get; set; }
        public EducationLevelDto? EducationLevel { get; set; }
        public int SubjectId { get; set; }
        public SubjectDto? Subject { get; set; }
        public int AcademicYearId { get; set; }
        public AcademicYearDto? AcademicYear { get; set; }
    }
}
