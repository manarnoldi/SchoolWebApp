using SchoolWebApp.Core.Entities.CBE.Cocurriculum;

namespace SchoolWebApp.Core.Interfaces.IServices.CBE.Cocurriculum
{
    public interface IStudentCoCurriculumActivityService : IGenericService<StudentCoCurriculumActivity>
    {
        Task<List<StudentCoCurriculumActivity>> GetByStudentId(int studentId);
    }
}
