﻿using Project.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Entities.Enums;
using SchoolWebApp.Core.Entities.Staff;

namespace SchoolWebApp.Core.Interfaces.IRepositories.Staff
{
    public interface IStaffDetailsRepository : IBaseRepository<StaffDetails>
    {
        Task<List<StaffDetails>> GetByStaffCategoryId(int staffCategoryId);
        Task<List<StaffDetails>> GetByEmploymentTypeId(int employmentTypeId);
        Task<List<StaffDetails>> SearchForStaff(int? staffCategoryId, int? employmentTypeId, Status? active);
    }
}
