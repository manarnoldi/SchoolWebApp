using SchoolWebApp.Core.Entities.CBE.Values;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Values;

namespace SchoolWebApp.Core.Services.CBE.Values
{
    public class ValueScoreService : GenericService<ValueScore>, IValueScoreService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ValueScoreService(IUnitOfWork unitOfWork)
        : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    }
}
