using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.Settings;
using SchoolWebApp.Core.Interfaces.IRepositories.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolWebApp.Infrastructure.Repositories.Settings
{
    public class DesignationsRepository : BaseRepository<Designation>, IDesignationsRepository
    {
        public DesignationsRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
