using AutoMapper;
using SchoolWebApp.Core.DTOs.Staff.StaffDiscipline;
using SchoolWebApp.Core.Entities.Staff;

namespace SchoolWebApp.Core.Profiles.Staff
{
    public class StaffDisciplineProfile: Profile
    {
        public StaffDisciplineProfile()
        {
            CreateMap<StaffDiscipline, StaffDisciplineDto>();
            CreateMap<StaffDisciplineDto, StaffDiscipline>();
            CreateMap<CreateStaffDisciplineDto, StaffDiscipline>();
            CreateMap<CreateStaffDisciplineDto, StaffDisciplineDto>();
        }
    }
}
