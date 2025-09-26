using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.Entities.CBE.Assessments;

namespace SchoolWebApp.Core.Interfaces.IServices.CBE.Assessments
{
    public interface ISpecificOutcomeService : IGenericService<SpecificOutcome>
    {
        Task<List<SpecificOutcome>> GetBySubStrandId(int subStrandId, int? learningLevelId);
        Task<List<SpecificOutcome>> GetByBroadOutcomeId(int broadOutcomeId, int? learningLevelId);
        Task<List<SpecificOutcome>> GetByGeneralOutcomeId(int generalOutcomeId, int? learningLevelId);
        Task<List<SpecificOutcome>> GetByLearningLevelId(int learningLevelId);
        Task<bool> CompetencyExists(int competencyId);
        Task<bool> SpecificOutcomeExists(int specificOutcomeId);
        Task<bool> SpecificOutcomeCompetencyExists(int specificOutcomeId, int competencyId);
        Task<SpecificOutcome> AddCompetencyToSpecificOutcome(int specificOutcomeId, int competencyId);
        Task RemoveCompetencyFromSpecificOutcome(int specificOutcomeId, int competencyId);
        Task<List<Competency>> GetCompetenciesForSpecificOutcomeId(int specificOutcomeId);        
    }
}
