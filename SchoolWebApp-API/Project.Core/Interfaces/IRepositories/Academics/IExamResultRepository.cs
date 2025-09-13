using Project.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.DTOs.Reports.Academics;
using SchoolWebApp.Core.Entities.Academics;

namespace SchoolWebApp.Core.Interfaces.IRepositories.Academics
{
    public interface IExamResultRepository : IBaseRepository<ExamResult>
    {
        //Task<List<ExamResult>> GetByStudentId(int studentId);
        Task<List<ExamResult>> GetByStudentSubjectId(int student, int SubjectId);
        Task<List<ExamResult>> GetByExamId(int examId);
        Task<ExamResult> GetByStudentExamId(int studentId, int examId);
        Task<List<RankedStudentExamTypePerformanceDto>> GetClassExamTypeRanking(int sessionId, int classId, int examTypeId, int? examNameId = null);
        Task<StudentPerformanceDto> GetStudentPerformace(int sessionId, int schoolClassId, int examNameId, int studentId);
    }
}
