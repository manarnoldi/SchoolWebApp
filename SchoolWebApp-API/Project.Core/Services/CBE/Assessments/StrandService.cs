using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Assessments;

namespace SchoolWebApp.Core.Services.CBE.Assessments
{
    public class StrandService : GenericService<Strand>, IStrandService
    {
        private readonly IUnitOfWork _unitOfWork;
        public StrandService(IUnitOfWork unitOfWork)
        : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Strand>> GetBySubjectId(int subjectId)
        {
            var strands = await _unitOfWork.Repository<Strand>()
                .Find(a => a.SubjectId == subjectId, includeProperties: "Subject");
            return strands.ToList();
        }
    }
}
