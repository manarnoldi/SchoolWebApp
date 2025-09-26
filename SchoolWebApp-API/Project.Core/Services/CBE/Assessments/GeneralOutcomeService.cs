using LinqKit;
using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.Students;
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

        public async Task<List<GeneralOutcome>> GetByEducationLevelTypeId(int? eduLevelTypeId)
        {
            var filter = PredicateBuilder.New<GeneralOutcome>();
            if (eduLevelTypeId != null)
                filter = filter.And(a => a.EducationLevelTypeId == eduLevelTypeId);

            var outcomes = await _unitOfWork.Repository<GeneralOutcome>()
                .Find(eduLevelTypeId == null ? null : filter, includeProperties: "EducationLevelType");
            return outcomes.ToList();
        }
    }
}
