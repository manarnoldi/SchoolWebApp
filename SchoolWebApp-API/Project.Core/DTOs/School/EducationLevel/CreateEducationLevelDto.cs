using SchoolWebApp.Core.DTOs.Settings;

namespace SchoolWebApp.Core.DTOs.School.EducationLevel
{
    public class CreateEducationLevelDto: BaseSettinsDto
    {
        public string? Abbr { get; set; }
        public int NumOfYears { get; set; }
        public int EducationLevelTypeId { get; set; }
        public int CurriculumId { get; set; }
    }
}
