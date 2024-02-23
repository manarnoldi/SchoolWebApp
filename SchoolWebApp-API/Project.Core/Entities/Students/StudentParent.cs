using SchoolWebApp.Core.Entities.Settings;
using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.Students
{
    public class StudentParent : Base
    {
        public int RelationShipId { get; set; }
        public RelationShip RelationShip { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public int ParentId { get; set; }
        public Parent Parent { get; set; }
        public string OtherDetails { get; set; }
    }
}
