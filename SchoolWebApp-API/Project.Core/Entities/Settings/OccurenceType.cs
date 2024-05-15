using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.Settings
{
    public class OccurenceType: SettingsBase
    {
        public string? Abbreviation { get; set; }
        public List<Discipline> Disciplines { get; set; }
    }
}
