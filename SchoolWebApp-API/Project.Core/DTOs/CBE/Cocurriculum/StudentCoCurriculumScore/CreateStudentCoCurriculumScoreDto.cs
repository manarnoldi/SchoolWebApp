using SchoolWebApp.Core.DTOs.CBE.Cocurriculum.CoCurriculumScore;

namespace SchoolWebApp.Core.DTOs.CBE.Cocurriculum.StudentCoCurriculumScore
{
    public class CreateStudentCoCurriculumScoreDto
    {
        public int StudentCoCurriculumActivityId { get; set; }
        public int CoCurriculumScoreId { get; set; }
        public CoCurriculumScoreDto? CoCurriculumScore { get; set; }
        public string? Description { get; set; }
    }
}
