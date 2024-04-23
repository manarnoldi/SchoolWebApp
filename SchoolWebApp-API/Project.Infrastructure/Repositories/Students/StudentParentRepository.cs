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

        public async Task<StudentParent> GetStudentParentByParentIdStudentId(int parentId, int studentId)
        {
            var studentParent = await _dbContext.StudentParents.Where(s => s.ParentId == parentId && s.StudentId == studentId).FirstOrDefaultAsync();
            return studentParent;
        }
    }
}
