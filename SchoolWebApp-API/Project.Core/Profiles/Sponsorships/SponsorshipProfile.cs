using AutoMapper;
using SchoolWebApp.Core.DTOs.Sponsorships;
using SchoolWebApp.Core.Entities.Sponsorships;

namespace SchoolWebApp.Core.Profiles.Sponsorships
{
    public class SponsorshipProfile : Profile
    {
        public SponsorshipProfile()
        {
            CreateMap<Sponsor, SponsorDto>()
                .ForMember(d => d.ReceivableAccountName, o => o.MapFrom(s => s.ReceivableAccount != null
                    ? (s.ReceivableAccount.Code + " - " + s.ReceivableAccount.Name) : null));
            CreateMap<CreateSponsorDto, Sponsor>();

            CreateMap<Sponsorship, SponsorshipDto>()
                .ForMember(d => d.SponsorName, o => o.MapFrom(s => s.Sponsor != null ? s.Sponsor.Name : null))
                .ForMember(d => d.StudentName, o => o.MapFrom(s => s.Student != null ? s.Student.FullName : null))
                .ForMember(d => d.StudentUPI, o => o.MapFrom(s => s.Student != null ? s.Student.UPI : null))
                .ForMember(d => d.SchoolClassName, o => o.MapFrom(s => s.SchoolClass != null
                    ? (s.SchoolClass.LearningLevel != null ? s.SchoolClass.LearningLevel.Name : "") : null))
                .ForMember(d => d.AcademicYearName, o => o.MapFrom(s => s.AcademicYear != null ? s.AcademicYear.Name : null))
                .ForMember(d => d.SessionName, o => o.MapFrom(s => s.Session != null ? s.Session.SessionName : null))
                .ForMember(d => d.FeeCategoryIds, o => o.MapFrom(s => s.FeeCategories.Select(x => x.FeeCategoryId).ToList()));

            CreateMap<SponsorPayment, SponsorPaymentDto>()
                .ForMember(d => d.SponsorName, o => o.MapFrom(s => s.Sponsor != null ? s.Sponsor.Name : null))
                .ForMember(d => d.BankAccountName, o => o.MapFrom(s => s.BankAccount != null
                    ? (s.BankAccount.Code + " - " + s.BankAccount.Name) : null));
        }
    }
}
