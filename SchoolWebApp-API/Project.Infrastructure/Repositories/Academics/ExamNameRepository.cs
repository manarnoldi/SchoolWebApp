using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Interfaces.IRepositories.Academics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolWebApp.Infrastructure.Repositories.Academics
{
    public class ExamNameRepository : BaseRepository<ExamName>, IExamNameRepository
    {
        public ExamNameRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<ExamName>> GetByExamTypeId(int examTypeId)
        {
            var examNames = await _dbContext.ExamNames
                .Where(e => e.ExamtypeId == examTypeId)
                .Include(e => e.ExamType)
                .ToListAsync();
            return examNames;
        }
    }
}