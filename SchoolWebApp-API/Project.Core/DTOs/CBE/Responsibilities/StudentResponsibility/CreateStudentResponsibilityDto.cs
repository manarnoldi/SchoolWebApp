using SchoolWebApp.Core.DTOs.CBE.Responsibilities.Responsibility;

namespace SchoolWebApp.Core.DTOs.CBE.Responsibilities.StudentResponsibility
{
    public class CreateStudentResponsibilityDto
    {
        public int AcademicYearId { get; set; }
        public int StudentId { get; set; }
        public int ResponsibilitySocialSkillId { get; set; }
        public ResponsibilityDto? ResponsibilitySocialSkill { get; set; }
        public string? Description { get; set; }
    }
}
