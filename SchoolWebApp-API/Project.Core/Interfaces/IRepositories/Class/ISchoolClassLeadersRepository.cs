﻿using Project.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Entities.Class;

namespace SchoolWebApp.Core.Interfaces.IRepositories.Class
{
    public interface ISchoolClassLeadersRepository : IBaseRepository<SchoolClassLeaders>
    {
        Task<List<SchoolClassLeaders>> GetBySchoolClassId(int schoolClassId);
    }
}

