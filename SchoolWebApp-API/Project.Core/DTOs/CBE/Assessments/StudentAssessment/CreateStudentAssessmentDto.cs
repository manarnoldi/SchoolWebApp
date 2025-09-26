using SchoolWebApp.Core.DTOs.Academics.Grade;
using SchoolWebApp.Core.DTOs.CBE.Assessments.AssessmentType;
using SchoolWebApp.Core.DTOs.CBE.Assessments.SpecificOutcome;
using SchoolWebApp.Core.DTOs.Class.SchoolClass;
using SchoolWebApp.Core.DTOs.Class.Session;
using SchoolWebApp.Core.DTOs.Staff.StaffDetails;
using SchoolWebApp.Core.DTOs.Students.Student;

namespace SchoolWebApp.Core.DTOs.CBE.Assessments.Assessment
{
    public class CreateStudentAssessmentDto
    {
        public int StudentId { get; set; }
        public StudentDto? Student { get; set; }
        public int SchoolClassId { get; set; }
        public SchoolClassDto? SchoolClass { get; set; }
        public int SpecificOutcomeId { get; set; }
        public SpecificOutcomeDto? SpecificOutcome { get; set; }
        public int GradeId { get; set; }
        public GradeDto? Grade { get; set; }
        public int SessionId { get; set; }
        public SessionDto? Session { get; set; }
        public int AssessmentTypeId { get; set; }
        public AssessmentTypeDto? AssessmentType { get; set; }
        public DateTime? AssessmentDate { get; set; }
        public int StaffDetailsId { get; set; }
        public StaffDetailDto? StaffDetails { get; set; }
        public string? Description { get; set; } = null;
    }
}
