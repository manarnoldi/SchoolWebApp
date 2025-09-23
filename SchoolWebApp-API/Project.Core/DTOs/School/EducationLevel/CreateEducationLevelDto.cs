using SchoolWebApp.Core.DTOs.Academics.Curriculum;
using SchoolWebApp.Core.DTOs.School.EducationLevelType;
using SchoolWebApp.Core.DTOs.Settings;

namespace SchoolWebApp.Core.DTOs.School.EducationLevel
{
    public class CreateEducationLevelDto: BaseSettinsDto
    {
        public string? Abbr { get; set; }
        public int NumOfYears { get; set; }
        public int EducationLevelTypeId { get; set; }
        public EducationLevelTypeDto? EducationLevelType { get; set; }
        public int CurriculumId { get; set; }
        public CurriculumDto? Curriculum { get; set; }
    }
}
