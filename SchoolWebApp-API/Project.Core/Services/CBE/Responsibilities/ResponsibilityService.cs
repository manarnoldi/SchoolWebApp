using SchoolWebApp.Core.Entities.CBE.Responsibilities;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Responsibilities;

namespace SchoolWebApp.Core.Services.CBE.Responsibilities
{
    public class ResponsibilityService : GenericService<Responsibility>, IResponsibilityService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ResponsibilityService(IUnitOfWork unitOfWork)
        : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    }
}
