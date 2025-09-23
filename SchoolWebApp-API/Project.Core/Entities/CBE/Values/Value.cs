using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.CBE.Values
{
    public class Value: SettingsBase
    {
        public List<StudentValueScore> StudentValues { get; set; } = new();
    }
}
