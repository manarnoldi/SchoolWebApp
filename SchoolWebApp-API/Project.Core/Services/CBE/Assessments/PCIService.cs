using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Assessments;

namespace SchoolWebApp.Core.Services.CBE.Assessments
{
    public class PCIService : GenericService<PCI>, IPCIService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PCIService(IUnitOfWork unitOfWork)
        : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<PCI>> GetBySubStrandId(int subStrandId)
        {
            var items = await _unitOfWork.Repository<PCI>()
                .Find(a => a.SubStrandId == subStrandId, includeProperties: "SubStrand");
            return items.ToList();
        }
    }
}
