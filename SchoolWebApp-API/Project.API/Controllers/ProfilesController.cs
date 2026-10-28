using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.API.Licensing;
using SchoolWebApp.Core.DTOs.Payroll;
using SchoolWebApp.Core.Entities.Elections;
using SchoolWebApp.Core.Entities.Payroll;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.API.Controllers
{
    /// <summary>
    /// The extra tabs on a student's and a staff member's page, drawn from the
    /// modules the school has: exam results, leadership and activities (with
    /// elections), duties, and payslips.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/profiles")]
    public class ProfilesController : ControllerBase
    {
        // Payroll officers (who may open anyone's payslips) hold this menu.
        private const string PayrollMenu = "/payroll/periods";

        private readonly ApplicationDbContext _db;
        private readonly LicenceService _licence;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProfilesController(ApplicationDbContext db, LicenceService licence, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _db = db;
            _licence = licence;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        private Task<bool> Has(string module) => _licence.HasModuleAsync(module, Request.Host.Host);

        // GET api/profiles/students/12/academics - every exam the student sat, newest term first and within a
        // term in exam-type order, with each subject's mark (the page filters by year and term)
        [HttpGet("students/{id:int}/academics")]
        [RequiresModule(AppModules.Academics)]
        public async Task<IActionResult> StudentAcademics(int id)
        {
            var rows = await _db.ExamResults.AsNoTracking()
                .Where(r => r.StudentId == id)
                .Select(r => new
                {
                    r.Exam!.SchoolExamId,
                    ExamName = r.Exam.SchoolExam!.ExamType!.Name,
                    ExamTypeRank = r.Exam.SchoolExam.ExamType.Rank,
                    r.Exam.SchoolExam.SessionId,
                    SessionStart = r.Exam.SchoolExam.Session!.StartDate,
                    r.Exam.SchoolExam.Session.AcademicYearId,
                    r.Exam.SchoolExam.Session.CurriculumId,
                    r.Exam.SchoolExam.Description,
                    Session = r.Exam.SchoolExam.Session.SessionName,
                    Year = r.Exam.SchoolExam.Session.AcademicYear!.Name,
                    r.Exam.SchoolExam.ExamStartDate,
                    r.Exam.SchoolExam.IsReleased,
                    r.Exam.SchoolClassId,
                    ClassName = r.Exam.SchoolClass!.Name,
                    Subject = r.Exam.Subject!.Name,
                    SubjectRank = r.Exam.Subject.Rank,
                    r.Exam.ExamMark,
                    r.Score
                })
                .ToListAsync();

            // Each exam's class mean (the mean of every classmate's subject average), to compare against.
            var examClass = rows.Select(r => new { r.SchoolExamId, r.SchoolClassId }).Distinct().ToList();
            var schoolExamIds = examClass.Select(e => e.SchoolExamId).ToList();
            var classIds = examClass.Select(e => e.SchoolClassId).Distinct().ToList();
            var classRows = await _db.ExamResults.AsNoTracking()
                .Where(r => schoolExamIds.Contains(r.Exam!.SchoolExamId) && classIds.Contains(r.Exam.SchoolClassId))
                .Select(r => new { r.Exam!.SchoolExamId, r.Exam.SchoolClassId, r.StudentId, r.Score, r.Exam.ExamMark })
                .ToListAsync();
            var classMeans = classRows
                .Where(r => examClass.Any(e => e.SchoolExamId == r.SchoolExamId && e.SchoolClassId == r.SchoolClassId))
                .GroupBy(r => r.SchoolExamId)
                .ToDictionary(g => g.Key, g => Math.Round(g.GroupBy(r => r.StudentId)
                    .Average(st => st.Average(r => r.Score / (r.ExamMark > 0 ? r.ExamMark : 100) * 100.0)), 1));

            var exams = rows.GroupBy(r => r.SchoolExamId).Select(g =>
            {
                var f = g.First();
                var subjects = g.Select(r =>
                {
                    var outOf = r.ExamMark > 0 ? r.ExamMark : 100;
                    return new { r.Subject, r.SubjectRank, Score = r.Score, OutOf = outOf, Percent = Math.Round(r.Score / outOf * 100.0, 1) };
                }).OrderBy(s => s.SubjectRank).ThenBy(s => s.Subject).ToList();
                return new
                {
                    SchoolExamId = g.Key,
                    Name = string.IsNullOrWhiteSpace(f.Description) ? f.ExamName : $"{f.ExamName} - {f.Description}",
                    f.SessionId,
                    f.Session,
                    f.AcademicYearId,
                    f.Year,
                    f.SessionStart,
                    f.ExamTypeRank,
                    StartDate = f.ExamStartDate,
                    Released = f.IsReleased,
                    f.SchoolClassId,
                    f.CurriculumId,
                    f.ClassName,
                    Mean = subjects.Count == 0 ? 0 : Math.Round(subjects.Average(s => s.Percent), 1),
                    ClassMean = classMeans.TryGetValue(g.Key, out var cm) ? cm : (double?)null,
                    Subjects = subjects
                };
            }).OrderByDescending(e => e.SessionStart).ThenBy(e => e.ExamTypeRank).ThenBy(e => e.StartDate).ToList();
            return Ok(exams);
        }

        // GET api/profiles/students/12/activities - class roles, responsibilities, co-curricular and elections
        [HttpGet("students/{id:int}/activities")]
        public async Task<IActionResult> StudentActivities(int id)
        {
            var classRoles = await ClassRolesAsync(id);
            var academics = await Has(AppModules.Academics);

            var responsibilities = !academics ? new List<object>() : (await _db.StudentResponsibilities.AsNoTracking()
                .Where(r => r.StudentId == id)
                .Select(r => new
                {
                    Year = r.AcademicYear!.Name,
                    Name = r.ResponsibilitySocialSkill!.Name,
                    r.ResponsibilitySocialSkill.Category,
                    r.Description
                })
                .ToListAsync()).OrderByDescending(r => r.Year).Cast<object>().ToList();

            var coCurricular = !academics ? new List<object>() : (await _db.StudentCoCurriculumActivities.AsNoTracking()
                .Where(a => a.StudentId == id)
                .Select(a => new { Name = a.CoCurriculumActivity!.Name, a.Description })
                .ToListAsync()).Cast<object>().ToList();

            var elections = new List<object>();
            if (await Has(AppModules.Elections))
            {
                var stood = await _db.ElectionCandidates.AsNoTracking()
                    .Where(c => c.StudentId == id || c.RunningMateStudentId == id)
                    .Select(c => new
                    {
                        Election = c.ElectionContest!.ElectionPosition!.Election!.Name,
                        Stage = c.ElectionContest.ElectionPosition.Election.Stage,
                        Year = c.ElectionContest.ElectionPosition.Election.AcademicYear!.Name,
                        Position = c.ElectionContest.ElectionPosition.Name,
                        Race = c.ElectionContest.Name,
                        RunningMateTitle = c.ElectionContest.ElectionPosition.RunningMateTitle,
                        AsRunningMate = c.RunningMateStudentId == id,
                        Partner = c.RunningMateStudentId == id ? c.Student!.FullName : c.RunningMateStudent!.FullName,
                        c.Status,
                        c.IsElected,
                        c.Votes
                    })
                    .ToListAsync();
                elections = stood.Select(c =>
                {
                    var declared = c.Stage == ElectionStage.Declared;
                    return (object)new
                    {
                        c.Election,
                        c.Year,
                        Position = c.AsRunningMate ? c.RunningMateTitle ?? c.Position : c.Position,
                        c.Race,
                        c.AsRunningMate,
                        c.Partner,
                        Outcome = c.Status == CandidateStatus.Rejected ? "Nomination rejected"
                            : c.Status == CandidateStatus.Withdrawn ? "Withdrew"
                            : c.Status == CandidateStatus.Nominated ? "Nominated"
                            : !declared ? "Candidate"
                            : c.IsElected ? "Elected" : "Not elected",
                        Elected = declared && c.IsElected,
                        Votes = declared ? c.Votes : (int?)null
                    };
                }).ToList();
            }

            return Ok(new { ClassRoles = classRoles, Responsibilities = responsibilities, CoCurricular = coCurricular, Elections = elections });
        }

        // GET api/profiles/staff/5/duties - class roles, and polling stations run as returning officer
        [HttpGet("staff/{id:int}/duties")]
        public async Task<IActionResult> StaffDuties(int id)
        {
            var classRoles = await ClassRolesAsync(id);
            var stations = new List<object>();
            if (await Has(AppModules.Elections))
            {
                stations = (await _db.ElectionStations.AsNoTracking()
                    .Where(s => s.OfficerUser != null && s.OfficerUser.PersonId == id)
                    .Select(s => new { Election = s.Election!.Name, Year = s.Election.AcademicYear!.Name, Station = s.Name, s.Election.Stage })
                    .ToListAsync())
                    .OrderByDescending(s => s.Year)
                    .Select(s => (object)new { s.Election, s.Year, s.Station, Stage = s.Stage.ToString() })
                    .ToList();
            }
            return Ok(new { ClassRoles = classRoles, Stations = stations });
        }

        // GET api/profiles/staff/5/payslips - the staff member's payslips, newest first.
        // They see their own once the payroll is approved; payroll officers see all.
        [HttpGet("staff/{id:int}/payslips")]
        [RequiresModule(AppModules.Payroll)]
        public async Task<IActionResult> StaffPayslips(int id)
        {
            var (self, officer) = await PayAccessAsync(id);
            if (!self && !officer) return Forbid();
            var list = await _db.Payslips.AsNoTracking()
                .Where(p => p.StaffDetailsId == id)
                .Where(p => officer || p.PayrollPeriod!.Status == PayrollPeriodStatus.Approved || p.PayrollPeriod.Status == PayrollPeriodStatus.Posted)
                .OrderByDescending(p => p.PayrollPeriod!.Year).ThenByDescending(p => p.PayrollPeriod!.Month)
                .Select(p => new
                {
                    p.Id,
                    p.PayrollPeriodId,
                    Period = p.PayrollPeriod!.Name,
                    p.PayrollPeriod.Month,
                    p.PayrollPeriod.Year,
                    Status = p.PayrollPeriod.Status.ToString(),
                    p.GrossPay,
                    p.TotalDeductions,
                    p.NetPay
                })
                .ToListAsync();
            return Ok(list);
        }

        // GET api/profiles/staff/5/payslips/31 - one payslip in full, for printing
        [HttpGet("staff/{id:int}/payslips/{payslipId:int}")]
        [RequiresModule(AppModules.Payroll)]
        public async Task<IActionResult> StaffPayslip(int id, int payslipId)
        {
            var (self, officer) = await PayAccessAsync(id);
            if (!self && !officer) return Forbid();
            var payslip = await _unitOfWork.Payslips.GetByIdFull(payslipId);
            if (payslip == null || payslip.StaffDetailsId != id) return NotFound();
            var released = payslip.PayrollPeriod!.Status is PayrollPeriodStatus.Approved or PayrollPeriodStatus.Posted;
            if (!officer && !released) return NotFound();
            return Ok(_mapper.Map<PayslipDto>(payslip));
        }

        // Class leadership roles a student or staff member has held, newest year first.
        private async Task<List<object>> ClassRolesAsync(int personId) =>
            (await _db.SchoolClassLeaders.AsNoTracking()
                .Where(l => l.PersonId == personId)
                .Select(l => new
                {
                    Year = l.SchoolClass!.AcademicYear!.Name,
                    ClassName = l.SchoolClass.Name,
                    Role = l.ClassLeadershipRole!.Name,
                    l.Description
                })
                .ToListAsync())
                .OrderByDescending(l => l.Year)
                .Cast<object>()
                .ToList();

        // Self: the signed-in user is this staff member. Officer: an administrator
        // or a role holding the payroll menu.
        private async Task<(bool Self, bool Officer)> PayAccessAsync(int staffId)
        {
            var userIdClaim = User.FindFirst("userid")?.Value;
            if (!int.TryParse(userIdClaim, out var userId)) return (false, false);
            var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
            var self = user?.PersonId == staffId;
            var roles = User.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value).ToList();
            var officer = roles.Contains("Administrator") || roles.Contains("SuperAdministrator")
                || await _db.MenuPermissions.AnyAsync(p => p.MenuPath == PayrollMenu && roles.Contains(p.RoleId));
            return (self, officer);
        }
    }
}
