using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.Class
{
    public class SchoolClassLeaders : Base
    {
        public required int SchoolClassId { get; set; }
        public SchoolClass? SchoolClass { get; set; }
        public required int PersonId { get; set; }
        public Person? Person { get; set; }
        public required int ClassLeadershipRoleId { get; set; }
        public ClassLeadershipRole? ClassLeadershipRole { get; set; }
        public string? Description { get; set; }

    }
}
