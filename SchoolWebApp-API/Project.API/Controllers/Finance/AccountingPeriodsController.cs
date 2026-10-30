using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.API.Licensing;
using SchoolWebApp.API.Security;
using SchoolWebApp.API.Services;
using System.Security.Claims;

namespace SchoolWebApp.API.Controllers.Finance
{
    /// <summary>
    /// The accounting periods (terms or years, per the Finance setting), their state, closing and reopening.
    /// </summary>
    [Route("api/accountingPeriods")]
    [ApiController]
    [Authorize]
    [RequiresModule(AppModules.Finance)]
    [RequiresMenu(MenuGroup.Finance)]
    public class AccountingPeriodsController : ControllerBase
    {
        private readonly AccountingPeriodService _periods;
        public AccountingPeriodsController(AccountingPeriodService periods) { _periods = periods; }

        // GET api/accountingPeriods?academicYearId=8 - the year's periods (all years when left out), each with its
        // income and expenses, whether it is closed (by whom, when), and what stops it being closed.
        [HttpGet]
        public async Task<IActionResult> List(int? academicYearId)
        {
            var all = await _periods.All();
            var closes = await _periods.Closes();
            var latestClosed = closes.Where(c => c.Status == Core.Entities.Finance.AccountingPeriodCloseStatus.Closed)
                .OrderByDescending(c => c.StartDate).FirstOrDefault();
            var rows = new List<object>();
            foreach (var p in all.Where(x => academicYearId == null || x.AcademicYearId == academicYearId))
            {
                var close = closes.FirstOrDefault(c => c.PeriodKey == p.Key && c.Status == Core.Entities.Finance.AccountingPeriodCloseStatus.Closed);
                var result = await _periods.Results(p.Start, p.End);
                var blockers = close == null ? await _periods.Blockers(p, all, closes) : new List<string>();
                rows.Add(new
                {
                    key = p.Key, name = p.Name, startDate = p.Start, endDate = p.End, academicYearId = p.AcademicYearId,
                    current = p.Start <= DateTime.Today && p.End >= DateTime.Today,
                    closed = close != null,
                    closedAt = close?.Created, closedBy = close?.CreatedBy, closingJournalId = close?.ClosingJournalId,
                    income = result.Income, expenses = result.Expenses, net = result.Income - result.Expenses,
                    canReopen = close != null && latestClosed?.PeriodKey == p.Key,
                    blockers,
                    history = closes.Where(c => c.PeriodKey == p.Key).OrderBy(c => c.Created)
                        .Select(c => new { closedAt = c.Created, closedBy = c.CreatedBy, reopenedAt = c.ReopenedAt, reopenedBy = c.ReopenedBy })
                });
            }
            return Ok(new { termly = await _periods.Termly(), periods = rows });
        }

        // POST api/accountingPeriods/close?key=T:4
        [HttpPost("close")]
        public async Task<IActionResult> Close(string key)
        {
            try
            {
                var c = await _periods.Close(key);
                return Ok(new { message = $"{c.Name} closed: {c.Income - c.Expenses:N2} to Retained Earnings.", c.ClosingJournalId });
            }
            catch (InvalidOperationException e) { return BadRequest(new { message = e.Message }); }
        }

        // POST api/accountingPeriods/reopen?key=T:4 - administrators only, the latest closed period only.
        [HttpPost("reopen")]
        public async Task<IActionResult> Reopen(string key)
        {
            if (!User.IsInRole("Administrator") && !User.IsInRole("SuperAdministrator"))
                return StatusCode(403, new { message = "Only an administrator can reopen a closed period." });
            try
            {
                await _periods.Reopen(key, User.FindFirstValue("username"));
                return Ok(new { message = "Reopened." });
            }
            catch (InvalidOperationException e) { return BadRequest(new { message = e.Message }); }
        }
    }
}
