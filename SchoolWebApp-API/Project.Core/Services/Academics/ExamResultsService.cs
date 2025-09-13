using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolWebApp.Core.DTOs.Reports.Academics;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IServices;
using SchoolWebApp.Core.Interfaces.IServices.Academics;
using SchoolWebApp.Core.Services.Students;

namespace SchoolWebApp.Core.Services.Academics
{
    public class ExamResultsService : IExamResultsService
    {
        private readonly ILogger<IExamResultsService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ExamResultsService(ILogger<IExamResultsService> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<List<StudentPerformanceRowDto>> GetBroadSheet(int sessionId, int schoolClassId, int examNameId)
        {
            throw new NotImplementedException();
        }
    }
}
