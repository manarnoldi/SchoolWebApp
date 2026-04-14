using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Assessments;

namespace SchoolWebApp.Core.Services.CBE.Assessments
{
    public class LessonAllocationService : GenericService<LessonAllocation>, ILessonAllocationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LessonAllocationService(IUnitOfWork unitOfWork)
        : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<LessonAllocation>> GetBySubjectId(int subjectId)
        {
            var items = await _unitOfWork.Repository<LessonAllocation>()
                .Find(a => a.SubjectId == subjectId, includeProperties: "Subject,LearningLevel");
            return items.ToList();
        }

        public async Task<List<LessonAllocation>> GetByLearningLevelId(int learningLevelId)
        {
            var items = await _unitOfWork.Repository<LessonAllocation>()
                .Find(a => a.LearningLevelId == learningLevelId, includeProperties: "Subject,LearningLevel");
            return items.ToList();
        }
    }
}
