using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolSoftWebApi.Models.Identity;
using SchoolWebApp.Core.Entities.Identity;
using SchoolWebApp.Core.Services;

namespace SchoolWebApp.API.Controllers
{
    [Authorize(Policy = "AdminRole")]
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        public UsersController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper, JwtService jwtService)
        {
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        //GET: api/users
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<UserToReturn>>(users));
        }

        //GET: api/users/username
        [HttpGet("username")]
        public async Task<ActionResult<UserToReturn>> GetUser([FromBody] string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return NotFound();
            return Ok(_mapper.Map<UserToReturn>(user));
        }

        //GET: api/users/userRoles
        [HttpGet]
        [Route("userRoles")]
        public async Task<IActionResult> GetUserRoles([FromBody] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound();
            var roles = await _userManager.GetRolesAsync(user);
            return Ok(roles);
        }

        //POST: api/users
        [HttpPost]
        public async Task<ActionResult<UserToReturn>> PostUser(UserDTO user)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userToSave = new AppUser { UserName = user.UserName, Email = user.Email, FirstName = user.FirstName, LastName = user.LastName };

            var result = await _userManager.CreateAsync(userToSave, user.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);

            var retUser = _mapper.Map<UserToReturn>(userToSave);

            return CreatedAtAction("GetUser", new { username = user.UserName }, retUser);
        }

        //POST: api/users/userRole
        [HttpPost]
        [Route("userRole")]
        public async Task<IActionResult> AddUserToRole(UserRoleDTO userRole)
        {
            var user = await _userManager.FindByEmailAsync(userRole.Email);
            if (user == null) return BadRequest(new { error = "User details not found." });

            var roleExist = await _roleManager.RoleExistsAsync(userRole.Role);
            if (!roleExist) return BadRequest(new { error = "Role not found in the system." });

            IdentityResult result = await _userManager.AddToRoleAsync(user, userRole.Role);
            if (result.Succeeded) return Ok(new { result = $"User {user.Email} added to the {userRole.Role} role." });
            else return BadRequest(result);
        }

        //POST: api/users/removefromrole
        [HttpPost]
        [Route("removefromrole")]
        public async Task<IActionResult> RemoveUserFromRole(UserRoleDTO userRole)
        {
            var user = await _userManager.FindByEmailAsync(userRole.Email);
            if (user == null) return BadRequest(new { error = "User details not found!" });

            var roleExist = await _roleManager.RoleExistsAsync(userRole.Role);
            if (!roleExist) return BadRequest(new { error = "Role name not found!" });

            var result = await _userManager.RemoveFromRoleAsync(user, userRole.Role);

            if (result.Succeeded) return Ok(new { result = $"User {user.Email} removed from the {userRole.Role} role" });

            return BadRequest(result);
        }
    }
}