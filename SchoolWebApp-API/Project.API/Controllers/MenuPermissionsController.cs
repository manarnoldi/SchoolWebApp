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

                if (roleClaims.Contains("Administrator"))
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
        [HttpPost("save")]
        [Authorize(Policy = "AdminRole")]
        public async Task<IActionResult> SavePermissions(SaveMenuPermissionsRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                // Delete existing permissions for this role
                var existing = await _unitOfWork.Repository<MenuPermission>()
                    .Find(p => p.RoleId == request.RoleId);
                foreach (var item in existing)
                {
                    _unitOfWork.Repository<MenuPermission>().Delete(item);
                }

                // Insert new permissions
                foreach (var path in request.MenuPaths)
                {
                    _unitOfWork.Repository<MenuPermission>().Create(new MenuPermission
                    {
                        RoleId = request.RoleId,
                        MenuPath = path.Path,
                        MenuName = path.Name
                    });
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
        public List<MenuPathItem> MenuPaths { get; set; } = new();
    }

    public class MenuPathItem
    {
        public required string Path { get; set; }
        public string? Name { get; set; }
    }
}
