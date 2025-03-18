using SchoolWebApp.Core.Entities.School;
using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.Academics
{
    public class EducationLevelSubject : Base
    {
        public string? Description { get; set; }
        public int EducationLevelId { get; set; }
        public EducationLevel? EducationLevel { get; set; }
        public int SubjectId { get; set; }
        public Subject? Subject { get; set; }
        public int AcademicYearId { get; set; }
        public AcademicYear? AcademicYear { get; set; }
    }
}
