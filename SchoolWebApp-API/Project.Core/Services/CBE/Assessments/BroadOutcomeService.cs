using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Assessments;

namespace SchoolWebApp.Core.Services.CBE.Assessments
{
    public class BroadOutcomeService : GenericService<SubjectOutcome>, IBroadOutcomeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BroadOutcomeService(IUnitOfWork unitOfWork)
        : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<SubjectOutcome>> GetByEducationLevelId(int eduLevelId)
        {
            var broadOutcomes = await _unitOfWork.Repository<SubjectOutcome>()
                .Find(a => a.EducationLevelId == eduLevelId, includeProperties: "EducationLevel,Subject");
            return broadOutcomes.ToList();
        }

        public async Task<List<SubjectOutcome>> GetByEducationLevelIdSubjectId(int eduLevelId, int subjectId)
        {
            var broadOutcomes = await _unitOfWork.Repository<SubjectOutcome>()
                .Find(a => a.EducationLevelId == eduLevelId && a.SubjectId == subjectId,
                includeProperties: "EducationLevel,Subject");
            return broadOutcomes.ToList();
        }

        public async Task<List<SubjectOutcome>> GetBySubjectId(int subjectId)
        {
            var broadOutcomes = await _unitOfWork.Repository<SubjectOutcome>()
                .Find(a => a.SubjectId == subjectId, includeProperties: "EducationLevel,Subject");
            return broadOutcomes.ToList();
        }
    }
}
