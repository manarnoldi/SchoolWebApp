using SchoolWebApp.Core.DTOs.CBE.Assessments.Assessment;
using SchoolWebApp.Core.Entities.CBE.Assessments;

namespace SchoolWebApp.Core.Interfaces.IServices.CBE.Assessments
{
    public interface IAssessmentService : IGenericService<Assessment>
    {
        Task<List<Assessment>> GetAssessmentsByStudentId(int studentId);
    }
}
