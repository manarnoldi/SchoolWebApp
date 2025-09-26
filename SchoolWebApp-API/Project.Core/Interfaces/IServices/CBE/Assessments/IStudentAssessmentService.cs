using SchoolWebApp.Core.Entities.CBE.Assessments;

namespace SchoolWebApp.Core.Interfaces.IServices.CBE.Assessments
{
    public interface IStudentAssessmentService : IGenericService<StudentAssessment>
    {
        Task<List<StudentAssessment>> GetByStudentId(int studentId);
        Task<List<StudentAssessment>> GetBySessionIdAndParams(int sessionId, int? studentId, int? schoolClassId, int? assessmentTypeid,
            int? specificOutcomeId);
        Task<List<StudentAssessment>> GetByAssessmentTypeId(int asessmentTypeId);
        Task<List<StudentAssessment>> GetBySpecificOutcomeId(int specificOutcomeId);
        Task<List<StudentAssessment>> GetByGradeId(int gradeId);
        Task<List<StudentAssessment>> GetBySchoolClassId(int schoolClassId);
    }
}
