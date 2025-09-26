using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Assessments;

namespace SchoolWebApp.Core.Services.CBE.Assessments
{
    public class CompetencyService : GenericService<Competency>, ICompetencyService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompetencyService(IUnitOfWork unitOfWork)
        : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<SpecificOutcome>> GetSpecificOutcomesForCompetencyId(int competencyId)
        {
            var specificOutcomes = (await _unitOfWork.Repository<Competency>()
                .Find(so => so.Id == competencyId, includeProperties: "SpecificOutcomes"))
                .SelectMany(so => so.SpecificOutcomes)
                .ToList();
            return specificOutcomes;
        }

    }
}
