using SchoolWebApp.Core.DTOs.Academics.Subject;
using SchoolWebApp.Core.DTOs.Class.SchoolClass;
using SchoolWebApp.Core.DTOs.Staff.StaffDetails;

namespace SchoolWebApp.Core.DTOs.Staff.StaffSubject
{
    public class CreateStaffSubjectDto
    {
        public string? Description { get; set; }
        public int StaffDetailsId { get; set; }
        public StaffDetailDto? StaffDetails { get; set; }
        public int SubjectId { get; set; }
        public SubjectDto? Subject { get; set; }
        public int SchoolClassId { get; set; }
        public SchoolClassDto? SchoolClass { get; set; }
    }
}
