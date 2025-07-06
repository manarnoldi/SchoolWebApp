using SchoolWebApp.Core.DTOs.Academics.ExamType;
using SchoolWebApp.Core.DTOs.School.EducationLevel;
using SchoolWebApp.Core.DTOs.Settings;

namespace SchoolWebApp.Core.DTOs.Academics.ExamName
{
    public class CreateExamNameDto : BaseSettinsDto
    {
        public int ExamTypeId { get; set; }
        public ExamTypeDto? ExamType { get; set; }
    }
}
