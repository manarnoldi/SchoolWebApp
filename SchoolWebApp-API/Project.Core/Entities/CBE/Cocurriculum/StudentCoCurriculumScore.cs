using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.CBE.Cocurriculum
{
    public class StudentCoCurriculumScore: Base
    {
        public int StudentCoCurriculumActivityId { get; set; }
        public StudentCoCurriculumActivity? StudentCoCurriculumActivity { get; set; } = null;

        public int CoCurriculumScoreId { get; set; }
        public CoCurriculumScore? CoCurriculumScore { get; set; } = null;
        public string? Description { get; set; } = null;
    }
}
