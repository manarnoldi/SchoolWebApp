using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.Shared;
namespace SchoolWebApp.Core.Entities.Settings
{
    public class SessionType : SettingsBase
    {
        public List<Session> Sessions { get; set; } = new();
    }
}
