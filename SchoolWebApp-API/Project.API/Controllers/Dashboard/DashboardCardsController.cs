using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.API.Licensing;
using SchoolWebApp.API.Services;
using SchoolWebApp.Core.Entities.Communications;
using SchoolWebApp.Core.Entities.Elections;
using SchoolWebApp.Core.Entities.Enums;
using SchoolWebApp.Core.Entities.Finance;

namespace SchoolWebApp.API.Controllers.Dashboard
{
    /// <summary>
    /// The dashboard's module cards. Each answers only when the licence has the
    /// module and the caller's role holds the card.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/dashboardCards")]
    public class DashboardCardsController : ControllerBase
    {
        // Each card is a permission of its own (Security > Menu Permissions > Dashboard),
        // the same paths the client checks (core/models/dashboard-cards.ts).
        public static readonly string[] FinanceMenus = { "/dashboard/cards/fees" };
        public static readonly string[] PayrollMenus = { "/dashboard/cards/payroll" };
        public static readonly string[] ElectionsMenus = { "/dashboard/cards/elections" };
        public static readonly string[] CommunicationsMenus = { "/dashboard/cards/messages" };
        public static readonly string[] AssessmentsMenus = { "/dashboard/cards/assessments" };

        private readonly ApplicationDbContext _db;
        private readonly MenuAccessService _access;

        public DashboardCardsController(ApplicationDbContext db, MenuAccessService access)
        {
            _db = db;
            _access = access;
        }

        // GET api/dashboardCards/finance - this term's fees, collections, arrears and spending
        [HttpGet("finance")]
        [RequiresModule(AppModules.Finance)]
        public async Task<IActionResult> Finance()
        {
            if (!await _access.CanAnyAsync(FinanceMenus)) return Forbid();
            var today = DateTime.Today;
            var session = await CurrentSessionAsync(today);

            var live = _db.StudentInvoices.AsNoTracking().Where(i => i.Status != InvoiceStatus.Cancelled);
            var term = session == null ? null : await live.Where(i => i.SessionId == session.Id)
                .GroupBy(_ => 1)
                .Select(g => new { Expected = g.Sum(i => i.TotalAmount - i.DiscountAmount), Collected = g.Sum(i => i.PaidAmount) })
                .FirstOrDefaultAsync();
            var outstanding = await live.SumAsync(i => (decimal?)(i.TotalAmount - i.DiscountAmount - i.PaidAmount)) ?? 0;

            var receipts = _db.Payments.AsNoTracking()
                .Where(p => p.PaymentType == PaymentType.Receipt && p.ApprovalStatus == PaymentApprovalStatus.Approved);
            var weekStart = today.AddDays(-(((int)today.DayOfWeek + 6) % 7));
            var collectedToday = await receipts.Where(p => p.PaymentDate >= today && p.PaymentDate < today.AddDays(1)).SumAsync(p => (decimal?)p.Amount) ?? 0;
            var collectedWeek = await receipts.Where(p => p.PaymentDate >= weekStart && p.PaymentDate < today.AddDays(1)).SumAsync(p => (decimal?)p.Amount) ?? 0;

            var monthStart = new DateTime(today.Year, today.Month, 1);
            var spentMonth = await _db.Expenses.AsNoTracking()
                .Where(e => e.Status == ExpenseStatus.Approved && e.ExpenseDate >= monthStart && e.ExpenseDate < monthStart.AddMonths(1))
                .SelectMany(e => e.Lines).SumAsync(l => (decimal?)l.Amount) ?? 0;

            // Arrears by the student's class this year.
            var arrears = new List<object>();
            if (session != null)
            {
                var owing = await live.Where(i => i.TotalAmount - i.DiscountAmount - i.PaidAmount > 0)
                    .GroupBy(i => i.StudentId)
                    .Select(g => new { StudentId = g.Key, Balance = g.Sum(i => i.TotalAmount - i.DiscountAmount - i.PaidAmount) })
                    .ToListAsync();
                var ids = owing.Select(o => o.StudentId).ToList();
                var classOf = (await _db.StudentClasses.AsNoTracking()
                        .Where(sc => ids.Contains(sc.StudentId) && sc.SchoolClass!.AcademicYearId == session.AcademicYearId)
                        .Select(sc => new { sc.Id, sc.StudentId, sc.SchoolClass!.Name }).ToListAsync())
                    .GroupBy(x => x.StudentId).ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.Id).First().Name);
                arrears = owing.GroupBy(o => classOf.GetValueOrDefault(o.StudentId, "No class"))
                    .Select(g => new { ClassName = g.Key, Students = g.Count(), Balance = g.Sum(o => o.Balance) })
                    .OrderByDescending(x => x.Balance).Take(5).Cast<object>().ToList();
            }

            return Ok(new
            {
                Term = session == null ? null : $"{session.SessionName} {session.AcademicYear?.Name}",
                Expected = term?.Expected ?? 0,
                Collected = term?.Collected ?? 0,
                Outstanding = outstanding,
                CollectedToday = collectedToday,
                CollectedThisWeek = collectedWeek,
                SpentThisMonth = spentMonth,
                ArrearsByClass = arrears
            });
        }

        // GET api/dashboardCards/payroll - the latest payroll (where its gross goes) against
        // the one before, and net pay over the last six payrolls
        [HttpGet("payroll")]
        [RequiresModule(AppModules.Payroll)]
        public async Task<IActionResult> Payroll()
        {
            if (!await _access.CanAnyAsync(PayrollMenus)) return Forbid();
            var periods = await _db.PayrollPeriods.AsNoTracking()
                .OrderByDescending(p => p.Year).ThenByDescending(p => p.Month).Take(6)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Month,
                    p.Year,
                    Status = p.Status.ToString(),
                    Staff = p.Payslips.Count,
                    Gross = p.Payslips.Sum(s => (decimal?)s.GrossPay) ?? 0,
                    Net = p.Payslips.Sum(s => (decimal?)s.NetPay) ?? 0,
                    Paye = p.Payslips.Sum(s => (decimal?)s.Paye) ?? 0,
                    Nssf = p.Payslips.Sum(s => (decimal?)s.NssfEmployee) ?? 0,
                    Shif = p.Payslips.Sum(s => (decimal?)s.Shif) ?? 0,
                    Ahl = p.Payslips.Sum(s => (decimal?)s.Ahl) ?? 0,
                    Deductions = p.Payslips.Sum(s => (decimal?)s.TotalDeductions) ?? 0
                })
                .ToListAsync();
            return Ok(new
            {
                Latest = periods.FirstOrDefault(),
                Previous = periods.Skip(1).FirstOrDefault(),
                Trend = periods.AsEnumerable().Reverse().Select(p => new { p.Name, p.Month, p.Year, p.Net, p.Status })
            });
        }

        // GET api/dashboardCards/elections - elections under way, and the last one declared
        [HttpGet("elections")]
        [RequiresModule(AppModules.Elections)]
        public async Task<IActionResult> Elections()
        {
            if (!await _access.CanAnyAsync(ElectionsMenus)) return Forbid();
            var list = await _db.Elections.AsNoTracking()
                .Where(e => e.Stage != ElectionStage.Draft)
                .OrderByDescending(e => e.Id).Take(6)
                .Select(e => new
                {
                    e.Id,
                    e.Name,
                    e.Stage,
                    e.PollingStart,
                    Voted = _db.ElectionVoteCasts.Count(v => v.ElectionId == e.Id),
                    Registered = _db.ElectionVoters.Count(v => v.ElectionId == e.Id)
                })
                .ToListAsync();
            var active = list.Where(e => e.Stage != ElectionStage.Declared).ToList();
            var lastDeclared = list.FirstOrDefault(e => e.Stage == ElectionStage.Declared);
            if (lastDeclared != null) active.Add(lastDeclared);
            return Ok(active.Select(e => new
            {
                e.Id,
                e.Name,
                Stage = (int)e.Stage,
                e.PollingStart,
                e.Voted,
                Registered = e.Registered == 0 ? (int?)null : e.Registered
            }));
        }

        // GET api/dashboardCards/communications - messages in the last 7 days, by how they went
        [HttpGet("communications")]
        [RequiresModule(AppModules.Communications)]
        public async Task<IActionResult> Communications()
        {
            if (!await _access.CanAnyAsync(CommunicationsMenus)) return Forbid();
            var since = DateTime.Today.AddDays(-6);
            var counts = await _db.OutboundMessages.AsNoTracking()
                .Where(m => m.Created >= since)
                .GroupBy(m => m.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();
            int Count(params MessageStatus[] s) => counts.Where(c => s.Contains(c.Status)).Sum(c => c.Count);
            return Ok(new
            {
                Since = since,
                Delivered = Count(MessageStatus.Delivered, MessageStatus.Sent),
                Waiting = Count(MessageStatus.Queued, MessageStatus.Sending),
                Failed = Count(MessageStatus.Failed, MessageStatus.Undelivered),
                Total = counts.Sum(c => c.Count)
            });
        }

        // GET api/dashboardCards/assessments - this term's CBE outcome grades: how much has
        // been assessed, the EE/ME/AE/BE mix by class and learning area, and the learners
        // most of whose grades are below expectation
        [HttpGet("assessments")]
        [RequiresModule(AppModules.Academics)]
        public async Task<IActionResult> Assessments()
        {
            if (!await _access.CanAnyAsync(AssessmentsMenus)) return Forbid();
            var today = DateTime.Today;
            // The current term; before its first grades, the last term that has some.
            var outcomeGrades = _db.StudentAssessments.AsNoTracking().Where(a => a.SpecificOutcomeId != null);
            var session = await CurrentSessionAsync(today);
            if (session == null || !await outcomeGrades.AnyAsync(a => a.SessionId == session.Id))
                session = await _db.Sessions.AsNoTracking().Include(s => s.AcademicYear)
                    .Where(s => s.StartDate <= today && outcomeGrades.Any(a => a.SessionId == s.Id))
                    .OrderByDescending(s => s.StartDate).FirstOrDefaultAsync() ?? session;
            if (session == null) return Ok(new { Term = (string?)null });

            // Grades are bucketed by their level (EE, ME, AE, BE), so a 4- or 8-point scale reads the same.
            var levelOf = (await _db.Grades.AsNoTracking().Select(g => new { g.Id, g.Abbr }).ToListAsync())
                .ToDictionary(g => g.Id, g => (g.Abbr ?? "").Trim().ToUpperInvariant() is var a && a.Length >= 2 ? a[..2] : a);
            int Level(int gradeId) => Array.IndexOf(CbeLevels, levelOf.GetValueOrDefault(gradeId, ""));

            var cells = await outcomeGrades.Where(a => a.SessionId == session.Id)
                .GroupBy(a => new { a.SchoolClassId, a.SpecificOutcome!.SubStrand!.Strand!.SubjectId, a.GradeId })
                .Select(g => new { g.Key.SchoolClassId, g.Key.SubjectId, g.Key.GradeId, Count = g.Count() })
                .ToListAsync();
            var byStudent = await outcomeGrades.Where(a => a.SessionId == session.Id)
                .GroupBy(a => new { a.StudentId, a.GradeId })
                .Select(g => new { g.Key.StudentId, g.Key.GradeId, Count = g.Count() })
                .ToListAsync();

            // The learning areas each class is taught that have a curriculum to assess
            // (those switched off for teaching documents, e.g. Day Care, have none).
            var taught = await _db.StaffSubjects.AsNoTracking()
                .Where(s => s.SchoolClass!.AcademicYearId == session.AcademicYearId
                            && s.Subject!.RequiresTeachingDocuments && s.SchoolClass.LearningLevel!.RequiresTeachingDocuments)
                .Select(s => new { s.SchoolClassId, s.SubjectId }).Distinct().ToListAsync();
            var classIds = taught.Select(t => t.SchoolClassId).Union(cells.Select(c => c.SchoolClassId)).Distinct().ToList();
            var classes = await _db.SchoolClasses.AsNoTracking().Where(c => classIds.Contains(c.Id))
                .OrderBy(c => c.LearningLevel!.Rank).ThenBy(c => c.Rank).ThenBy(c => c.Name)
                .Select(c => new { c.Id, c.Name }).ToListAsync();
            var learners = await _db.StudentClasses.AsNoTracking()
                .Where(sc => classIds.Contains(sc.SchoolClassId) && sc.Student!.Status == Status.Active)
                .Select(sc => sc.StudentId).Distinct().CountAsync();
            var subjectIds = cells.Select(c => c.SubjectId).Distinct().ToList();
            var subjects = await _db.Subjects.AsNoTracking().Where(s => subjectIds.Contains(s.Id))
                .ToDictionaryAsync(s => s.Id, s => s.Name);

            int[] Mix(IEnumerable<(int GradeId, int Count)> rows)
            {
                var mix = new int[CbeLevels.Length];
                foreach (var (gradeId, count) in rows)
                    if (Level(gradeId) is var l && l >= 0) mix[l] += count;
                return mix;
            }
            // Below expectation: approaching (AE) or below (BE).
            static int Below(int[] mix) => mix[2] + mix[3];

            var byClass = classes.Select(c =>
            {
                var own = cells.Where(x => x.SchoolClassId == c.Id).ToList();
                var areas = taught.Where(t => t.SchoolClassId == c.Id).Select(t => t.SubjectId)
                    .Union(own.Select(x => x.SubjectId)).Distinct().Count();
                return new
                {
                    SchoolClassId = c.Id,
                    ClassName = c.Name,
                    Areas = areas,
                    AreasAssessed = own.Select(x => x.SubjectId).Distinct().Count(),
                    Mix = Mix(own.Select(x => (x.GradeId, x.Count)))
                };
            }).ToList();

            // Learning areas with the largest share below expectation, school-wide - only
            // those with enough grades to say so.
            var weakest = cells.GroupBy(c => c.SubjectId)
                .Select(g => new { SubjectId = g.Key, Mix = Mix(g.Select(x => (x.GradeId, x.Count))) })
                .Where(s => s.Mix.Sum() >= 50)
                .Select(s => new { Subject = subjects.GetValueOrDefault(s.SubjectId, "-"), Grades = s.Mix.Sum(), Below = Below(s.Mix) })
                .OrderByDescending(s => (double)s.Below / s.Grades).Take(5).ToList();

            // Needing support: at least half of a learner's (five or more) outcome grades below expectation.
            var students = byStudent.GroupBy(s => s.StudentId)
                .Select(g => Mix(g.Select(x => (x.GradeId, x.Count)))).ToList();
            var needSupport = students.Count(m => m.Sum() >= 5 && Below(m) * 2 >= m.Sum());

            return Ok(new
            {
                Term = $"{session.SessionName} {session.AcademicYear?.Name}",
                Levels = CbeLevels,
                Mix = Mix(cells.Select(c => (c.GradeId, c.Count))),
                Learners = learners,
                LearnersAssessed = students.Count,
                Areas = byClass.Sum(c => c.Areas),
                AreasAssessed = byClass.Sum(c => c.AreasAssessed),
                NeedSupport = needSupport,
                Classes = byClass.Where(c => c.Areas > 0),
                Weakest = weakest
            });
        }

        private static readonly string[] CbeLevels = { "EE", "ME", "AE", "BE" };

        private async Task<SchoolWebApp.Core.Entities.Class.Session?> CurrentSessionAsync(DateTime today) =>
            await _db.Sessions.AsNoTracking().Include(s => s.AcademicYear)
                .Where(s => s.StartDate <= today && s.EndDate >= today).OrderByDescending(s => s.StartDate).FirstOrDefaultAsync()
            ?? await _db.Sessions.AsNoTracking().Include(s => s.AcademicYear)
                .Where(s => s.StartDate <= today).OrderByDescending(s => s.StartDate).FirstOrDefaultAsync();
    }
}
