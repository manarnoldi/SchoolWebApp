using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.Students;
using SchoolWebApp.Core.Interfaces.IRepositories.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolWebApp.Infrastructure.Repositories.Students
{
    public class StudentParentRepository : BaseRepository<StudentParent>, IStudentParentRepository
    {
        public StudentParentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<StudentParent>> GetParentsByStudentId(int studentId)
        {
            var parents = await _dbContext.StudentParents.Where(e => e.StudentId == studentId).Include(P => P.Parent).ToListAsync();
            return parents;
        }

        public async Task<List<StudentParent>> GetStudentsByParentId(int parentId)
        {
            var students = await _dbContext.StudentParents.Where(e => e.ParentId == parentId).Include(P => P.Student).ToListAsync();
            return students;
        }
    }
}
