using AutoMapper;
using SchoolWebApp.Core.DTOs.CBE.Assessments.Assessment;
using SchoolWebApp.Core.Entities.CBE.Assessments;

namespace SchoolWebApp.Core.Profiles.CBE.Assessments
{
    public class StudentAssessmentProfile : Profile
    {
        public StudentAssessmentProfile()
        {
            CreateMap<StudentAssessment, StudentAssessmentDto>();
            CreateMap<StudentAssessmentDto, StudentAssessment>();
            CreateMap<CreateStudentAssessmentDto, StudentAssessment>();
            CreateMap<CreateStudentAssessmentDto, StudentAssessmentDto>();
        }
    }
}
