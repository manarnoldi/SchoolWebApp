using AutoMapper;
using SchoolWebApp.Core.DTOs.Staff.StaffDetails;
using SchoolWebApp.Core.Entities.Staff;

namespace SchoolWebApp.Core.Profiles.Staff
{
    public class StaffDetailProfile: Profile
    {
        public StaffDetailProfile()
        {
            CreateMap<StaffDetails, StaffDetailDto>();
            CreateMap<StaffDetailDto, StaffDetails>();
            CreateMap<CreateStaffDetailDto, StaffDetails>();
            CreateMap<CreateStaffDetailDto, StaffDetailDto>();
        }
    }
}
