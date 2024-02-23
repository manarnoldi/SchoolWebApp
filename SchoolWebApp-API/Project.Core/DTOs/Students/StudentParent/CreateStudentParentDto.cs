namespace SchoolWebApp.Core.DTOs.Students.StudentParent
{
    public class CreateStudentParentDto
    {
        public int RelationShipId { get; set; }
        public int StudentId { get; set; }
        public int ParentId { get; set; }
        public string OtherDetails { get; set; }
    }
}
