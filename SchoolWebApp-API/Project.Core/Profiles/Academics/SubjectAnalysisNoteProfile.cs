using AutoMapper;
using SchoolWebApp.Core.DTOs.Academics.SubjectAnalysisNote;
using SchoolWebApp.Core.Entities.Academics;

namespace SchoolWebApp.Core.Profiles.Academics
{
    public class SubjectAnalysisNoteProfile : Profile
    {
        public SubjectAnalysisNoteProfile()
        {
            CreateMap<SubjectAnalysisNote, SubjectAnalysisNoteDto>();
            CreateMap<SubjectAnalysisNoteDto, SubjectAnalysisNote>();
        }
    }
}
