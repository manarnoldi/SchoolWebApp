using SchoolWebApp.Core.Entities.Shared;
using SchoolWebApp.Core.Entities.Staff;

namespace SchoolWebApp.Core.Entities.Settings
{
    public   class Designation: SettingsBase
    {
        public List<StaffDetails> StaffDetails { get; set; } = new();
    }
}
