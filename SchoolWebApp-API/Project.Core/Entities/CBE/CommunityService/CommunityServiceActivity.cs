using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.CBE.CommunityService
{
    public class CommunityServiceActivity : SettingsBase
    {
        public List<StudentCommunityServiceActivity> StudentCommunityServiceActivities { get; set; } = new();
    }
}
