using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolSoftWebApi.Models.Identity;
using SchoolWebApp.Core.Entities.Identity;
using SchoolWebApp.Core.Entities.Shared;
using System.Security.Claims;

namespace SchoolWebApp.API.Controllers
{
    // Base requirement is any authenticated user.
    // Admin-only actions are guarded individually via [Authorize(Policy = "AdminRole")].
    [Authorize]
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly ILogger<UsersController> _logger;
        private readonly ApplicationDbContext _db;

        public UsersController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IMapper mapper, ILogger<UsersController> logger, ApplicationDbContext db)
        {
            _mapper = mapper;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }

        //GET: api/users
        [HttpGet]
        [Authorize(Policy = "AdminRole")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users.Include(u => u.Person).ToListAsync();
            var personIds = users.Where(u => u.PersonId.HasValue).Select(u => u.PersonId!.Value).Distinct().ToList();
            var discMap = await LoadPersonDiscriminatorsAsync(personIds);

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
                    PersonId = u.PersonId,
                    PersonName = u.Person?.FullName,
                    PersonUPI = u.Person?.UPI,
                    PersonType = u.PersonId.HasValue && discMap.TryGetValue(u.PersonId.Value, out var t) ? t : null,
                    Roles = roles.ToList()
                });
            }
            return Ok(result);
        }

        // Reads the EF-managed TPH Discriminator shadow column for the supplied Person rows.
        // Values are the entity names ("Student", "StaffDetails", "Parent").
        private async Task<Dictionary<int, string>> LoadPersonDiscriminatorsAsync(IReadOnlyCollection<int> personIds)
        {
            if (personIds.Count == 0) return new Dictionary<int, string>();
            var rows = await _db.Set<Person>()
                .AsNoTracking()
                .IgnoreQueryFilters()
                .Where(p => personIds.Contains(p.Id))
                .Select(p => new { p.Id, Discriminator = EF.Property<string>(p, "Discriminator") })
                .ToListAsync();
            return rows.ToDictionary(r => r.Id, r => r.Discriminator);
        }

        //GET: api/users/username
        [HttpGet("username")]
        [Authorize(Policy = "AdminRole")]
        public async Task<ActionResult<UserToReturn>> GetUser([FromBody] string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return NotFound();
            return Ok(_mapper.Map<UserToReturn>(user));
        }

        //GET: api/users/Id
        // Any authenticated user can fetch their own profile; only admins can fetch others.
        [HttpGet("{Id}")]
        [Authorize]
        public async Task<ActionResult<UserWithRolesDto>> GetUserById(int Id)
        {
            var isAdmin = User.IsInRole("Administrator") || User.IsInRole("SuperAdministrator");
            if (!isAdmin)
            {
                var usernameClaim = User.FindFirstValue("username") ?? User.Identity?.Name;
                var me = await _userManager.FindByNameAsync(usernameClaim ?? "");
                if (me == null || me.Id != Id) return Forbid();
            }

            var user = await _userManager.Users.Include(u => u.Person).FirstOrDefaultAsync(u => u.Id == Id);
            if (user == null) return NotFound();
            var roles = await _userManager.GetRolesAsync(user);
            var discMap = await LoadPersonDiscriminatorsAsync(user.PersonId.HasValue ? new[] { user.PersonId.Value } : Array.Empty<int>());
            return Ok(new UserWithRolesDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PersonId = user.PersonId,
                PersonName = user.Person?.FullName,
                PersonUPI = user.Person?.UPI,
                PersonType = user.PersonId.HasValue && discMap.TryGetValue(user.PersonId.Value, out var t) ? t : null,
                Roles = roles.ToList()
            });
        }

        //GET: api/users/userRoles/{id}
        [HttpGet("userRoles/{id}")]
        [Authorize(Policy = "AdminRole")]
        public async Task<IActionResult> GetUserRoles(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound();
            var roles = await _userManager.GetRolesAsync(user);
            return Ok(roles);
        }

        //GET: api/users/availablePersons
        //GET: api/users/availablePersons?includePersonId=42  (for the Edit case so the
        //  currently-linked Person stays in the dropdown of the row being edited)
        [HttpGet("availablePersons")]
        [Authorize(Policy = "AdminRole")]
        public async Task<IActionResult> GetAvailablePersons([FromQuery] int? includePersonId = null)
        {
            var takenIds = await _userManager.Users
                .Where(u => u.PersonId.HasValue)
                .Select(u => u.PersonId!.Value)
                .ToListAsync();

            var taken = new HashSet<int>(takenIds);
            if (includePersonId.HasValue) taken.Remove(includePersonId.Value);

            var persons = await _db.Set<Person>()
                .AsNoTracking()
                .Where(p => !taken.Contains(p.Id))
                .Select(p => new
                {
                    p.Id,
                    p.FullName,
                    p.UPI,
                    p.Email,
                    PersonType = EF.Property<string>(p, "Discriminator")
                })
                .OrderBy(p => p.FullName)
                .ToListAsync();

            return Ok(persons);
        }

        //POST: api/users
        [HttpPost]
        [Authorize(Policy = "AdminRole")]
        public async Task<ActionResult<UserToReturn>> PostUser(UserDTO user)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (user.PersonId.HasValue)
            {
                var personExists = await _db.Set<Person>().AnyAsync(p => p.Id == user.PersonId.Value);
                if (!personExists) return BadRequest(new { error = "Selected person does not exist." });

                var alreadyLinked = await _userManager.Users.AnyAsync(u => u.PersonId == user.PersonId);
                if (alreadyLinked) return BadRequest(new { error = "Selected person is already linked to another user." });
            }

            var userToSave = new AppUser
            {
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName ?? "",
                LastName = user.LastName ?? "",
                PersonId = user.PersonId,
                // Admin set the initial password, so force the user to change it on first login.
                MustChangePassword = true
            };

            var result = await _userManager.CreateAsync(userToSave, user.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);

            var retUser = _mapper.Map<UserToReturn>(userToSave);

            return CreatedAtAction("GetUserById", new { Id = userToSave.Id }, retUser);
        }

        //PUT: api/users/{id}
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminRole")]
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

                if (model.PersonId != user.PersonId)
                {
                    if (model.PersonId.HasValue)
                    {
                        var personExists = await _db.Set<Person>().AnyAsync(p => p.Id == model.PersonId.Value);
                        if (!personExists) return BadRequest(new { error = "Selected person does not exist." });

                        var alreadyLinked = await _userManager.Users.AnyAsync(u => u.PersonId == model.PersonId && u.Id != id);
                        if (alreadyLinked) return BadRequest(new { error = "Selected person is already linked to another user." });
                    }
                    user.PersonId = model.PersonId;
                }

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded) return BadRequest(result.Errors);

                if (!string.IsNullOrEmpty(model.Password))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var passResult = await _userManager.ResetPasswordAsync(user, token, model.Password);
                    if (!passResult.Succeeded) return BadRequest(passResult.Errors);

                    // Admin set a new password for the user, so force change on next login.
                    user.MustChangePassword = true;
                    await _userManager.UpdateAsync(user);
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
        [Authorize(Policy = "AdminRole")]
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
        [Authorize(Policy = "AdminRole")]
        public async Task<IActionResult> AddUserToRole(UserRoleDTO userRole)
        {
            try
            {
                // SuperAdministrator role can only be assigned by another SuperAdministrator
                if (string.Equals(userRole.Role, "SuperAdministrator", StringComparison.OrdinalIgnoreCase) && !User.IsInRole("SuperAdministrator"))
                    return StatusCode(StatusCodes.Status403Forbidden, new { error = "Only a SuperAdministrator can assign the SuperAdministrator role." });

                var user = await _userManager.FindByEmailAsync(userRole.Email);
                if (user == null) return BadRequest(new { error = "User details not found." });

                var roleExist = await _roleManager.RoleExistsAsync(userRole.Role);
                if (!roleExist) return BadRequest(new { error = "Role not found in the system." });

                IdentityResult result = await _userManager.AddToRoleAsync(user, userRole.Role);
                if (result.Succeeded) return Ok(new { result = $"User {user.Email} added to the {userRole.Role} role." });

                var errMsg = string.Join(" ", result.Errors.Select(e => e.Description));
                return BadRequest(new { error = string.IsNullOrEmpty(errMsg) ? "Failed to add role." : errMsg });
            }
            catch (Exception ex)
            {
                var errMessage = $"An error occurred while adding the user to the role.";
                _logger.LogError(ex, errMessage);
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = errMessage });
            }
        }

        //POST: api/users/removefromrole
        [HttpPost]
        [Route("removefromrole")]
        [Authorize(Policy = "AdminRole")]
        public async Task<IActionResult> RemoveUserFromRole(UserRoleDTO userRole)
        {
            try
            {
                // SuperAdministrator role can only be removed by another SuperAdministrator
                if (string.Equals(userRole.Role, "SuperAdministrator", StringComparison.OrdinalIgnoreCase) && !User.IsInRole("SuperAdministrator"))
                    return StatusCode(StatusCodes.Status403Forbidden, new { error = "Only a SuperAdministrator can remove the SuperAdministrator role." });

                var user = await _userManager.FindByEmailAsync(userRole.Email);
                if (user == null) return BadRequest(new { error = "User details not found!" });

                var roleExist = await _roleManager.RoleExistsAsync(userRole.Role);
                if (!roleExist) return BadRequest(new { error = "Role name not found!" });

                var isInRole = await _userManager.IsInRoleAsync(user, userRole.Role);
                if (!isInRole) return BadRequest(new { error = $"User is not in the '{userRole.Role}' role." });

                var result = await _userManager.RemoveFromRoleAsync(user, userRole.Role);

                if (result.Succeeded) return Ok(new { result = $"User {user.Email} removed from the {userRole.Role} role" });

                var errMsg = string.Join(" ", result.Errors.Select(e => e.Description));
                return BadRequest(new { error = string.IsNullOrEmpty(errMsg) ? "Failed to remove role." : errMsg });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing user from role.");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        //POST: api/users/reset-password
        [HttpPost("reset-password")]
        [Authorize(Policy = "AdminRole")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO model)
        {
            if (!ModelState.IsValid)
            {
                var msg = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(new { message = string.IsNullOrEmpty(msg) ? "Invalid request." : msg });
            }
            try
            {
                var user = await _userManager.FindByIdAsync(model.UserId.ToString());
                if (user == null) return NotFound(new { message = "User not found." });

                // Remove existing password (if any) and set the new one directly.
                if (await _userManager.HasPasswordAsync(user))
                {
                    var removeResult = await _userManager.RemovePasswordAsync(user);
                    if (!removeResult.Succeeded)
                    {
                        var errs = string.Join(" ", removeResult.Errors.Select(e => e.Description));
                        return BadRequest(new { message = string.IsNullOrEmpty(errs) ? "Failed to clear current password." : errs });
                    }
                }

                var addResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
                if (!addResult.Succeeded)
                {
                    var errs = string.Join(" ", addResult.Errors.Select(e => e.Description));
                    return BadRequest(new { message = string.IsNullOrEmpty(errs) ? "Failed to set new password." : errs });
                }

                // Admin-issued reset → force the user to change password on next login.
                user.MustChangePassword = true;
                await _userManager.UpdateAsync(user);

                return Ok(new { message = $"Password for {user.UserName} has been reset successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password.");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
