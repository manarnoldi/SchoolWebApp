using SchoolWebApp.Core.Entities.School;
using SchoolWebApp.Core.Entities.Shared;
using SchoolWebApp.Core.Entities.Students;

namespace SchoolWebApp.Core.Entities.Settings
{
    public class SchoolLevel: SettingsBase
    {
        public List<SchoolDetails> SchoolDetails { get; set; }
        public List<FormerSchool> FormerSchools { get; set; }
    }
}
