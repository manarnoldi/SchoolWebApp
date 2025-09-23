using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.Shared;
using SchoolWebApp.Core.Entities.Students;

namespace SchoolWebApp.Core.Entities.CBE.Responsibilities
{
    public class StudentResponsibility: Base
    {
        public int AcademicYearId { get; set; }
        public AcademicYear? AcademicYear { get; set; }

        public int StudentId { get; set; }
        public Student? Student { get; set; }

        public int ResponsibilitySocialSkillId { get; set; }

        public string? Description { get; set; } = null;
    }
}
