using SchoolWebApp.Core.Entities.Settings;
using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolWebApp.Core.Entities.Students
{
    public class StudentParent: Base
    {
        [NotMapped]
        public override int Id { get; set; }
        public int RelationShipId { get; set; }
        public RelationShip? RelationShip { get; set; }
        public int StudentId { get; set; }
        public Student? Student { get; set; }
        public int ParentId { get; set; }
        public Parent? Parent { get; set; }
        public string? OtherDetails { get; set; }
    }
}
