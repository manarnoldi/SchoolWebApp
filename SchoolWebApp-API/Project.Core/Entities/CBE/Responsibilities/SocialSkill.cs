using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.CBE.Responsibilities
{
    public class SocialSkill : SettingsBase
    {
        public List<Responsibility> Responsibilities { get; set; } = new();
    }
}
