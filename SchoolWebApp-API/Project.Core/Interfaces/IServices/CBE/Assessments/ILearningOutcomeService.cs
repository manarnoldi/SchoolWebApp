using SchoolWebApp.Core.Entities.CBE.Assessments;

namespace SchoolWebApp.Core.Interfaces.IServices.CBE.Assessments
{
    public interface ILearningOutcomeService : IGenericService<LearningOutcome>
    {
        Task<List<LearningOutcome>> GetLearningOutcomesBySubStrandId(int subStrandId);
        Task<List<LearningOutcome>> GetLearningOutcomesByBroadOutcomeId(int broadOutcomeId);
        Task<List<LearningOutcome>> GetLearningOutcomesByCompetencyId(int competencyId);

    }
}
