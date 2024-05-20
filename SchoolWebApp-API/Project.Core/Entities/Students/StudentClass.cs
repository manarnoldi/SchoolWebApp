using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.Students
{
    public  class StudentClass: Base
    {
        public string Description { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }

        public int SchoolClassId { get; set; }
        public SchoolClass SchoolClass { get; set; }
        public List<StudentAttendance> StudentAttendances { get; set; }
    }
}
