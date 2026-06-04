using Project.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.DTOs.Reports.Academics;
using SchoolWebApp.Core.Entities.CBE.Exams;

namespace SchoolWebApp.Core.Interfaces.IRepositories.Academics
{
    public interface IExamResultRepository : IBaseRepository<ExamResult>
    {
        //Task<List<ExamResult>> GetByStudentId(int studentId);
        Task<List<ExamResult>> GetByStudentSubjectId(int student, int SubjectId);
        Task<List<ExamResult>> GetByExamId(int examId);
        Task<ExamResult> GetByStudentExamId(int studentId, int examId);
        Task<List<ExamResult>> GetClassExamTypeRanking(int sessionId, int classId, int examTypeId);
        Task<StudentPerformanceDto> GetStudentPerformace(int sessionId, int schoolClassId, int examNameId, int studentId);

        // One-shot missing-marks query: for every exam matching the selection,
        // the allocated students who have no result yet. Replaces the client's
        // per-exam request fan-out.
        Task<List<ExamResult>> GetMissingMarks(int academicYearId, int curriculumId, int sessionId, int? examTypeId);

        // Results attached to a single subject allocation - shown in the
        // deallocation confirmation (they cascade-delete with the allocation).
        Task<List<ExamResult>> GetByAllocationId(int studentSubjectId);

        // Total results attached to a set of allocations (bulk deallocation).
        Task<int> CountByAllocationIds(List<int> studentSubjectIds);
    }
}
