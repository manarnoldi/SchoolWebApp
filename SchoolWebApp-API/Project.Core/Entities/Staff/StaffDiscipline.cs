using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.Staff
{
    public class StaffDiscipline : Discipline
    {
        public int StaffId { get; set; }
        public StaffDetails StaffDetails { get; set; }        
    }
}
