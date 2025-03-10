using SchoolWebApp.Core.Entities.Shared;
using SchoolWebApp.Core.Entities.Students;

namespace SchoolWebApp.Core.Entities.Settings
{
    public class RelationShip : SettingsBase
    {
        public List<StudentParent> StudentParents { get; set; } = new();

    }
}
