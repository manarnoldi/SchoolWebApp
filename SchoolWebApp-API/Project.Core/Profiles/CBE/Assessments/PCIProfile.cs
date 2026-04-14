using AutoMapper;
using SchoolWebApp.Core.DTOs.CBE.Assessments.PCI;
using SchoolWebApp.Core.Entities.CBE.Assessments;

namespace SchoolWebApp.Core.Profiles.CBE.Assessments
{
    public class PCIProfile: Profile
    {
        public PCIProfile()
        {
            CreateMap<Entities.CBE.Assessments.PCI, PCIDto>();
            CreateMap<PCIDto, Entities.CBE.Assessments.PCI>();
            CreateMap<CreatePCIDto, Entities.CBE.Assessments.PCI>();
            CreateMap<CreatePCIDto, PCIDto>();
        }
    }
}
