using AutoMapper;
using SchoolWebApp.Core.DTOs.School.Department;
using SchoolWebApp.Core.Entities.School;

namespace SchoolWebApp.Core.Profiles.School
{
    public class DepartmentProfile: Profile
    {
        public DepartmentProfile()
        {
            CreateMap<Department, DepartmentDto>();
            CreateMap<DepartmentDto, Department>();
            CreateMap<CreateDepartmentDto, Department>();
            CreateMap<CreateDepartmentDto, DepartmentDto>();
        }
    }
}
