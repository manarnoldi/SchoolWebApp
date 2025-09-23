using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.DTOs.Reports.Academics;
using SchoolWebApp.Core.Entities.CBE.Exams;
using SchoolWebApp.Core.Interfaces.IRepositories.Academics;

namespace SchoolWebApp.Infrastructure.Repositories.Academics
{
    public class ExamResultRepository : BaseRepository<ExamResult>, IExamResultRepository
    {
        private readonly IMapper _mapper;
        public ExamResultRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext)
        {
            _mapper = mapper;
        }

        public async Task<List<ExamResult>> GetByExamId(int examId)
        {
            var examsResults = await _dbContext.ExamResults
                .Where(e => e.ExamId == examId)
                .Include(e => e.Student)
                .Include(e => e.Exam.SchoolClass)
                .Include(e => e.Exam.Subject)
                .Include(e => e.Exam.ExamType)
                .ToListAsync();
            return examsResults;
        }

        public async Task<List<ExamResult>> GetClassExamTypeRanking(int sessionId, int classId, int examTypeId)
        {           
            var rankedList = _dbContext.ExamResults.Where(er => er.Equals(classId)).ToList();

            return rankedList;
        }
        public async Task<ExamResult> GetByStudentExamId(int studentId, int examId)
        {
            var examsResults = await _dbContext.ExamResults
                .Where(e => e.StudentId == studentId && e.ExamId == examId)
                .Include(e => e.Student)
                .Include(e => e.Exam)
                .Include(e => e.Exam.Subject)
                .FirstOrDefaultAsync();
            return examsResults;
        }

        public async Task<List<ExamResult>> GetByStudentSubjectId(int studentId, int subjectId)
        {
            var examsResults = await _dbContext.ExamResults
                  .Where(e => e.StudentId == studentId && e.Exam.SubjectId == subjectId)
                .Include(e => e.Student)
                .Include(e => e.Exam.Subject)
                .ToListAsync();
            return examsResults;
        }

        public async Task<StudentPerformanceDto> GetStudentPerformace(int sessionId, int classId, int examTypeId, int studentId)
        {
            var subjectPerformanceList = await (
        from studentClass in _dbContext.StudentClasses
        where studentClass.StudentId == studentId && studentClass.SchoolClassId == classId
        join studentSubject in _dbContext.StudentSubjects
            on studentClass.Id equals studentSubject.StudentClassId

        join exam in _dbContext.Exams
            on studentSubject.SubjectId equals exam.SubjectId
        where exam.SchoolClassId == classId &&
              exam.SessionId == sessionId &&
              exam.ExamTypeId == examTypeId

        join result in _dbContext.ExamResults
            on new { ExamId = exam.Id, StudentId = studentId }
            equals new { result.ExamId, result.StudentId }
            into resultJoin
        from result in resultJoin.DefaultIfEmpty()

        select new SubjectPerformanceDto
        {
            SubjectId = studentSubject.SubjectId,
            Score = result != null ? result.Score : null,
            ExamMark = exam.ExamMark
        }
    ).ToListAsync();

            var studentPerformance = new StudentPerformanceDto
            {
                ExamTypeId = examTypeId,
                StudentId = studentId,
                SessionId = sessionId,
                ClassId = classId,
                AllocatedSubjectCount = subjectPerformanceList.Count,
                SubjectPerformance = subjectPerformanceList
            };

            return studentPerformance;
        }

    }
}
