using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Assessments;

namespace SchoolWebApp.Core.Services.CBE.Assessments
{
    public class BroadOutcomeService : GenericService<BroadOutcome>, IBroadOutcomeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BroadOutcomeService(IUnitOfWork unitOfWork)
        : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<BroadOutcome>> GetByEducationLevelId(int eduLevelId)
        {
            var broadOutcomes = await _unitOfWork.Repository<BroadOutcome>()
                .Find(a => a.EducationLevelId == eduLevelId, includeProperties: "EducationLevel,Subject");
            return broadOutcomes.ToList();
        }

        public async Task<List<BroadOutcome>> GetByEducationLevelIdSubjectId(int eduLevelId, int subjectId)
        {
            var broadOutcomes = await _unitOfWork.Repository<BroadOutcome>()
                .Find(a => a.EducationLevelId == eduLevelId && a.SubjectId == subjectId,
                includeProperties: "EducationLevel,Subject");
            return broadOutcomes.ToList();
        }

        public async Task<List<BroadOutcome>> GetBySubjectId(int subjectId)
        {
            var broadOutcomes = await _unitOfWork.Repository<BroadOutcome>()
                .Find(a => a.SubjectId == subjectId, includeProperties: "EducationLevel,Subject");
            return broadOutcomes.ToList();
        }
    }
}
