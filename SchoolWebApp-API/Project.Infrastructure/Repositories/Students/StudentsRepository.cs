using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
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
            //List<StudentParentDetailsDto> parents = new List<StudentParentDetailsDto>();

            var studentParents = await _dbContext.StudentParents.Where(p => p.StudentId == studentId)
                .Include(sp=>sp.Parent)
                .Include(sp=>sp.RelationShip)
                .ToListAsync();
            //foreach (var studentParent in studentParents)
            //{
            //    var parentDetails = _dbContext.Parents.Find(studentParent.ParentId);
            //    var studentParentDto = _mapper.Map<StudentParentDetailsDto>(parentDetails);
            //    studentParentDto.RelationShipId = studentParent.RelationShipId;
            //    studentParentDto.OtherDetails = studentParent.OtherDetails;
            //    parents.Add(studentParentDto);
            //}
            return studentParents;
        }
    }
}
