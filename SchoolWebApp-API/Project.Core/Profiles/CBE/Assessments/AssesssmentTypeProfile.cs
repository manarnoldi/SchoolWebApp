using AutoMapper;
using SchoolWebApp.Core.DTOs.CBE.Assessments.AssessmentType;
using SchoolWebApp.Core.Entities.CBE.Assessments;

namespace SchoolWebApp.Core.Profiles.CBE.Assessments
{
    public class AssesssmentTypeProfile: Profile
    {
        public AssesssmentTypeProfile()
        {
            CreateMap<AssessmentType, AssessmentTypeDto>();
            CreateMap<AssessmentTypeDto, AssessmentType>();
            CreateMap<CreateAssessmentTypeDto, AssessmentType>();
            CreateMap<CreateAssessmentTypeDto, AssessmentTypeDto>(); ;
        }
    }
}
