using SchoolWebApp.Core.Entities.CBE.Assessments;

namespace SchoolWebApp.Core.Interfaces.IServices.CBE.Assessments
{
    public interface IStrandService: IGenericService<Strand>
    {
        Task<List<Strand>> GetBySubjectId(int subjectId);
    }
}
