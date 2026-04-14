using SchoolWebApp.Core.Entities.CBE.Values;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Values;

namespace SchoolWebApp.Core.Services.CBE.Values
{
    public class StudentValueScoreService : GenericService<StudentValueScore>, IStudentValueScoreService
    {
        private readonly IUnitOfWork _unitOfWork;
        public StudentValueScoreService(IUnitOfWork unitOfWork)
        : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<StudentValueScore>> GetByStudentId(int studentId)
        {
            var items = await _unitOfWork.Repository<StudentValueScore>()
                .Find(a => a.StudentId == studentId, includeProperties: "Value,ValueScore,Student,Session");
            return items.ToList();
        }

        public async Task<List<StudentValueScore>> GetBySessionId(int sessionId)
        {
            var items = await _unitOfWork.Repository<StudentValueScore>()
                .Find(a => a.SessionId == sessionId, includeProperties: "Value,ValueScore,Student,Session");
            return items.ToList();
        }
    }
}
