using SchoolWebApp.Core.Entities.CBE.Assessments;

namespace SchoolWebApp.Core.Interfaces.IServices.CBE.Assessments
{
    public interface ICompetencyService : IGenericService<Competency>
    {
        Task<List<SpecificOutcome>> GetSpecificOutcomesForCompetencyId(int competencyId);
    }
}
