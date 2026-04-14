using SchoolWebApp.Core.Entities.CBE.Responsibilities;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Responsibilities;

namespace SchoolWebApp.Core.Services.CBE.Responsibilities
{
    public class SocialSkillService : GenericService<SocialSkill>, ISocialSkillService
    {
        private readonly IUnitOfWork _unitOfWork;
        public SocialSkillService(IUnitOfWork unitOfWork)
        : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    }
}
