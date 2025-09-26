using SchoolWebApp.Core.DTOs.Academics.Subject;
using SchoolWebApp.Core.DTOs.Settings;

namespace SchoolWebApp.Core.DTOs.CBE.Assessments.Strand
{
    public class CreateStrandDto: BaseSettinsDto
    {
        public int SubjectId { get; set; }
        public SubjectDto? Subject { get; set; }
    }
}
