using SchoolWebApp.Core.Entities.CBE.Cocurriculum;

namespace SchoolWebApp.Core.Interfaces.IServices.CBE.Cocurriculum
{
    public interface ICoCurriculumScoreService : IGenericService<CoCurriculumScore>
    {
        Task<List<CoCurriculumScore>> GetByScoreTypeId(int scoreTypeId);
    }
}
