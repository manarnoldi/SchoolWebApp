using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.CBE.Responsibilities
{
    public class ResponsibilitySocialSkill : Base
    {
        public int ResponsibilityId { get; set; }
        public Responsibility? Responsibility { get; set; } = null;
        public int SocialSkillId { get; set; }
        public SocialSkill? SocialSkill { get; set; } = null;
        public string? Description { get; set; }
    }
}
