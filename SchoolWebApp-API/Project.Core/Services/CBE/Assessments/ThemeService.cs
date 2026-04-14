using LinqKit;
using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Assessments;
using SchoolWebApp.Core.Services;

namespace SchoolWebApp.Core.Services.CBE.Assessments
{
    public class ThemeService : GenericService<Theme>, IThemeService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ThemeService(IUnitOfWork unitOfWork)
        : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Theme>> GetBySubjectId(int? subjectId, int? learningLvlId)
        {
            var filter = PredicateBuilder.New<Theme>();
            if (subjectId != null)
                filter = filter.And(a => a.SubjectId == subjectId);
            if (learningLvlId != null)
                filter = filter.And(a => a.LearningLevelId == learningLvlId);

            var themes = await _unitOfWork.Repository<Theme>()
                .Find(subjectId == null && learningLvlId == null ? null : filter,
                includeProperties: "Subject,LearningLevel,Curriculum");
            return themes.ToList();
        }
    }
}
