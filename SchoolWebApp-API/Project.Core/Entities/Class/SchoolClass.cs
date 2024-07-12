using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.School;
using SchoolWebApp.Core.Entities.Shared;
using SchoolWebApp.Core.Entities.Staff;
using SchoolWebApp.Core.Entities.Students;

namespace SchoolWebApp.Core.Entities.Class
{
    public class SchoolClass : Base
    {
        public required string Name { get; set; }
        public string? Description { get; set; }

        public int LearningLevelId { get; set; }
        public LearningLevel LearningLevel { get; set; }

        public int SchoolStreamId { get; set; }
        public SchoolStream SchoolStream { get; set; }

        public int AcademicYearId { get; set; }
        public AcademicYear AcademicYear { get; set; }

        public List<SchoolClassLeaders> SchoolClassLeaders { get; set; }
        public List<Exam> Exams { get; set; }
        public List<StudentClass> StudentClasses { get; set; }
        public List<StaffSubject> StaffSubjects { get; set; }
    }
}
