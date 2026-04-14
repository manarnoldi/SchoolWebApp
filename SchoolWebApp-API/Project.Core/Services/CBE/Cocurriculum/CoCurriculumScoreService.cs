using SchoolWebApp.Core.Entities.CBE.Cocurriculum;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Cocurriculum;

namespace SchoolWebApp.Core.Services.CBE.Cocurriculum
{
    public class CoCurriculumScoreService : GenericService<CoCurriculumScore>, ICoCurriculumScoreService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CoCurriculumScoreService(IUnitOfWork unitOfWork)
        : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CoCurriculumScore>> GetByScoreTypeId(int scoreTypeId)
        {
            var items = await _unitOfWork.Repository<CoCurriculumScore>()
                .Find(a => a.CoCurriculumScoreTypeId == scoreTypeId, includeProperties: "CoCurriculumScoreType");
            return items.ToList();
        }
    }
}
