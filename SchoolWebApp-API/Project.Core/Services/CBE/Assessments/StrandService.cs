using LinqKit;
using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Assessments;

namespace SchoolWebApp.Core.Services.CBE.Assessments
{
    public class StrandService : GenericService<Strand>, IStrandService
    {
        private readonly IUnitOfWork _unitOfWork;
        public StrandService(IUnitOfWork unitOfWork)
        : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Strand>> GetBySubjectId(int? subjectId, int? learningLvlId, int? academicYearId)
        {
            var filter = PredicateBuilder.New<Strand>();
            if (subjectId != null)
                filter = filter.And(a => a.SubjectId == subjectId);
            if (learningLvlId != null)
                filter = filter.And(a => a.LearningLevelId == learningLvlId);
            if (academicYearId != null)
                filter = filter.And(a => a.AcademicYearId == academicYearId);

            var strands = await _unitOfWork.Repository<Strand>()
                .Find(subjectId == null && learningLvlId == null && academicYearId == null ? null : filter, 
                includeProperties: "Subject,LearningLevel,AcademicYear");
            return strands.ToList();
        }
    }
}
