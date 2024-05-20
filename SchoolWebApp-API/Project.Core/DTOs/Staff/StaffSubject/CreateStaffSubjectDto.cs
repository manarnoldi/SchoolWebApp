namespace SchoolWebApp.Core.DTOs.Staff.StaffSubject
{
    public class CreateStaffSubjectDto
    {
        public string? Description { get; set; }
        public int StaffDetailsId { get; set; }
        public int SubjectId { get; set; }
        public int AcademicYearId { get; set; }
        public int SchoolClassId { get; set; }
    }
}
