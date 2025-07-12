using SchoolWebApp.Core.DTOs.Settings;

namespace SchoolWebApp.Core.DTOs.School.EducationLevelType
{
    public class CreateEducationLevelTypeDto: BaseSettinsDto
    {
        public int Rank { get; set; }
        public string? Abbr { get; set; }
    }
}
