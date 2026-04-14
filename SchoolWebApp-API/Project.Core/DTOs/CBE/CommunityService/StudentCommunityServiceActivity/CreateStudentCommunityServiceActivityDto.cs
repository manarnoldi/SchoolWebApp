using SchoolWebApp.Core.DTOs.CBE.CommunityService.CommunityServiceActivity;

namespace SchoolWebApp.Core.DTOs.CBE.CommunityService.StudentCommunityServiceActivity
{
    public class CreateStudentCommunityServiceActivityDto
    {
        public int StudentId { get; set; }
        public int CommunityServiceActivityId { get; set; }
        public CommunityServiceActivityDto? CommunityServiceActivity { get; set; }
        public int SessionId { get; set; }
        public int AcademicYearId { get; set; }
        public string? Description { get; set; }
    }
}
