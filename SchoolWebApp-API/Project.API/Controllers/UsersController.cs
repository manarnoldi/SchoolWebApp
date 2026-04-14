using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolSoftWebApi.Models.Identity;
using SchoolWebApp.Core.Entities.Identity;

namespace SchoolWebApp.API.Controllers
{
    [Authorize(Policy = "AdminRole")]
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly ILogger<UsersController> _logger;

        public UsersController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IMapper mapper, ILogger<UsersController> logger)
        {
            _mapper = mapper;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        //GET: api/users
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var result = new List<UserWithRolesDto>();
            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);
                result.Add(new UserWithRolesDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Roles = roles.ToList()
                });
            }
            return Ok(result);
        }

        //GET: api/users/username
        [HttpGet("username")]
        public async Task<ActionResult<UserToReturn>> GetUser([FromBody] string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return NotFound();
            return Ok(_mapper.Map<UserToReturn>(user));
        }

        //GET: api/users/Id
        [HttpGet("{Id}")]
        public async Task<ActionResult<UserWithRolesDto>> GetUserById(int Id)
        {
            var user = await _userManager.FindByIdAsync(Id.ToString());
            if (user == null) return NotFound();
            var roles = await _userManager.GetRolesAsync(user);
            return Ok(new UserWithRolesDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = roles.ToList()
            });
        }

        //GET: api/users/userRoles/{id}
        [HttpGet("userRoles/{id}")]
        public async Task<IActionResult> GetUserRoles(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound();
            var roles = await _userManager.GetRolesAsync(user);
            return Ok(roles);
        }

        //POST: api/users
        [HttpPost]
        public async Task<ActionResult<UserToReturn>> PostUser(UserDTO user)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userToSave = new AppUser { UserName = user.UserName, Email = user.Email, FirstName = user.FirstName ?? "", LastName = user.LastName ?? "" };

            var result = await _userManager.CreateAsync(userToSave, user.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);

            var retUser = _mapper.Map<UserToReturn>(userToSave);

            return CreatedAtAction("GetUserById", new { Id = userToSave.Id }, retUser);
        }

        //PUT: api/users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserUpdateDTO model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString());
                if (user == null) return NotFound();

                user.FirstName = model.FirstName ?? user.FirstName;
                user.LastName = model.LastName ?? user.LastName;
                user.Email = model.Email ?? user.Email;
                user.UserName = model.UserName ?? user.UserName;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded) return BadRequest(result.Errors);

                if (!string.IsNullOrEmpty(model.Password))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var passResult = await _userManager.ResetPasswordAsync(user, token, model.Password);
                    if (!passResult.Succeeded) return BadRequest(passResult.Errors);
                }

                return Ok(_mapper.Map<UserToReturn>(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user.");
                return StatusCode(500, new { message = "Error updating user." });
            }
        }

        //DELETE: api/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString());
                if (user == null) return NotFound();

                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded) return BadRequest(result.Errors);

                return Ok(new { message = $"User {user.Email} deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user.");
                return StatusCode(500, new { message = "Error deleting user." });
            }
        }

        //POST: api/users/userRole
        [HttpPost]
        [Route("userRole")]
        public async Task<IActionResult> AddUserToRole(UserRoleDTO userRole)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(userRole.Email);
                if (user == null) return BadRequest(new { error = "User details not found." });

                var roleExist = await _roleManager.RoleExistsAsync(userRole.Role);
                if (!roleExist) return BadRequest(new { error = "Role not found in the system." });

                IdentityResult result = await _userManager.AddToRoleAsync(user, userRole.Role);
                if (result.Succeeded) return Ok(new { result = $"User {user.Email} added to the {userRole.Role} role." });
                else return BadRequest(result);
            }
            catch (Exception ex)
            {
                var errMessage = $"An error occurred while adding the user to the role.";
                _logger.LogError(ex, errMessage);
                return StatusCode(StatusCodes.Status500InternalServerError, errMessage);
            }
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

        //POST: api/users/reset-password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var user = await _userManager.FindByIdAsync(model.UserId.ToString());
                if (user == null) return NotFound(new { message = "User not found." });

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return BadRequest(new { message = string.Join(" ", errors) });
                }

                return Ok(new { message = $"Password for {user.UserName} has been reset successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password.");
                return StatusCode(500, new { message = "Error resetting password." });
            }
        }
    }
}
