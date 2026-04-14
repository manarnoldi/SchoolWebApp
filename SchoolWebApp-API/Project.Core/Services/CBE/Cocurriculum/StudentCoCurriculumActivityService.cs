using SchoolWebApp.Core.Entities.CBE.Cocurriculum;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Cocurriculum;

namespace SchoolWebApp.Core.Services.CBE.Cocurriculum
{
    public class StudentCoCurriculumActivityService : GenericService<StudentCoCurriculumActivity>, IStudentCoCurriculumActivityService
    {
        private readonly IUnitOfWork _unitOfWork;
        public StudentCoCurriculumActivityService(IUnitOfWork unitOfWork)
        : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<StudentCoCurriculumActivity>> GetByStudentId(int studentId)
        {
            var items = await _unitOfWork.Repository<StudentCoCurriculumActivity>()
                .Find(a => a.StudentId == studentId, includeProperties: "Student,CoCurriculumActivity");
            return items.ToList();
        }
    }
}
