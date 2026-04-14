using SchoolWebApp.Core.Entities.CBE.Values;

namespace SchoolWebApp.Core.Interfaces.IServices.CBE.Values
{
    public interface IStudentValueScoreService : IGenericService<StudentValueScore>
    {
        Task<List<StudentValueScore>> GetByStudentId(int studentId);
        Task<List<StudentValueScore>> GetBySessionId(int sessionId);
    }
}
