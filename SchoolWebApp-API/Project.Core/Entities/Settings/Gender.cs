using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.Settings
{
    public class Gender: SettingsBase
    {
        public List<Person> People { get; set; }
    }
}
