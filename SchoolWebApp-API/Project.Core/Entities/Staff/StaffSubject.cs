using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.Staff
{
    public class StaffSubject : Base
    {
        public string? Description { get; set; }
        public int StaffDetailsId { get; set; }
        public StaffDetails? StaffDetails { get; set; }
        public int SubjectId { get; set; }
        public Subject? Subject { get; set; }
        public int SchoolClassId { get; set; }
        public SchoolClass? SchoolClass { get; set; }
    }
}
