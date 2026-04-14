using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Assessments;

namespace SchoolWebApp.Core.Services.CBE.Assessments
{
    public class KeyQuestionService : GenericService<KeyQuestion>, IKeyQuestionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public KeyQuestionService(IUnitOfWork unitOfWork)
        : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<KeyQuestion>> GetBySubStrandId(int subStrandId)
        {
            var items = await _unitOfWork.Repository<KeyQuestion>()
                .Find(a => a.SubStrandId == subStrandId, includeProperties: "SubStrand");
            return items.ToList();
        }
    }
}
