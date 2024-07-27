using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.Students;
using SchoolWebApp.Core.Interfaces.IRepositories.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SchoolWebApp.Infrastructure.Repositories.Students
{
    public class StudentParentRepository : BaseRepository<StudentParent>, IStudentParentRepository
    {
        public StudentParentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<StudentParent> GetStudentParentByIds(int studentId, int parentId)
        {
            var studentParent = await _dbContext.StudentParents
                .FindAsync(studentId, parentId);
            return studentParent;
        }

    }
}
