using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.CBE.Responsibilities
{
    public class SocialSkill : SettingsBase
    {
        public List<ResponsibilitySocialSkill> ResponsibilitySocialSkills { get; set; } = new();
    }
}
