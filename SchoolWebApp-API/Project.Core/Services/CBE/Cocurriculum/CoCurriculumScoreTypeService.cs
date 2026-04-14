using SchoolWebApp.Core.Entities.CBE.Cocurriculum;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Cocurriculum;

namespace SchoolWebApp.Core.Services.CBE.Cocurriculum
{
    public class CoCurriculumScoreTypeService : GenericService<CoCurriculumScoreType>, ICoCurriculumScoreTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CoCurriculumScoreTypeService(IUnitOfWork unitOfWork)
        : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    }
}
