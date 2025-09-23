using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.CBE.Cocurriculum
{
    public class CoCurriculumScore: SettingsBase
    {
        public int CoCurriculumScoreTypeId { get; set; }
        public CoCurriculumScoreType? CoCurriculumScoreType { get; set; } = null;
        public List<StudentCoCurriculumScore> StudentCoCurriculumScores { get; set; } = new();
    }
}
