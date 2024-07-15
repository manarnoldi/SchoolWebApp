using SchoolWebApp.Core.DTOs.Staff.StaffDetails;

namespace SchoolWebApp.Core.DTOs.Staff.StaffDiscipline
{
    public class CreateStaffDisciplineDto: DisciplineDto
    {
        public int StaffDetailsId { get; set; }
        public StaffDetailDto? StaffDetails { get; set; }
    }
}
