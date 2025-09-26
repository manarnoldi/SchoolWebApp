using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Assessments;

namespace SchoolWebApp.Core.Services.CBE.Assessments
{
    public class AssessmentTypeService : GenericService<AssessmentType>, IAssessmentTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        public AssessmentTypeService( IUnitOfWork unitOfWork)
        : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }    
    }
}
