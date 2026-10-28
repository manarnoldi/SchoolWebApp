using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.API.Licensing;

namespace SchoolWebApp.API.Controllers
{
    /// <summary>The school's licence: what it includes, and installing a new key.</summary>
    [Authorize]
    [ApiController]
    [Route("api/licence")]
    public class LicenceController : ControllerBase
    {
        private readonly LicenceService _licence;

        public LicenceController(LicenceService licence)
        {
            _licence = licence;
        }

        // GET api/licence - every signed-in user: the menus and pages follow it
        [HttpGet]
        public async Task<ActionResult<LicenceStatus>> Get() =>
            await _licence.StatusAsync(Request.Host.Host);

        public class InstallLicenceDto
        {
            public string Key { get; set; } = "";
        }

        // POST api/licence - administrators install the key Kodetek sent
        [HttpPost]
        [Authorize(Roles = "Administrator,SuperAdministrator")]
        public async Task<ActionResult<LicenceStatus>> Install([FromBody] InstallLicenceDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Key)) return BadRequest("Paste the licence key.");
            var status = _licence.Check(dto.Key, Request.Host.Host);
            if (status.State is "Invalid" or "WrongSite" or "Expired") return BadRequest(status.Message);
            await _licence.SaveAsync(dto.Key);
            return await _licence.StatusAsync(Request.Host.Host);
        }
    }
}
