using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.Enums;
using SchoolWebApp.Core.Entities.Staff;
using SchoolWebApp.Core.Entities.Students;
using SchoolWebApp.Core.Interfaces.IRepositories.Students;

namespace SchoolWebApp.Infrastructure.Repositories.Students
{
    public class StudentsRepository : BaseRepository<Student>, IStudentsRepository
    {
        private readonly IMapper _mapper;
        public StudentsRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext)
        {
            _mapper = mapper;
        }

        public async Task<List<Student>> GetByLearningModeId(int learningModeId)
        {
            var students = await _dbContext.Students.Where(e => e.LearningModeId == learningModeId).ToListAsync();
            return students;
        }

        public async Task<List<StudentParent>> GetParentsByStudentId(int studentId)
        {
            var studentParents = await _dbContext.StudentParents.Where(p => p.StudentId == studentId)
                .Include(sp => sp.Parent)
                .Include(sp => sp.RelationShip)
                .ToListAsync();
            return studentParents;
        }

        public async Task<List<Student>> SearchForStudent(Status? active)
        {
            var query = _dbContext.Students
                .Include(e => e.LearningMode)
                .Include(e => e.Nationality)
                .Include(e => e.Religion)
                .Include(e => e.Gender)
                .AsQueryable();

            if (active != null)
                query = query.Where(s => s.Status == active);

            var students = await query.ToListAsync();
            return students;
        }
    }
}
