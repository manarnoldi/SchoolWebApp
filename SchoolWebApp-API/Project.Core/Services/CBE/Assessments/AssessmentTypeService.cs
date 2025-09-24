using AutoMapper;
using Microsoft.Extensions.Logging;
using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Assessments;

namespace SchoolWebApp.Core.Services.CBE.Assessments
{
    public class AssessmentTypeService : GenericService<AssessmentType>, IAssessmentTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AssessmentTypeService> _logger;

        public AssessmentTypeService(ILogger<AssessmentTypeService> logger, IUnitOfWork unitOfWork, IMapper mapper)
        : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
    
    }
}
