using Project.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Entities.Students;

namespace SchoolWebApp.Core.Interfaces.IRepositories.Students
{
    public interface IStudentSubjectRepository : IBaseRepository<StudentSubject>
    {
        Task<List<StudentSubject>> GetByStudentClassId(int studentClassId);
        Task<List<StudentSubject>> GetBySchoolClassId(int schoolClassId, int studentId);
        Task<List<StudentSubject>> GetBySubjectId(int subjectId);
        Task<List<StudentSubject>> GetByStudentId(int studentId);
        Task<StudentSubject> GetByStudentClassSubjectId(int studentClassId, int subjectId);
    }
}
