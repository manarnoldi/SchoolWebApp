using AutoMapper;
using Microsoft.Extensions.Logging;
using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Assessments;

namespace SchoolWebApp.Core.Services.CBE.Assessments
{
    public class StrandService : GenericService<Strand>, IStrandService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<StrandService> _logger;

        public StrandService(ILogger<StrandService> logger, IUnitOfWork unitOfWork, IMapper mapper)
        : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<Strand>> GetStrandsByEducationLevelSubject(int eduLevelSubjectId)
        {
            var strands = await _unitOfWork.Repository<Strand>().Find(a => a.EducationLevelSubjectId == eduLevelSubjectId);
            return strands.ToList();
        }

    }
}
