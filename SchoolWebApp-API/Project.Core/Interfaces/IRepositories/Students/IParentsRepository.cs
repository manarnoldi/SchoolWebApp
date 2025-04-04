﻿using Project.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Entities.Students;

namespace SchoolWebApp.Core.Interfaces.IRepositories.Students
{
    public interface IParentsRepository : IBaseRepository<Parent>
    {
        Task<List<Student>> GetStudentsByParentId(int parentId);
    }
}
