using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.CBE.Assessments
{
    public class Strand : SettingsBase
    {
        public int EducationLevelSubjectId { get; set; }
        public EducationLevelSubject? EducationLevelSubject { get; set; } = null;

        public List<SubStrand> SubStrands { get; set; } = new();
    }
}
