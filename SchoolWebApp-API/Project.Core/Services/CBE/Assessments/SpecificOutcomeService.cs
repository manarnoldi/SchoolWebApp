using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Assessments;

namespace SchoolWebApp.Core.Services.CBE.Assessments
{
    public class SpecificOutcomeService : GenericService<SpecificOutcome>, ISpecificOutcomeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SpecificOutcomeService(IUnitOfWork unitOfWork)
        : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<SpecificOutcome>> GetByBroadOutcomeId(int broadOutcomeId)
        {
            var outcomes = await _unitOfWork.Repository<SpecificOutcome>()
                .Find(a => a.BroadOutcomeId == broadOutcomeId, includeProperties: "SubStrand,BroadOutcome,GeneralOutcome");
            return outcomes.ToList();
        }

        public async Task<List<SpecificOutcome>> GetByGeneralOutcomeId(int generalOutcomeId)
        {
            var outcomes = await _unitOfWork.Repository<SpecificOutcome>()
                .Find(a => a.GeneralOutcomeId == generalOutcomeId, includeProperties: "SubStrand,BroadOutcome,GeneralOutcome");
            return outcomes.ToList();
        }

        public async Task<List<SpecificOutcome>> GetBySubStrandId(int subStrandId)
        {
            var outcomes = await _unitOfWork.Repository<SpecificOutcome>()
                .Find(a => a.SubStrandId == subStrandId, includeProperties: "SubStrand,BroadOutcome,GeneralOutcome");
            return outcomes.ToList();
        }

        public async Task<bool> CompetencyExists(int competencyId)
        {
            var competencyExists = await _unitOfWork.Repository<Competency>()
                .ItemExistsAsync(a => a.Id == competencyId);
            return competencyExists;
        }

        public async Task<bool> SpecificOutcomeExists(int specificOutcomeId)
        {
            var specificOutcomeExists = await _unitOfWork.Repository<SpecificOutcome>()
                .ItemExistsAsync(a => a.Id == specificOutcomeId);
            return specificOutcomeExists;
        }
        public async Task<bool> SpecificOutcomeCompetencyExists(int specificOutcomeId, int competencyId)
        {
            var itemFound = await _unitOfWork.Repository<SpecificOutcome>()
                .ItemExistsAsync(i => i.Id == specificOutcomeId && i.Competencies.Any(c => c.Id == competencyId));
            return itemFound;
        }

        public async Task<SpecificOutcome> AddCompetencyToSpecificOutcome(int specificOutcomeId, int competencyId)
        {
            await ValidateSpecificOutcomeAndCompetency(specificOutcomeId, competencyId);

            var specificOutcome = await this.GetById(specificOutcomeId);
            var competency = await _unitOfWork.Repository<Competency>().GetById(competencyId);

            if (specificOutcome == null || competency == null)
                throw new InvalidOperationException("Either specific outcome or competency cannot be found in the database.");

            specificOutcome.Competencies.Add(competency);
            await _unitOfWork.SaveChangesAsync();
            return specificOutcome;
        }

        private async Task ValidateSpecificOutcomeAndCompetency(int specificOutcomeId, int competencyId)
        {
            var specificOutcomeExists = await this.SpecificOutcomeExists(specificOutcomeId);
            var competencyExists = await this.CompetencyExists(competencyId);
            var specificOutcomeCompetencyExists = await this.SpecificOutcomeCompetencyExists(specificOutcomeId, competencyId);

            if (specificOutcomeCompetencyExists)
                throw new InvalidOperationException($"The specific outcome and competency combination submitted already exists.");
            if (!specificOutcomeExists)
                throw new InvalidOperationException($"The specific outcome of Id - '{specificOutcomeId}' does not exist in specific outcomes.");
            if (!competencyExists)
                throw new InvalidOperationException($"The competency of Id - '{competencyId}' does not exist in competencies.");
        }

        public async Task RemoveCompetencyFromSpecificOutcome(int specificOutcomeId, int competencyId)
        {
            var competency = await _unitOfWork.Repository<Competency>().GetById(competencyId);
            var specificOutcomeCompetencyExists = await this.SpecificOutcomeCompetencyExists(specificOutcomeId, competencyId);

            if (!specificOutcomeCompetencyExists)
                throw new InvalidOperationException($"The specific outcome and competency combination does not exist.");

            var specificOutcome = await this.GetById(specificOutcomeId, includeProperties: "Competencies");
            if (specificOutcome == null || competency == null)
                throw new InvalidOperationException("Either specific outcome or competency cannot be found in the database.");
            specificOutcome.Competencies.Remove(competency);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<Competency>> GetCompetenciesForSpecificOutcomeId(int specificOutcomeId)
        {
            var competencies = (await _unitOfWork.Repository<SpecificOutcome>()
                .Find(so => so.Id == specificOutcomeId, includeProperties: "Competencies"))
                .SelectMany(so => so.Competencies)
                .ToList();
            return competencies;
        }
    }
}
