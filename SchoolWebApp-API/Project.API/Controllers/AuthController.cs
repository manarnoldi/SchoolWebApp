using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.Entities.Identity;
using SchoolWebApp.Core.Services;

namespace SchoolSoftWebApi.Controllers
{
    /// <summary>
    /// This is a class for authenticating users. It generates a bearer token and sends it back to the requestor.
    /// </summary>
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        /// <summary>
        /// This is a field for storing injected user manager
        /// </summary>
        private readonly UserManager<AppUser> _userManager;
        /// <summary>
        /// This is a field for storing injected JwtService
        /// </summary>
        private readonly JwtService _jwtService;

        /// <summary>
        /// This is the constructor that injects the user manager and Jwt service
        /// </summary>
        /// <param name="userManager">The user manager parameter</param>
        /// <param name="jwtService">The Jwt service parameter</param>
        public AuthController(UserManager<AppUser> userManager, JwtService jwtService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
        }

        /// <summary>
        /// This method generates a token.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        //POST: api/auth/login
        [HttpPost("login")]
        public async Task<ActionResult<AuthenticationResponse>> CreateBearerToken(AuthenticationRequest request)
        {
            if (!ModelState.IsValid) return BadRequest("Bad credentials");

            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user == null) return BadRequest("Bad credentials");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!isPasswordValid) return BadRequest("Bad credentials");

            var token = _jwtService.CreateToken(user, await _userManager.GetRolesAsync(user));

            return Ok(token);
        }
    }
}