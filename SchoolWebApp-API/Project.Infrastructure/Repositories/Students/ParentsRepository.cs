using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.DTOs.Students.StudentParent;
using SchoolWebApp.Core.Entities.Students;
using SchoolWebApp.Core.Interfaces.IRepositories.Students;

namespace SchoolWebApp.Infrastructure.Repositories.Students
{
    public class ParentsRepository : BaseRepository<Parent>, IParentsRepository
    {
        public ParentsRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<Student>> GetStudentsByParentId(int parentId)
        {
            var students = new List<Student>();
            var parentStudents = await _dbContext.StudentParents.Where(p => p.ParentId == parentId).ToListAsync();

            foreach (var parentStudent in parentStudents)
            {
                var stud = _dbContext.Students.Find(parentStudent.StudentId);
                students.Add(stud);
            }
            return students;
        }
    }
}
