using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.API.Licensing;
using SchoolWebApp.API.Services;
using SchoolWebApp.Core.Entities.Communications;
using SchoolWebApp.Core.Entities.Elections;
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

        private async Task<SchoolWebApp.Core.Entities.Class.Session?> CurrentSessionAsync(DateTime today) =>
            await _db.Sessions.AsNoTracking().Include(s => s.AcademicYear)
                .Where(s => s.StartDate <= today && s.EndDate >= today).OrderByDescending(s => s.StartDate).FirstOrDefaultAsync()
            ?? await _db.Sessions.AsNoTracking().Include(s => s.AcademicYear)
                .Where(s => s.StartDate <= today).OrderByDescending(s => s.StartDate).FirstOrDefaultAsync();
    }
}
