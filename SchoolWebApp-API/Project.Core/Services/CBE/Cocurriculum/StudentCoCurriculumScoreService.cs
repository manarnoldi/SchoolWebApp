using SchoolWebApp.Core.Entities.CBE.Cocurriculum;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Cocurriculum;

namespace SchoolWebApp.Core.Services.CBE.Cocurriculum
{
    public class StudentCoCurriculumScoreService : GenericService<StudentCoCurriculumScore>, IStudentCoCurriculumScoreService
    {
        private readonly IUnitOfWork _unitOfWork;
        public StudentCoCurriculumScoreService(IUnitOfWork unitOfWork)
        : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<StudentCoCurriculumScore>> GetByStudentCoCurriculumActivityId(int studentCoCurriculumActivityId)
        {
            var items = await _unitOfWork.Repository<StudentCoCurriculumScore>()
                .Find(a => a.StudentCoCurriculumActivityId == studentCoCurriculumActivityId, includeProperties: "StudentCoCurriculumActivity,CoCurriculumScore");
            return items.ToList();
        }
    }
}
