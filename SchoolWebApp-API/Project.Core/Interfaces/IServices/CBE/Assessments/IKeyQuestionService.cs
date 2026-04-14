using SchoolWebApp.Core.Entities.CBE.Assessments;

namespace SchoolWebApp.Core.Interfaces.IServices.CBE.Assessments
{
    public interface IKeyQuestionService : IGenericService<KeyQuestion>
    {
        Task<List<KeyQuestion>> GetBySubStrandId(int subStrandId);
    }
}
