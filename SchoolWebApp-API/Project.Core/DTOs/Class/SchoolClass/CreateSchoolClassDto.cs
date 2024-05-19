namespace SchoolWebApp.Core.DTOs.Class.SchoolClass
{
    public class CreateSchoolClassDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int LearningLevelId { get; set; }
        public int SchoolStreamId { get; set; }
        public int AcademicYearId { get; set; }
        public int StudentId { get; set; }
        public int StaffDetailsId { get; set; }
    }
}
