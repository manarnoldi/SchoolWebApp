using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.Settings
{
    public class OccurenceType: SettingsBase
    {
        public List<Discipline> Disciplines { get; set; }
    }
}
