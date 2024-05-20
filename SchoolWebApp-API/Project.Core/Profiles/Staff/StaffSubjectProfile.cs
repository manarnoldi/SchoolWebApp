using AutoMapper;
using SchoolWebApp.Core.DTOs.Staff.StaffSubject;
using SchoolWebApp.Core.Entities.Staff;

namespace SchoolWebApp.Core.Profiles.Staff
{
    public class StaffSubjectProfile: Profile
    {
        public StaffSubjectProfile()
        {
            CreateMap<StaffSubject, StaffSubjectDto>();
            CreateMap<StaffSubjectDto, StaffSubject>();
            CreateMap<CreateStaffSubjectDto, StaffSubject>();
            CreateMap<CreateStaffSubjectDto, StaffSubjectDto>();
        }
    }
}
