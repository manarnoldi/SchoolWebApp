using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.Core.Interfaces.IServices;

namespace SchoolWebApp.Core.Interfaces.IServices.CBE.Assessments
{
    public interface IThemeService : IGenericService<Theme>
    {
        Task<List<Theme>> GetBySubjectId(int? subjectId, int? learningLvlId);
    }
}
