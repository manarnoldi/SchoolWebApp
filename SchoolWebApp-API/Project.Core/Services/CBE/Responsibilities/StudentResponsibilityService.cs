using SchoolWebApp.Core.Entities.CBE.Responsibilities;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Responsibilities;

namespace SchoolWebApp.Core.Services.CBE.Responsibilities
{
    public class StudentResponsibilityService : GenericService<StudentResponsibility>, IStudentResponsibilityService
    {
        private readonly IUnitOfWork _unitOfWork;
        public StudentResponsibilityService(IUnitOfWork unitOfWork)
        : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<StudentResponsibility>> GetByStudentId(int studentId)
        {
            var items = await _unitOfWork.Repository<StudentResponsibility>()
                .Find(a => a.StudentId == studentId, includeProperties: "Student,AcademicYear,ResponsibilitySocialSkill");
            return items.ToList();
        }

        public async Task<List<StudentResponsibility>> GetByAcademicYearId(int academicYearId)
        {
            var items = await _unitOfWork.Repository<StudentResponsibility>()
                .Find(a => a.AcademicYearId == academicYearId, includeProperties: "Student,AcademicYear,ResponsibilitySocialSkill");
            return items.ToList();
        }
    }
}
