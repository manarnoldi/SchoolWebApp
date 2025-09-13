using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.DTOs.Academics.Exam;
using SchoolWebApp.Core.DTOs.Academics.ExamResult;
using SchoolWebApp.Core.DTOs.Reports.Academics;
using SchoolWebApp.Core.DTOs.Students.StudentClass;
using SchoolWebApp.Core.DTOs.Students.StudentSubjects;
using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Interfaces.IRepositories.Academics;
using System.Linq;
using System.Linq;
using static System.Formats.Asn1.AsnWriter;

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
                .Include(e => e.Exam.ExamName)
                .ToListAsync();
            return examsResults;
        }

        public async Task<List<RankedStudentExamTypePerformanceDto>> GetClassExamTypeRanking(int sessionId, int classId, int examTypeId, int? examNameId = null)
        {
            // Step 1: Load all ExamNames under this ExamType
            var examNamesQuery = _dbContext.ExamNames.AsQueryable();

            //load all grades for later usage in the code
            var grades = await _dbContext.Grades.ToListAsync();

            if (examNameId.HasValue)
            {
                examNamesQuery = examNamesQuery.Where(en => en.Id == examNameId.Value && en.ExamtypeId == examTypeId);
            }
            else
            {
                examNamesQuery = examNamesQuery.Where(en => en.ExamtypeId == examTypeId);
            }

            var examNames = await examNamesQuery.ToListAsync();
            var examNameIds = examNames.Select(en => en.Id).ToList();

            // Step 2: Get relevant Exams from this ExamType
            var exams = await _dbContext.Exams
                .Where(e => examNameIds.Contains(e.ExamNameId) &&
                            e.SchoolClassId == classId &&
                            e.SessionId == sessionId)
                .ToListAsync();

            if (!examNameId.HasValue)
            {
                var subjectGroups = exams
                    .GroupBy(e => new { e.SubjectId, e.SessionId, e.SchoolClassId })
                    .ToList();

                foreach (var group in subjectGroups)
                {
                    var total = group.Sum(e => e.ContributingMark);
                    if (Math.Abs(total - 100) > 0.001)
                    {
                        throw new InvalidOperationException(
                            $"Total contributing marks for SubjectId {group.Key.SubjectId}, " +
                            $"SessionId {group.Key.SessionId}, ClassId {group.Key.SchoolClassId} " +
                            $"must equal 100%. Current total: {total}");
                    }
                }
            }

            var examIds = exams.Select(e => e.Id).ToList();

            // Step 3: Load Results
            var results = await _dbContext.ExamResults
                .Where(r => examIds.Contains(r.ExamId))
                .ToListAsync();

            // Step 4: Load StudentClass and Subjects
            var studentClasses = await _dbContext.StudentClasses
                .Where(sc => sc.SchoolClassId == classId)
                .ToListAsync();

            var studentSubjects = await _dbContext.StudentSubjects
                .Where(ss => studentClasses.Select(sc => sc.Id).Contains(ss.StudentClassId))
                .ToListAsync();

            var students = await _dbContext.Students
                .Where(s => studentClasses.Select(sc => sc.StudentId).Contains(s.Id))
                .ToListAsync();

            var rankedList = new List<RankedStudentExamTypePerformanceDto>();

            foreach (var sc in studentClasses)
            {
                var allocatedSubjects = studentSubjects
                    .Where(ss => ss.StudentClassId == sc.Id)
                    .Select(ss => ss.SubjectId)
                    .Distinct()
                    .ToList();

                var subjectPerformances = allocatedSubjects.Select(subjectId =>
                {
                    var subjectExams = exams.Where(e => e.SubjectId == subjectId).ToList();

                    var breakdowns = subjectExams.Select(e =>
                    {
                        var score = results.FirstOrDefault(r => r.ExamId == e.Id && r.StudentId == sc.StudentId)?.Score;
                        double? weighted = score.HasValue
                            ? (score.Value / e.ExamMark) * e.ContributingMark
                            : null;

                        return new ExamNameBreakdownDto
                        {
                            ExamNameId = e.ExamNameId,
                            RawScore = score,
                            ExamMark = e.ExamMark,
                            ContributingMark = e.ContributingMark,
                            WeightedScore = weighted,
                            PercentScore = score.HasValue ? (score.Value / e.ExamMark) * 100 : (double?)null
                        };
                    }).ToList();

                    return new SubjectPerformanceByExamTypeDto
                    {
                        SubjectId = subjectId,
                        BreakdownByExamName = breakdowns,
                        TotalRawScore = breakdowns
                            .Where(b => b.RawScore.HasValue)
                            .Sum(b => b.RawScore.Value),
                        TotalWeightedScore = breakdowns
                            .Where(b => b.WeightedScore.HasValue)
                            .Sum(b => b.WeightedScore.Value)
                    };
                }).ToList();

                
                var student = students.FirstOrDefault(s => s.Id == sc.StudentId);
                double totalWeighted = subjectPerformances.Sum(sp => sp.TotalWeightedScore);
                double meanMark = allocatedSubjects.Count > 0 ? totalWeighted / allocatedSubjects.Count : 0;
                var grade = grades.FirstOrDefault(g => meanMark >= g.MinScore && meanMark <= g.MaxScore);

                rankedList.Add(new RankedStudentExamTypePerformanceDto
                {
                    ExamTypeId = examTypeId,
                    StudentId = sc.StudentId,
                    SubjectCount = allocatedSubjects.Count,
                    ClassId = classId,
                    SessionId = sessionId,
                    StudentFullName = $"{student?.FullName}",
                    SubjectPerformance = subjectPerformances,
                    MeanMark = meanMark,
                    MeanMarkGrade = grade?.Abbr,
                    MeanMarkPoints = grade?.Points,
                    Rank = 0 // Assigned below
                });
            }

            // Step 5: Ranking
            var ranked = rankedList
                .OrderByDescending(s => s.TotalWeightedScore)
                .Select((s, index) =>
                {
                    s.Rank = index + 1;
                    return s;
                })
                .ToList();

            return ranked;
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

        public async Task<StudentPerformanceDto> GetStudentPerformace(int sessionId, int classId, int examNameId, int studentId)
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
              exam.ExamNameId == examNameId

        join result in _dbContext.ExamResults
            on new { ExamId = exam.Id, StudentId = studentId }
            equals new { result.ExamId, result.StudentId }
            into resultJoin
        from result in resultJoin.DefaultIfEmpty()

        select new SubjectPerformanceDto
        {
            SubjectId = studentSubject.SubjectId,
            Score = result != null ? result.Score : null,
            ExamMark = exam.ExamMark,
            ContributingMark = exam.ContributingMark
        }
    ).ToListAsync();

            var studentPerformance = new StudentPerformanceDto
            {
                ExamNameId = examNameId,
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
