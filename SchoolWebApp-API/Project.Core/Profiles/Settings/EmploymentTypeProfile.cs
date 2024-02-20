using AutoMapper;
using SchoolWebApp.Core.DTOs.Settings.EmploymentType;
using SchoolWebApp.Core.Entities.Settings;

namespace SchoolWebApp.Core.Profiles.Settings
{
    public class EmploymentTypeProfile: Profile
    {
        public EmploymentTypeProfile()
        {
            CreateMap<EmploymentType, EmploymentTypeDto>();
            CreateMap<EmploymentTypeDto, EmploymentType>();
            CreateMap<CreateEmploymentTypeDto, EmploymentType>();
            CreateMap<CreateEmploymentTypeDto, EmploymentTypeDto>();
        }
    }
}
