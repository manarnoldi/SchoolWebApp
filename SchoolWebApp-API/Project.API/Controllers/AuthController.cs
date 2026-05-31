using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.Entities.Identity;
using SchoolWebApp.Core.Services;

namespace SchoolWebApp.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly JwtService _jwtService;
        private readonly IAuditService _audit;

        public AuthController(UserManager<AppUser> userManager, JwtService jwtService, IAuditService audit)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _audit = audit;
        }

        //POST: api/auth/login
        [HttpPost("login")]
        public async Task<ActionResult<AuthenticationResponse>> CreateBearerToken(AuthenticationRequest request)
        {
            if (!ModelState.IsValid) return BadRequest("Bad credentials");

            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user == null)
            {
                await _audit.LogLoginFailedAsync(request.UserName, "User not found");
                return BadRequest("Bad credentials");
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!isPasswordValid)
            {
                await _audit.LogLoginFailedAsync(request.UserName, "Invalid password");
                return BadRequest("Bad credentials");
            }

            var token = _jwtService.CreateToken(user, await _userManager.GetRolesAsync(user));

            // Successful sign-in - record after the token is built so
            // a token-generation failure surfaces as LoginFailed
            // rather than a misleading Login + 500.
            await _audit.LogLoginAsync(user.UserName, user.Id.ToString());

            return Ok(token);
        }

        //POST: api/auth/change-password
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.FindFirst("userid")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound(new { message = "User not found." });

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return BadRequest(new { message = string.Join(" ", errors) });
            }

            if (user.MustChangePassword)
            {
                user.MustChangePassword = false;
                await _userManager.UpdateAsync(user);
            }

            return Ok(new { message = "Password changed successfully." });
        }
    }
}