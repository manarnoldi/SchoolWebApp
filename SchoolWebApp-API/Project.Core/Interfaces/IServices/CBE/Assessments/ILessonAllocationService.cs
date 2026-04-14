using SchoolWebApp.Core.Entities.CBE.Assessments;

namespace SchoolWebApp.Core.Interfaces.IServices.CBE.Assessments
{
    public interface ILessonAllocationService : IGenericService<LessonAllocation>
    {
        Task<List<LessonAllocation>> GetBySubjectId(int subjectId);
        Task<List<LessonAllocation>> GetByLearningLevelId(int learningLevelId);
    }
}
