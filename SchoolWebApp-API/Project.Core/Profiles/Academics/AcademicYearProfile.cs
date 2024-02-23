using AutoMapper;
using SchoolWebApp.Core.DTOs.Academics.AcademicYear;
using SchoolWebApp.Core.Entities.Academics;

namespace SchoolWebApp.Core.Profiles.Academics
{
    public class AcademicYearProfile: Profile
    {
        public AcademicYearProfile()
        {
            CreateMap<AcademicYear, AcademicYearDto>();
            CreateMap<AcademicYearDto, AcademicYear>();
            CreateMap<CreateAcademicYearDto, AcademicYear>();
            CreateMap<CreateAcademicYearDto, AcademicYearDto>();
        }
    }
}
