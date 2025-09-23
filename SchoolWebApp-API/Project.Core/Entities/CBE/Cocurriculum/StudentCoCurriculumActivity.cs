using SchoolWebApp.Core.Entities.Shared;
using SchoolWebApp.Core.Entities.Students;

namespace SchoolWebApp.Core.Entities.CBE.Cocurriculum
{
    public class StudentCoCurriculumActivity: Base
    {
        public int StudentId { get; set; }
        public Student? Student { get; set; } = null;
        public int CoCurriculumActivityId { get; set; }
        public CoCurriculumActivity? CoCurriculumActivity { get; set; } = null;
        public string? Description { get; set; } = null;
        public List<StudentCoCurriculumScore> StudentCoCurriculumScores { get; set; } = new();
    }
}