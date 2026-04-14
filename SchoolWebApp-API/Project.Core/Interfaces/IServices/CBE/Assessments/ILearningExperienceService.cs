using SchoolWebApp.Core.Entities.CBE.Assessments;

namespace SchoolWebApp.Core.Interfaces.IServices.CBE.Assessments
{
    public interface ILearningExperienceService : IGenericService<LearningExperience>
    {
        Task<List<LearningExperience>> GetBySubStrandId(int subStrandId);
    }
}
