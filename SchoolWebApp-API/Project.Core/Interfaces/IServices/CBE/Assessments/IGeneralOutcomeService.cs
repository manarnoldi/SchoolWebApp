using SchoolWebApp.Core.Entities.CBE.Assessments;

namespace SchoolWebApp.Core.Interfaces.IServices.CBE.Assessments
{
    public interface IGeneralOutcomeService : IGenericService<GeneralOutcome>
    {
        Task<List<GeneralOutcome>> GetByEducationLevelTypeId(int? eduLevelTypeId);
    }
}
