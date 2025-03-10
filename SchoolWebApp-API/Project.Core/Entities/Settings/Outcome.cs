using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.Settings
{
    public class Outcome: SettingsBase
    {
        public List<Discipline> Disciplines { get; set; } = new();
    }
}
