using SchoolWebApp.Core.DTOs.Settings;
using SchoolWebApp.Core.Entities.Enums;

namespace SchoolWebApp.Core.DTOs.Class.ClassLeadershipRole
{
    public class CreateClassLeadershipRoleDto : BaseSettinsDto
    {
        public PersonType PersonType { get; set; }
    }
}
