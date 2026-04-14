using SchoolWebApp.Core.Entities.CBE.Cocurriculum;

namespace SchoolWebApp.Core.Interfaces.IServices.CBE.Cocurriculum
{
    public interface IStudentCoCurriculumScoreService : IGenericService<StudentCoCurriculumScore>
    {
        Task<List<StudentCoCurriculumScore>> GetByStudentCoCurriculumActivityId(int studentCoCurriculumActivityId);
    }
}
