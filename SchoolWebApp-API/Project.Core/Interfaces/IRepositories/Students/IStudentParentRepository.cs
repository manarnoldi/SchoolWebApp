﻿using Project.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Entities.Students;

namespace SchoolWebApp.Core.Interfaces.IRepositories.Students
{
    public interface IStudentParentRepository : IBaseRepository<StudentParent>
    {
        Task<StudentParent> GetStudentParentByIds(int studentId, int parentId);

    }
}
