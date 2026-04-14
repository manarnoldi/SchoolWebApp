using SchoolWebApp.Core.DTOs.CBE.Cocurriculum.CoCurriculumActivity;

namespace SchoolWebApp.Core.DTOs.CBE.Cocurriculum.StudentCoCurriculumActivity
{
    public class CreateStudentCoCurriculumActivityDto
    {
        public int StudentId { get; set; }
        public int CoCurriculumActivityId { get; set; }
        public CoCurriculumActivityDto? CoCurriculumActivity { get; set; }
        public string? Description { get; set; }
    }
}
