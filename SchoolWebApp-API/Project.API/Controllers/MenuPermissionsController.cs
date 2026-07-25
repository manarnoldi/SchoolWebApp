using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.Entities.Identity;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MenuPermissionsController : ControllerBase
    {
        private readonly ILogger<MenuPermissionsController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public MenuPermissionsController(ILogger<MenuPermissionsController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        // GET: api/menuPermissions
        [HttpGet]
        [Authorize(Policy = "AdminRole")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var items = await _unitOfWork.Repository<MenuPermission>().Find();
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving menu permissions.");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/menuPermissions/byRole/{roleId}
        [HttpGet("byRole/{roleId}")]
        [Authorize(Policy = "AdminRole")]
        public async Task<IActionResult> GetByRole(string roleId)
        {
            try
            {
                var items = await _unitOfWork.Repository<MenuPermission>()
                    .Find(p => p.RoleId == roleId);
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving menu permissions by role.");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/menuPermissions/myPermissions
        [HttpGet("myPermissions")]
        public async Task<IActionResult> GetMyPermissions()
        {
            try
            {
                var userId = User.FindFirst("userid")?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var roleClaims = User.FindAll(System.Security.Claims.ClaimTypes.Role)
                    .Select(c => c.Value).ToList();

                if (roleClaims.Contains("SuperAdministrator"))
                {
                    return Ok(new { allAccess = true, paths = new List<string>() });
                }

                var allPermissions = await _unitOfWork.Repository<MenuPermission>().Find();
                var rolePermissions = allPermissions
                    .Where(p => roleClaims.Contains(p.RoleId, StringComparer.OrdinalIgnoreCase))
                    .Select(p => p.MenuPath)
                    .Distinct()
                    .ToList();

                return Ok(new { allAccess = false, paths = rolePermissions });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user permissions.");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // POST: api/menuPermissions/save
        /// <summary>
        /// Applies only the changes made in the UI: deletes the deselected paths
        /// and creates the newly-selected ones for the role. Unchanged rows are
        /// left untouched (no full delete-and-reinsert), so a save only writes
        /// the delta.
        /// </summary>
        [HttpPost("save")]
        [Authorize(Policy = "AdminRole")]
        public async Task<IActionResult> SavePermissions(SaveMenuPermissionsRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var repo = _unitOfWork.Repository<MenuPermission>();

                // Remove deselected paths.
                if (request.Removed != null && request.Removed.Any())
                {
                    var toRemove = await repo.Find(p => p.RoleId == request.RoleId
                        && request.Removed.Contains(p.MenuPath));
                    foreach (var item in toRemove)
                        repo.Delete(item);
                }

                // Add newly-selected paths, skipping any that already exist so a
                // double-save can't create duplicates.
                if (request.Added != null && request.Added.Any())
                {
                    var existingPaths = (await repo.Find(p => p.RoleId == request.RoleId))
                        .Select(p => p.MenuPath)
                        .ToHashSet();
                    foreach (var path in request.Added)
                    {
                        if (existingPaths.Contains(path.Path)) continue;
                        repo.Create(new MenuPermission
                        {
                            RoleId = request.RoleId,
                            MenuPath = path.Path,
                            MenuName = path.Name
                        });
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                return Ok(new { message = "Permissions saved successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving menu permissions.");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }

    public class SaveMenuPermissionsRequest
    {
        public required string RoleId { get; set; }
        // Paths newly selected in the UI (to create).
        public List<MenuPathItem> Added { get; set; } = new();
        // Paths newly deselected in the UI (to delete).
        public List<string> Removed { get; set; } = new();
    }

    public class MenuPathItem
    {
        public required string Path { get; set; }
        public string? Name { get; set; }
    }
}
