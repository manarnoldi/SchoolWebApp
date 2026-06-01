using Project.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Entities.CBE.Exams;

namespace SchoolWebApp.Core.Interfaces.IRepositories.Academics
{
    public interface ISchoolExamRepository : IBaseRepository<SchoolExam>
    {
        Task<List<SchoolExam>> SearchForSchoolExams(int academicYearId, int curriculumId,
            int? sessionId, int? examTypeId);
    }
}
