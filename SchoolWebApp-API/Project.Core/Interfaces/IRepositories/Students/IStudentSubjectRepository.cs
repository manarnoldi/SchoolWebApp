using Project.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Entities.Students;

namespace SchoolWebApp.Core.Interfaces.IRepositories.Students
{
    public interface IStudentSubjectRepository : IBaseRepository<StudentSubject>
    {
        Task<List<StudentSubject>> GetByAcademicYearId(int academicYearId);
        Task<List<StudentSubject>> GetBySubjectId(int subjectId);
    }
}
