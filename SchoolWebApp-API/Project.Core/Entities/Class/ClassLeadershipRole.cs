using SchoolWebApp.Core.Entities.Enums;
using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.Class
{
    public class ClassLeadershipRole: SettingsBase
    {
        public PersonType PersonType { get; set; }
        public List<SchoolClassLeaders> SchoolClassLeaders { get; set; } = new();
    }
}
