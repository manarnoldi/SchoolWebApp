using AutoMapper;
using Microsoft.Extensions.Logging;
using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Assessments;

namespace SchoolWebApp.Core.Services.CBE.Assessments
{
    public class LearningOutcomeService : GenericService<LearningOutcome>, ILearningOutcomeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<LearningOutcomeService> _logger;

        public LearningOutcomeService(ILogger<LearningOutcomeService> logger, IUnitOfWork unitOfWork, IMapper mapper)
        : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<LearningOutcome>> GetLearningOutcomesByBroadOutcomeId(int broadOutcomeId)
        {
            var outcomes = await _unitOfWork.Repository<LearningOutcome>()
                .Find(a => a.BroadOutcomeId == broadOutcomeId, includeProperties: "SubStrand,BroadOutcome,Competency");
            return outcomes.ToList();
        }

        public async Task<List<LearningOutcome>> GetLearningOutcomesByCompetencyId(int competencyId)
        {
            var outcomes = await _unitOfWork.Repository<LearningOutcome>()
                .Find(a => a.CompetencyId == competencyId, includeProperties: "SubStrand,BroadOutcome,Competency");
            return outcomes.ToList();
        }

        public async Task<List<LearningOutcome>> GetLearningOutcomesBySubStrandId(int subStrandId)
        {
            var outcomes = await _unitOfWork.Repository<LearningOutcome>()
                .Find(a => a.SubStrandId == subStrandId, includeProperties: "SubStrand,BroadOutcome,Competency");
            return outcomes.ToList();
        }
    }
}
