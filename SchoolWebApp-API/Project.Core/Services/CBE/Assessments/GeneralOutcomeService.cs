using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Assessments;

namespace SchoolWebApp.Core.Services.CBE.Assessments
{
    public class GeneralOutcomeService : GenericService<GeneralOutcome>, IGeneralOutcomeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public GeneralOutcomeService(IUnitOfWork unitOfWork)
        : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<GeneralOutcome>> GetByEducationLevelTypeId(int eduLevelTypeId)
        {
            var outcomes = await _unitOfWork.Repository<GeneralOutcome>()
                .Find(a => a.EducationLevelTypeId == eduLevelTypeId, includeProperties: "EducationLevelType");
            return outcomes.ToList();
        }
    }
}
