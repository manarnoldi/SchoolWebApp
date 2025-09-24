using SchoolWebApp.Core.Entities.CBE.Assessments;

namespace SchoolWebApp.Core.Interfaces.IServices.CBE.Assessments
{
    public interface IBroadOutcomeService : IGenericService<BroadOutcome>
    {
        Task<List<BroadOutcome>> GetByEducationLevelId(int eduLevelId);
    }
}
