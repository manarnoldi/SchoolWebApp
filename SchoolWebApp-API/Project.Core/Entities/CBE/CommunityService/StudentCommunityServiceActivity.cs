using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.Shared;
using SchoolWebApp.Core.Entities.Students;

namespace SchoolWebApp.Core.Entities.CBE.CommunityService
{
    public class StudentCommunityServiceActivity : Base
    {
        public int StudentId { get; set; }
        public Student? Student { get; set; } = null;
        public int CommunityServiceActivityId { get; set; }
        public CommunityServiceActivity? CommunityServiceActivity { get; set; } = null;
        public int SessionId { get; set; }
        public Session? Session { get; set; } = null;
        public int AcademicYearId { get; set; }
        public AcademicYear? AcademicYear { get; set; } = null;
        public string? Description { get; set; } = null;
    }
}
