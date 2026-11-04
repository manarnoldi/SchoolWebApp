using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.API.Licensing;

namespace SchoolWebApp.API.Controllers
{
    /// <summary>
    /// The user guide (everyone) and the administrator guide (administrators), built by
    /// tools/user-guide into Guides/. Served only to a signed-in user, only while
    /// Security > Show user guides is on (off on the demo site), and without the
    /// chapters of modules the school's licence does not include.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/userGuide")]
    public class UserGuideController : ControllerBase
    {
        private static readonly string[] AdminRoles = { "Administrator", "SuperAdministrator" };

        // <!--module:Finance--> ... <!--/module:Finance--> around a module's chapters, sections and contents entries.
        private static readonly Regex ModuleBlock = new(@"<!--module:(\w+)-->.*?<!--/module:\1-->", RegexOptions.Singleline | RegexOptions.Compiled);

        private readonly ApplicationDbContext _db;
        private readonly LicenceService _licence;
        private readonly IWebHostEnvironment _env;

        public UserGuideController(ApplicationDbContext db, LicenceService licence, IWebHostEnvironment env)
        {
            _db = db;
            _licence = licence;
            _env = env;
        }

        // GET api/userGuide - which guides this user is shown
        [HttpGet]
        public async Task<IActionResult> Available()
        {
            var on = await GuidesOnAsync();
            return Ok(new { user = on, admin = on && IsAdmin() });
        }

        // GET api/userGuide/user | api/userGuide/admin - the guide as one HTML page
        [HttpGet("{kind}")]
        public async Task<IActionResult> Get(string kind)
        {
            kind = kind.ToLowerInvariant();
            if (kind != "user" && kind != "admin") return NotFound();
            if (!await GuidesOnAsync()) return NotFound();
            if (kind == "admin" && !IsAdmin()) return Forbid();

            var path = Path.Combine(_env.ContentRootPath, "Guides", $"{kind}-guide.html");
            if (!System.IO.File.Exists(path)) return NotFound();
            var html = await System.IO.File.ReadAllTextAsync(path);

            var modules = (await _licence.StatusAsync(Request.Host.Host)).Modules;
            html = ModuleBlock.Replace(html, m => modules.Contains(m.Groups[1].Value) ? m.Value : "");
            return Content(html, "text/html; charset=utf-8");
        }

        private bool IsAdmin() => AdminRoles.Any(User.IsInRole);

        // On unless an administrator turned it off.
        private async Task<bool> GuidesOnAsync()
        {
            var value = await _db.GlobalSettings
                .Where(s => s.Module == "Security" && s.SettingKey == "ShowUserGuides")
                .Select(s => s.SettingValue)
                .FirstOrDefaultAsync();
            return !string.Equals(value, "false", StringComparison.OrdinalIgnoreCase);
        }
    }
}
