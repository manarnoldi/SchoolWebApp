using AutoMapper;
using SchoolWebApp.Core.DTOs.Settings.StaffCategory;
using SchoolWebApp.Core.Entities.Settings;

namespace SchoolWebApp.Core.Profiles.Settings
{
    public class StaffCategoryProfile: Profile
    {
        public StaffCategoryProfile()
        {
            CreateMap<StaffCategory, StaffCategoryDto>();
            CreateMap<StaffCategoryDto, StaffCategory>();
            CreateMap<CreateStaffCategoryDto, StaffCategory>();
            CreateMap<CreateStaffCategoryDto, StaffCategoryDto>();
        }
    }
}
