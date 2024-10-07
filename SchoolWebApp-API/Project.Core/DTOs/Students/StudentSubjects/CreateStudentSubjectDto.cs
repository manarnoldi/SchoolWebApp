using SchoolWebApp.Core.DTOs.Academics.Subject;
using SchoolWebApp.Core.DTOs.Students.StudentClass;

namespace SchoolWebApp.Core.DTOs.Students.StudentSubjects
{
    public class CreateStudentSubjectDto
    {
        public int StudentClassId { get; set; }
        public StudentClassDto? StudentClass { get; set; }
        public int SubjectId { get; set; }
        public SubjectDto? Subject { get; set; }
        public string? Description { get; set; }
    }
}
