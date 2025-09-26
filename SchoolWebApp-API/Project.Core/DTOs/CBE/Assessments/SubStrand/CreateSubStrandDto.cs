using SchoolWebApp.Core.DTOs.CBE.Assessments.Strand;
using SchoolWebApp.Core.DTOs.Settings;

namespace SchoolWebApp.Core.DTOs.CBE.Assessments.SubStrand
{
    public class CreateSubStrandDto: BaseSettinsDto
    {
        public int StrandId { get; set; }
        public StrandDto? Strand { get; set; }
    }
}
