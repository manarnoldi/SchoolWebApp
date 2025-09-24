using AutoMapper;
using Microsoft.Extensions.Logging;
using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Assessments;

namespace SchoolWebApp.Core.Services.CBE.Assessments
{
    public class CompetencyService : GenericService<Competency>, ICompetencyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CompetencyService> _logger;

        public CompetencyService(ILogger<CompetencyService> logger, IUnitOfWork unitOfWork, IMapper mapper)
        : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
    
    }
}
