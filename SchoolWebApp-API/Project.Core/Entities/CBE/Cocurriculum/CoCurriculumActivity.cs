using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.CBE.Cocurriculum
{
    public class CoCurriculumActivity: SettingsBase
    {
        public List<StudentCoCurriculumActivity> StudentCoCurriculumActivities { get; set; } = new();

    }
}
