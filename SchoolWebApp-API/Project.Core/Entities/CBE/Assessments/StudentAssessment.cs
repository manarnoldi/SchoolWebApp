using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.Settings;
using SchoolWebApp.Core.Entities.Shared;
using SchoolWebApp.Core.Entities.Staff;
using SchoolWebApp.Core.Entities.Students;

namespace SchoolWebApp.Core.Entities.CBE.Assessments
{
    public class StudentAssessment : Base
    {
        public int StudentId { get; set; }
        public Student? Student { get; set; } = null;
        public int SchoolClassId { get; set; }
        public SchoolClass? SchoolClass { get; set; } = null;
        public int SpecificOutcomeId { get; set; }
        public SpecificOutcome? SpecificOutcome { get; set; } = null;
        public int GradeId { get; set; }
        public Grade? Grade { get; set; } = null;
        public int SessionId { get; set; }
        public Session? Session { get; set; } = null;
        public int AssessmentTypeId { get; set; }
        public AssessmentType? AssessmentType { get; set; } = null;
        public DateTime? AssessmentDate { get; set; }
        public int StaffDetailsId { get; set; }
        public StaffDetails? StaffDetails { get; set; }
        public string? Description { get; set; } = null;
    }
}
