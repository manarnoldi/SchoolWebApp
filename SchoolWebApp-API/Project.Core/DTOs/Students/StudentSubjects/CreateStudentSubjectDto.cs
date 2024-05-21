using SchoolWebApp.Core.Entities.Academics;

namespace SchoolWebApp.Core.DTOs.Students.StudentSubjects
{
    public class CreateStudentSubjectDto
    {
        public int StudentId { get; set; }
        public int SubjectId { get; set; }
        public int AcademicYearId { get; set; }
    }
}
