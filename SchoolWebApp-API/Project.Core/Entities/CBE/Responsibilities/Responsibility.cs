using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.CBE.Responsibilities
{
    public class Responsibility : SettingsBase
    {
        public List<ResponsibilitySocialSkill> ResponsibilitySocialSkills { get; set; } = new();
    }
}
