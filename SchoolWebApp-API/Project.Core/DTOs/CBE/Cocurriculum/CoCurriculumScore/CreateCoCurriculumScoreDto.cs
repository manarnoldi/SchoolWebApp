using SchoolWebApp.Core.DTOs.CBE.Cocurriculum.CoCurriculumScoreType;
using SchoolWebApp.Core.DTOs.Settings;

namespace SchoolWebApp.Core.DTOs.CBE.Cocurriculum.CoCurriculumScore
{
    public class CreateCoCurriculumScoreDto : BaseSettinsDto
    {
        public string? Abbreviation { get; set; }
        public int CoCurriculumScoreTypeId { get; set; }
        public CoCurriculumScoreTypeDto? CoCurriculumScoreType { get; set; }
    }
}
