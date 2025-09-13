using SchoolWebApp.Core.DTOs.Class.SchoolClass;
using SchoolWebApp.Core.DTOs.Students.Student;

namespace SchoolWebApp.Core.DTOs.Students.StudentClass
{
    public class CreateStudentClassDto
    {
        public string? Description { get; set; }
        public int StudentId { get; set; }
        public StudentDto? Student { get; set; }
        public int SchoolClassId { get; set; }
        public SchoolClassDto? SchoolClass { get; set; }

        public string? FullName { get; set; }
        public string? UPI { get; set; }
    }
}
