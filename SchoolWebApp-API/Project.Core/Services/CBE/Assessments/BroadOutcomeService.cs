using AutoMapper;
using Microsoft.Extensions.Logging;
using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Assessments;

namespace SchoolWebApp.Core.Services.CBE.Assessments
{
    public class BroadOutcomeService : GenericService<BroadOutcome>, IBroadOutcomeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<BroadOutcomeService> _logger;

        public BroadOutcomeService(ILogger<BroadOutcomeService> logger, IUnitOfWork unitOfWork, IMapper mapper)
        : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<BroadOutcome>> GetByEducationLevelId(int eduLevelId)
        {
            var broadOutcomes = await _unitOfWork.Repository<BroadOutcome>().Find(a => a.EducationLevelId == eduLevelId);
            return broadOutcomes.ToList();
        }
    }
}
