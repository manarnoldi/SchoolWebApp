using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;

namespace SchoolWebApp.API.Services
{
    /// <summary>
    /// Whether the caller may open a page, by the side menu's rule: super
    /// administrators see everything, everyone else only the menu paths their
    /// roles hold (a role with none has no pages). Used by the API's menu checks
    /// (RequiresMenu) and the dashboard cards.
    /// </summary>
    public class MenuAccessService
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _http;
        private List<string>? _paths;

        public MenuAccessService(ApplicationDbContext db, IHttpContextAccessor http)
        {
            _db = db;
            _http = http;
        }

        public async Task<bool> CanAnyAsync(params string[] paths)
        {
            var user = _http.HttpContext?.User;
            if (user == null) return false;
            var roles = user.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value).ToList();
            if (roles.Contains("SuperAdministrator")) return true;
            _paths ??= await _db.MenuPermissions.AsNoTracking()
                .Where(p => roles.Contains(p.RoleId))
                .Select(p => p.MenuPath).Distinct().ToListAsync();
            return paths.Any(_paths.Contains);
        }
    }
}
