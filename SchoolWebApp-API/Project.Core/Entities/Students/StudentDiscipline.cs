using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.Students
{
    public class StudentDiscipline : Discipline
    {
        public int StudentId { get; set; }
        public Student Student { get; set; }        
    }
}
