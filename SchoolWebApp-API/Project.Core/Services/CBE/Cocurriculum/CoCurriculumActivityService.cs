using SchoolWebApp.Core.Entities.CBE.Cocurriculum;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Cocurriculum;

namespace SchoolWebApp.Core.Services.CBE.Cocurriculum
{
    public class CoCurriculumActivityService : GenericService<CoCurriculumActivity>, ICoCurriculumActivityService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CoCurriculumActivityService(IUnitOfWork unitOfWork)
        : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    }
}
