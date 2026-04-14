using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.CBE.Values
{
    public class ValueScore: SettingsBase
    {
        public string? Abbreviation { get; set; }
        public List<StudentValueScore> StudentValues { get; set; } = new();
    }
}
