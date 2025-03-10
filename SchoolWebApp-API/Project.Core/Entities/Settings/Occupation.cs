using SchoolWebApp.Core.Entities.Shared;
using SchoolWebApp.Core.Entities.Students;

namespace SchoolWebApp.Core.Entities.Settings
{
    public class Occupation: SettingsBase
    {
        public List<Parent> Parents  { get; set; } = new();
    }
}
