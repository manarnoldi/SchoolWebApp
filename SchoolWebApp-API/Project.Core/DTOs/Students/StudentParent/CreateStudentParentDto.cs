using SchoolWebApp.Core.DTOs.Settings.Relationship;
using SchoolWebApp.Core.DTOs.Students.Parent;
using SchoolWebApp.Core.DTOs.Students.Student;
using SchoolWebApp.Core.Entities.Settings;

namespace SchoolWebApp.Core.DTOs.Students.StudentParent
{
    public class CreateStudentParentDto
    {
        public int RelationShipId { get; set; }
        public RelationshipDto? RelationShip { get; set; }
        public int StudentId { get; set; }
        public StudentDto? Student { get; set; }
        public int ParentId { get; set; }
        public ParentDto? Parent { get; set; }
        public string? OtherDetails { get; set; }
    }
}
