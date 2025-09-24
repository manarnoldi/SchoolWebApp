using AutoMapper;
using Microsoft.Extensions.Logging;
using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Assessments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolWebApp.Core.Services.CBE.Assessments
{
    public class SubStrandService : GenericService<SubStrand>, ISubStrandService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<SubStrandService> _logger;

        public SubStrandService(ILogger<SubStrandService> logger, IUnitOfWork unitOfWork, IMapper mapper)
        : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
    }
}
