using SchoolWebApp.Core.Entities.Class;

namespace SchoolWebApp.Core.DTOs.Students.StudentClass
{
    public class CreateStudentClassDto
    {
        public string Description { get; set; }
        public int StudentId { get; set; }
        public int SchoolClassId { get; set; }
    }
}
