using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SchoolWebApp.API.Services;

namespace SchoolWebApp.API.Security
{
    /// <summary>The pages (menu paths) whose holders use an API: a role needs one of them.</summary>
    public enum MenuGroup
    {
        Finance,
        FinanceLookups,
        Payroll,
        Communications,
        ParentMessaging,
        GlobalSettings,
        Dropdowns,
        Students,
        Staff,
        SchoolSetup,
        Academics,
        Elections
    }

    public static class MenuGroups
    {
        public static readonly Dictionary<MenuGroup, string[]> Paths = new()
        {
            [MenuGroup.Finance] = new[]
            {
                "/finance/budget-register", "/finance/budgets", "/finance/budget-amendments", "/finance/fee-structures",
                "/finance/invoices", "/finance/payments", "/finance/sponsors", "/finance/sponsorships", "/finance/sponsor-payments",
                "/finance/expenses", "/finance/journal-entries", "/finance/reports/fees", "/finance/reports/expenses",
                "/finance/reports/statements", "/finance/reports"
            },
            // Chart of accounts and fee categories are also set up and picked outside Finance.
            [MenuGroup.FinanceLookups] = new[]
            {
                "/settings/dropdowns", "/settings/global-settings", "/payroll/deduction-types", "/payroll/employee-salaries", "/payroll/periods"
            },
            [MenuGroup.Payroll] = new[]
            {
                "/payroll/employee-salaries", "/payroll/loan-advances", "/payroll/periods", "/payroll/reports",
                // Set-up pages, opened from Settings > Dropdowns > Payroll Settings.
                "/payroll/earning-types", "/payroll/deduction-types", "/payroll/tax-bands", "/payroll/nssf-bands",
                "/payroll/reliefs", "/payroll/settings"
            },
            [MenuGroup.Communications] = new[]
            {
                "/communications/compose", "/communications/queue", "/communications/templates",
                "/communications/sms-report", "/communications/settings"
            },
            // Pages with a "send to parents" button (results, invoices, payments, balances).
            [MenuGroup.ParentMessaging] = new[]
            {
                "/cbe/exams/school-exams", "/reports/academics/exam-results", "/reports/academics/results-analysis",
                "/finance/invoices", "/finance/payments", "/finance/reports/fees", "/finance/reports/statements"
            },
            [MenuGroup.GlobalSettings] = new[] { "/settings/global-settings" },
            [MenuGroup.Dropdowns] = new[] { "/settings/dropdowns" },
            [MenuGroup.Students] = new[]
            {
                "/students/details", "/students/parents", "/students/students-subjects", "/students/students-attendances",
                "/students/promotion", "/class/classes"
            },
            [MenuGroup.Staff] = new[] { "/staff/details", "/staff/staff-attendances", "/school/bulk-staff-subjects" },
            [MenuGroup.SchoolSetup] = new[]
            {
                "/school/details", "/school/academicYears", "/school/sessions", "/school/events", "/settings/dropdowns"
            },
            [MenuGroup.Academics] = new[]
            {
                "/cbe/assessments/assessments", "/cbe/assessments/admin", "/cbe/exams/school-exams", "/cbe/exams/exams",
                "/cbe/exams/exam-results-bulk", "/cbe/cocurriculum/student-scores", "/cbe/responsibilities/student-assignments",
                "/cbe/community-service/student-assignments", "/cbe/values/student-assignments", "/cbe/exams/exam-results",
                "/reports/academics/exam-results", "/reports/academics/subject-performance", "/reports/academics/class-performance"
            },
            [MenuGroup.Elections] = new[] { "/elections/manage", "/elections/polling", "/elections/results" }
        };
    }

    /// <summary>
    /// An API used by certain pages: the caller's roles must hold one of their menus
    /// (see MenuAccessService). WritesOnly leaves reads open (lookups many pages
    /// use). LogOnly records who would be refused without refusing, until
    /// Security:MenuChecks is set to "Enforce".
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RequiresMenuAttribute : Attribute
    {
        public MenuGroup[] Groups { get; }
        public bool WritesOnly { get; set; }
        public bool LogOnly { get; set; }

        public RequiresMenuAttribute(params MenuGroup[] groups)
        {
            Groups = groups;
        }
    }

    /// <summary>Global filter applying RequiresMenu; an action's own attribute replaces its controller's.</summary>
    public class MenuPermissionFilter : IAsyncActionFilter
    {
        private readonly MenuAccessService _access;
        private readonly ILogger<MenuPermissionFilter> _logger;
        private readonly bool _enforceAll;

        public MenuPermissionFilter(MenuAccessService access, ILogger<MenuPermissionFilter> logger, IConfiguration config)
        {
            _access = access;
            _logger = logger;
            _enforceAll = string.Equals(config["Security:MenuChecks"], "Enforce", StringComparison.OrdinalIgnoreCase);
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var metadata = context.ActionDescriptor.EndpointMetadata.OfType<RequiresMenuAttribute>().ToList();
            // Endpoint metadata lists the controller's attributes first, then the action's.
            var rule = metadata.LastOrDefault();
            var write = !HttpMethods.IsGet(context.HttpContext.Request.Method) && !HttpMethods.IsHead(context.HttpContext.Request.Method);
            if (rule != null && (write || !rule.WritesOnly))
            {
                var paths = rule.Groups.SelectMany(g => MenuGroups.Paths[g]).Distinct().ToArray();
                if (!await _access.CanAnyAsync(paths))
                {
                    var who = context.HttpContext.User.FindFirst("username")?.Value ?? context.HttpContext.User.FindFirst("userid")?.Value ?? "?";
                    var roles = string.Join(",", context.HttpContext.User.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value));
                    var call = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
                    if (rule.LogOnly && !_enforceAll)
                    {
                        _logger.LogWarning("Menu permission (not enforced yet): {User} [{Roles}] would be refused {Call} - needs one of {Groups}",
                            who, roles, call, string.Join(", ", rule.Groups));
                    }
                    else
                    {
                        _logger.LogWarning("Menu permission: refused {User} [{Roles}] {Call} - needs one of {Groups}",
                            who, roles, call, string.Join(", ", rule.Groups));
                        context.Result = new ObjectResult("You do not have permission for this page.") { StatusCode = StatusCodes.Status403Forbidden };
                        return;
                    }
                }
            }
            await next();
        }
    }
}
