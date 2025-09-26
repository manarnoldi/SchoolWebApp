using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.CBE.Responsibilities
{
    public class Responsibility : SettingsBase
    {
        public List<SocialSkill> SocialSkills { get; set; } = new();
    }
}
