﻿using Project.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Entities.Staff;

namespace SchoolWebApp.Core.Interfaces.IRepositories.Staff
{
    public interface IStaffDisciplineRepository : IBaseRepository<StaffDiscipline>
    {
        Task<List<StaffDiscipline>> GetByStaffDetailsId(int staffDetailsId);
        Task<List<StaffDiscipline>> GetByStaffDateFromandDateTo(int staffId, DateOnly dateFrom, DateOnly dateTo);
    }
}
