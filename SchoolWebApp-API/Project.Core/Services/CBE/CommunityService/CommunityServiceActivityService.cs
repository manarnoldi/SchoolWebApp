using SchoolWebApp.Core.Entities.CBE.CommunityService;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IServices.CBE.CommunityService;

namespace SchoolWebApp.Core.Services.CBE.CommunityService
{
    public class CommunityServiceActivityService : GenericService<CommunityServiceActivity>, ICommunityServiceActivityService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CommunityServiceActivityService(IUnitOfWork unitOfWork)
        : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    }
}
