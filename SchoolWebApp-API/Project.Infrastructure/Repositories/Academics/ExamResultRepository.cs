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
                .Include(e => e.Exam.SchoolExam.ExamType)
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

        public async Task<List<ExamResult>> GetByAllocationId(int studentSubjectId)
        {
            return await _dbContext.ExamResults
                .Where(r => r.StudentSubjectId == studentSubjectId)
                .Include(r => r.Exam).ThenInclude(e => e.SchoolExam).ThenInclude(se => se.ExamType)
                .Include(r => r.Exam).ThenInclude(e => e.Subject)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> CountByAllocationIds(List<int> studentSubjectIds)
        {
            if (studentSubjectIds == null || studentSubjectIds.Count == 0) return 0;
            return await _dbContext.ExamResults
                .CountAsync(r => studentSubjectIds.Contains(r.StudentSubjectId));
        }

        public async Task<List<ExamResult>> GetMissingMarks(int academicYearId, int curriculumId, int sessionId, int? examTypeId)
        {
            // 1. All exams in scope. Year/term/curriculum/type now live on the
            //    SchoolExam header; include it so the mapped ExamDto carries the
            //    flattened exam type.
            var exams = await _dbContext.Exams
                .Where(e => e.SchoolClass.AcademicYearId == academicYearId
                    && e.SchoolExam.Session.CurriculumId == curriculumId
                    && e.SchoolExam.SessionId == sessionId
                    && (examTypeId == null || e.SchoolExam.ExamTypeId == examTypeId))
                .Include(e => e.SchoolExam).ThenInclude(se => se.ExamType)
                .Include(e => e.SchoolClass)
                .Include(e => e.Subject)
                .AsNoTracking()
                .ToListAsync();

            if (exams.Count == 0) return new List<ExamResult>();

            var examIds = exams.Select(e => e.Id).ToList();
            var classIds = exams.Select(e => e.SchoolClassId).Distinct().ToList();
            var subjectIds = exams.Select(e => e.SubjectId).Distinct().ToList();

            // 2. (examId, studentId) pairs that already have a result.
            var existing = await _dbContext.ExamResults
                .Where(r => examIds.Contains(r.ExamId))
                .Select(r => new { r.ExamId, r.StudentId })
                .ToListAsync();
            var have = new HashSet<(int ExamId, int StudentId)>(
                existing.Select(x => (x.ExamId, x.StudentId)));

            // 3. Student allocations for the involved class+subject pairs.
            var allocations = await _dbContext.StudentSubjects
                .Where(ss => classIds.Contains(ss.StudentClass.SchoolClassId)
                    && subjectIds.Contains(ss.SubjectId))
                .Select(ss => new
                {
                    SchoolClassId = ss.StudentClass.SchoolClassId,
                    ss.SubjectId,
                    StudentId = ss.StudentClass.StudentId,
                    Student = ss.StudentClass.Student
                })
                .AsNoTracking()
                .ToListAsync();

            var allocByClassSubject = allocations
                .GroupBy(a => (a.SchoolClassId, a.SubjectId))
                .ToDictionary(g => g.Key, g => g.ToList());

            // 4. For each exam, the allocated students with no result yet.
            var missing = new List<ExamResult>();
            foreach (var exam in exams)
            {
                if (!allocByClassSubject.TryGetValue((exam.SchoolClassId, exam.SubjectId), out var allocs))
                    continue;
                foreach (var a in allocs)
                {
                    if (have.Contains((exam.Id, a.StudentId)))
                        continue;
                    // Score is left at its default - these rows represent the
                    // absence of a result; the report doesn't render a score.
                    missing.Add(new ExamResult
                    {
                        ExamId = exam.Id,
                        Exam = exam,
                        StudentId = a.StudentId,
                        Student = a.Student
                    });
                }
            }
            return missing;
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
              exam.SchoolExam.SessionId == sessionId &&
              exam.SchoolExam.ExamTypeId == examTypeId

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
