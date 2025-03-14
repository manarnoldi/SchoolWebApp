using Project.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Entities.Academics;

namespace SchoolWebApp.Core.Interfaces.IRepositories.Academics
{
    public interface IExamRepository : IBaseRepository<Exam>
    {
        Task<List<Exam>> GetBySessionId(int sessionId);
        Task<List<Exam>> GetBySchoolClassId(int schoolClassId);
        Task<List<Exam>> SearchForExam(int academicYearId, int curriculumId, int sessionId, int? schoolClassId, int? subjectId);
    }
}
