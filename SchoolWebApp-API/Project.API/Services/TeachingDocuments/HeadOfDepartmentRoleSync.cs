using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.Entities.Identity;

namespace SchoolWebApp.API.Services.TeachingDocuments
{
    /// <summary>
    /// Keeps the HeadOfDepartment role in step with who heads a department, so
    /// heads show up as approvers without anyone assigning the role by hand.
    /// A staff member gets the role when made a head and loses it only once
    /// they head no department at all. Applies to the user accounts linked to
    /// the staff record (AppUser.PersonId).
    /// </summary>
    public class HeadOfDepartmentRoleSync
    {
        public const string RoleName = "HeadOfDepartment";

        private readonly ApplicationDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<HeadOfDepartmentRoleSync> _logger;

        public HeadOfDepartmentRoleSync(ApplicationDbContext db, UserManager<AppUser> userManager,
            ILogger<HeadOfDepartmentRoleSync> logger)
        {
            _db = db;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>The department's head as saved, before an edit or delete.</summary>
        public Task<int?> SavedHeadAsync(int departmentId) =>
            _db.Departments.AsNoTracking().Where(d => d.Id == departmentId)
                .Select(d => d.StaffDetailsId).FirstOrDefaultAsync();

        /// <summary>
        /// Call after the department change is saved, with the staff whose
        /// headship may have changed (old and new head). Never throws: a role
        /// that fails to sync is logged and the department change stands.
        /// </summary>
        public async Task SyncAsync(params int?[] staffDetailsIds)
        {
            foreach (var staffId in staffDetailsIds.Where(s => s.HasValue).Select(s => s!.Value).Distinct())
            {
                try
                {
                    var isHead = await _db.Departments.AnyAsync(d => d.StaffDetailsId == staffId);
                    var users = await _userManager.Users.Where(u => u.PersonId == staffId).ToListAsync();
                    foreach (var user in users)
                    {
                        var hasRole = await _userManager.IsInRoleAsync(user, RoleName);
                        if (isHead && !hasRole) await _userManager.AddToRoleAsync(user, RoleName);
                        else if (!isHead && hasRole) await _userManager.RemoveFromRoleAsync(user, RoleName);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Could not sync the {Role} role for staff {StaffId}.", RoleName, staffId);
                }
            }
        }
    }
}
