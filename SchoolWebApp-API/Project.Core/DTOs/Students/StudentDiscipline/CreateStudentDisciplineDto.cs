using SchoolWebApp.Core.DTOs.Staff.StaffDetails;
using SchoolWebApp.Core.DTOs.Students.Student;

namespace SchoolWebApp.Core.DTOs.Students.StudentDiscipline
{
    public class CreateStudentDisciplineDto: DisciplineDto
    {
        public int StudentId { get; set; }
        public StudentDto? Student { get; set; }
    }
}
