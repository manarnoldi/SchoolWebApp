using SchoolWebApp.Core.DTOs.Class.ClassLeadershipRole;
using SchoolWebApp.Core.DTOs.Class.SchoolClass;

namespace SchoolWebApp.Core.DTOs.Class.SchoolClassLeaders
{
    public class CreateSchoolClassLeadersDto
    {
        public required int SchoolClassId { get; set; }
        public SchoolClassDto? SchoolClass { get; set; }
        public int PersonId { get; set; }
        public PersonDto? Person { get; set; }
        public required int ClassLeadershipRoleId { get; set; }
        public ClassLeadershipRoleDto? ClassLeadershipRole { get; set; }
        public string? Description { get; set; }
    }
}
