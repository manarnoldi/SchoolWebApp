using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Assessments;

namespace SchoolWebApp.Core.Services.CBE.Assessments
{
    public class SubStrandService : GenericService<SubStrand>, ISubStrandService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubStrandService(IUnitOfWork unitOfWork)
        : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<SubStrand>> GetByStrandId(int strandId)
        {
            var subStrands = await _unitOfWork.Repository<SubStrand>().Find(a => a.StrandId == strandId, includeProperties:"Strand");
            return subStrands.ToList();
        }
    }
}
