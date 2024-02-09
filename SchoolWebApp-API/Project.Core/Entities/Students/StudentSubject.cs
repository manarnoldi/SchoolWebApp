using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.Students
{
    public class StudentSubject : Base
    {
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }
        public int AcademicYearId {get; set; }
        public AcademicYear AcademicYear {get; set; }
    }
}
