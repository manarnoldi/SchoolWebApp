using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Assessments;

namespace SchoolWebApp.Core.Services.CBE.Assessments
{
    public class StudentAssessmentService : GenericService<StudentAssessment>, IStudentAssessmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StudentAssessmentService(IUnitOfWork unitOfWork)
        : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<StudentAssessment>> GetByAssessmentTypeId(int asessmentTypeId)
        {
            var assessments = await _unitOfWork.Repository<StudentAssessment>()
                .Find(a => a.AssessmentTypeId == asessmentTypeId, includeProperties: "AssessmentType,Grade,Session,SpecificOutcome,Student");
            return (List<StudentAssessment>)assessments;
        }

        public async Task<List<StudentAssessment>> GetByGradeId(int gradeId)
        {
            var assessments = await _unitOfWork.Repository<StudentAssessment>()
                .Find(a => a.GradeId == gradeId, includeProperties: "AssessmentType,Grade,Session,SpecificOutcome,Student");
            return (List<StudentAssessment>)assessments;
        }

        public async Task<List<StudentAssessment>> GetBySessionId(int sessionId)
        {
            var assessments = await _unitOfWork.Repository<StudentAssessment>()
                .Find(a => a.SessionId == sessionId, includeProperties: "AssessmentType,Grade,Session,SpecificOutcome,Student");
            return (List<StudentAssessment>)assessments;
        }

        public async Task<List<StudentAssessment>> GetBySpecificOutcomeId(int specificOutcomeId)
        {
            var assessments = await _unitOfWork.Repository<StudentAssessment>()
                .Find(a => a.SpecificOutcomeId == specificOutcomeId, includeProperties: "AssessmentType,Grade,Session,SpecificOutcome,Student");
            return (List<StudentAssessment>)assessments;
        }

        public async Task<List<StudentAssessment>> GetByStudentId(int studentId)
        {
            var assessments = await _unitOfWork.Repository<StudentAssessment>()
                .Find(a => a.StudentId == studentId, includeProperties: "AssessmentType,Grade,Session,SpecificOutcome,Student");
            return (List<StudentAssessment>)assessments;
        }

        public async Task<List<StudentAssessment>> GetBySchoolClassId(int schoolClassId)
        {
            var assessments = await _unitOfWork.Repository<StudentAssessment>()
                .Find(a => a.SchoolClassId == schoolClassId, includeProperties: "AssessmentType,Grade,Session,SpecificOutcome,Student");
            return (List<StudentAssessment>)assessments;
        }
    }
}
