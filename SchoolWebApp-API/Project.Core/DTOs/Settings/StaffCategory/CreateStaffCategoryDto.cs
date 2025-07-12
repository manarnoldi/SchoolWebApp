namespace SchoolWebApp.Core.DTOs.Settings.StaffCategory
{
    public class CreateStaffCategoryDto : BaseSettinsDto
    {
        public required string Code { get; set; }
        public bool ForTeaching { get; set; }
    }
}
