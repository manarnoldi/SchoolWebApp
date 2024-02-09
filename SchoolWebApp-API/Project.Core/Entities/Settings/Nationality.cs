using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.Settings
{
    public class Nationality: SettingsBase
    {
        public List<Person> People { get; set; }
    }
}
