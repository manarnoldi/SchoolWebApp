﻿using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.Staff;
using SchoolWebApp.Core.Interfaces.IRepositories.Staff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolWebApp.Infrastructure.Repositories.Staff
{
    public class StaffDisciplineRepository : BaseRepository<StaffDiscipline>, IStaffDisciplineRepository
    {
        public StaffDisciplineRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<StaffDiscipline>> GetByStaffDetailsId(int staffDetailsId)
        {
            var staffDisciplines = await _dbContext.StaffDisciplines
                .Where(e => e.StaffDetailsId == staffDetailsId)
                .Include(s => s.StaffDetails)
                .Include(s => s.Outcome)
                .Include(s => s.OccurenceType)
                .ToListAsync();
            return staffDisciplines;
        }
    }
}
