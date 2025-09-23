using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.CBE.Cocurriculum
{
    public class CoCurriculumScoreType: SettingsBase
    {
        public List<CoCurriculumScore> CoCurriculumScores { get; set; } = new();
    }
}
