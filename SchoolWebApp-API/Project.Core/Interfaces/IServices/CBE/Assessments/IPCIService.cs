using SchoolWebApp.Core.Entities.CBE.Assessments;

namespace SchoolWebApp.Core.Interfaces.IServices.CBE.Assessments
{
    public interface IPCIService : IGenericService<PCI>
    {
        Task<List<PCI>> GetBySubStrandId(int subStrandId);
    }
}
