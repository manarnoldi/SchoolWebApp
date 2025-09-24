using Microsoft.Extensions.Logging;
using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Assessments;

namespace SchoolWebApp.Core.Services.CBE.Assessments
{
    public class AssessmentService : GenericService<Assessment>, IAssessmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        
        private readonly ILogger<AssessmentService> _logger;

        public AssessmentService(ILogger<AssessmentService> logger, IUnitOfWork unitOfWork)
        : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<Assessment>> GetAssessmentsByStudentId(int studentId)
        {
            var assessments = await _unitOfWork.Repository<Assessment>().Find(a => a.StudentId == studentId);            
            return (List<Assessment>)assessments;
        }
    }
}
