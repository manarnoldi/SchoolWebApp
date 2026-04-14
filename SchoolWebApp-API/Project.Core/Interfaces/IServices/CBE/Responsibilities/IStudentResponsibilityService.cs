using SchoolWebApp.Core.Entities.CBE.Responsibilities;

namespace SchoolWebApp.Core.Interfaces.IServices.CBE.Responsibilities
{
    public interface IStudentResponsibilityService : IGenericService<StudentResponsibility>
    {
        Task<List<StudentResponsibility>> GetByStudentId(int studentId);
        Task<List<StudentResponsibility>> GetByAcademicYearId(int academicYearId);
    }
}
