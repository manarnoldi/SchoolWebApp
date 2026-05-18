using AutoMapper;
using SchoolWebApp.Core.DTOs.Approvals;
using SchoolWebApp.Core.Entities.Approvals;

namespace SchoolWebApp.Core.Profiles.Approvals
{
    public class ApprovalProfile : Profile
    {
        public ApprovalProfile()
        {
            CreateMap<ApprovalWorkflow, ApprovalWorkflowDto>();
            CreateMap<ApprovalWorkflowDto, ApprovalWorkflow>();
            CreateMap<CreateApprovalWorkflowDto, ApprovalWorkflow>();

            CreateMap<ApprovalWorkflowStep, ApprovalWorkflowStepDto>()
                .ForMember(d => d.RoleName, o => o.MapFrom(s => s.Role != null ? s.Role.Name : null));
            CreateMap<ApprovalWorkflowStepDto, ApprovalWorkflowStep>();
            CreateMap<CreateApprovalWorkflowStepDto, ApprovalWorkflowStep>();

            CreateMap<ApprovalRequest, ApprovalRequestDto>()
                .ForMember(d => d.SubmittedByName, o => o.MapFrom(s =>
                    s.SubmittedBy != null ? (s.SubmittedBy.FirstName + " " + s.SubmittedBy.LastName).Trim() : null));
            CreateMap<ApprovalStepAction, ApprovalStepActionDto>()
                .ForMember(d => d.AssignedToName, o => o.MapFrom(s =>
                    s.AssignedTo != null ? (s.AssignedTo.FirstName + " " + s.AssignedTo.LastName).Trim() : null))
                .ForMember(d => d.ActionedByName, o => o.MapFrom(s =>
                    s.ActionedBy != null ? (s.ActionedBy.FirstName + " " + s.ActionedBy.LastName).Trim() : null));
        }
    }
}
