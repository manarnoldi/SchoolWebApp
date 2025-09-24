using SchoolWebApp.Core.DTOs.Academics.AcademicYear;
using SchoolWebApp.Core.DTOs.CBE.Assessments.Assessment;
using AutoMapper;
using SchoolWebApp.Core.Entities.CBE.Assessments;

namespace SchoolWebApp.Core.Profiles.CBE.Assessments
{
    public class AssessmentProfile : Profile
    {
        public AssessmentProfile()
        {
            CreateMap<Assessment, AssessmentDto>();
            CreateMap<AssessmentDto, Assessment>();
            CreateMap<CreateAssessmentDto, Assessment>();
            CreateMap<CreateAssessmentDto, AssessmentDto>();
        }
    }
}
