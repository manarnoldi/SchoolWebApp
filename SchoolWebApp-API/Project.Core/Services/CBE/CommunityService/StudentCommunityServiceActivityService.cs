using SchoolWebApp.Core.Entities.CBE.CommunityService;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IServices.CBE.CommunityService;

namespace SchoolWebApp.Core.Services.CBE.CommunityService
{
    public class StudentCommunityServiceActivityService : GenericService<StudentCommunityServiceActivity>, IStudentCommunityServiceActivityService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StudentCommunityServiceActivityService(IUnitOfWork unitOfWork)
        : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<StudentCommunityServiceActivity>> GetByStudentId(int studentId)
        {
            var items = await _unitOfWork.Repository<StudentCommunityServiceActivity>()
                .Find(a => a.StudentId == studentId, includeProperties: "Student,CommunityServiceActivity,Session,AcademicYear");
            return items.ToList();
        }

        public async Task<List<StudentCommunityServiceActivity>> GetByActivityId(int activityId)
        {
            var items = await _unitOfWork.Repository<StudentCommunityServiceActivity>()
                .Find(a => a.CommunityServiceActivityId == activityId, includeProperties: "Student,CommunityServiceActivity,Session,AcademicYear");
            return items.ToList();
        }
    }
}
