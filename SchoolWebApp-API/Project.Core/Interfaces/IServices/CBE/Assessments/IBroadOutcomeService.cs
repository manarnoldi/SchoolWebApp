using SchoolWebApp.Core.Entities.CBE.Assessments;

namespace SchoolWebApp.Core.Interfaces.IServices.CBE.Assessments
{
    public interface IBroadOutcomeService : IGenericService<SubjectOutcome>
    {
        Task<List<SubjectOutcome>> GetByEducationLevelId(int eduLevelId);
        Task<List<SubjectOutcome>> GetBySubjectId(int subjectId);
        Task<List<SubjectOutcome>> GetByEducationLevelIdSubjectId(int eduLevelId, int subjectId);
    }
}
