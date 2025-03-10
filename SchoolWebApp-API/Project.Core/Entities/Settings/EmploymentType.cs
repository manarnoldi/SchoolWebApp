using SchoolWebApp.Core.Entities.Shared;
using SchoolWebApp.Core.Entities.Staff;

namespace SchoolWebApp.Core.Entities.Settings
{
    public class EmploymentType : SettingsBase
    {
        public List<StaffDetails> StaffDetails { get; set; } = new();
    }
}
