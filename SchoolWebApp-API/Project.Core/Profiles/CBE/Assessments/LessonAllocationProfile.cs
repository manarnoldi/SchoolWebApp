using AutoMapper;
using SchoolWebApp.Core.DTOs.CBE.Assessments.LessonAllocation;
using SchoolWebApp.Core.Entities.CBE.Assessments;

namespace SchoolWebApp.Core.Profiles.CBE.Assessments
{
    public class LessonAllocationProfile: Profile
    {
        public LessonAllocationProfile()
        {
            CreateMap<Entities.CBE.Assessments.LessonAllocation, LessonAllocationDto>();
            CreateMap<LessonAllocationDto, Entities.CBE.Assessments.LessonAllocation>();
            CreateMap<CreateLessonAllocationDto, Entities.CBE.Assessments.LessonAllocation>();
            CreateMap<CreateLessonAllocationDto, LessonAllocationDto>();
        }
    }
}
