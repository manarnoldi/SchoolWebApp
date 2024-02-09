using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.Entities.Identity;

namespace SchoolSoftWebApi.Controllers
{
    /// <summary>
    /// A class that manages roles. It requires a user with an authorization policy AdminRole
    /// </summary>
    [Authorize(Policy = "AdminRole")]
    [ApiController]
    [Route("api/roles")]
    public class RolesController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        /// <summary>
        /// The class constructor that injects the dependent services used in the class
        /// </summary>
        /// <param name="userManager">The user manager service</param>
        /// <param name="mapper">The automapper service</param>
        /// <param name="roleManager">The role manager service</param>
        public RolesController(UserManager<AppUser> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager)
        {
            _mapper = mapper;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        /// <summary>
        /// A method to get all roles from the system
        /// </summary>
        /// <returns>Returns all the roles</returns>
        //GET api/rols/post
        [HttpGet]
        public IActionResult GetAllRoles()
        {
            var roles = _roleManager.Roles.ToList();
            return Ok(roles);
        }

        //POST: api/roles/post
        [Route("post")]
        [HttpPost]
        public async Task<IActionResult> CreateRole(RoleDTO role)
        {
            var roleExist = await _roleManager.RoleExistsAsync(role.RoleName);

            if (roleExist) return BadRequest(new { error = "Role already registered." });

            var roleResult = await _roleManager.CreateAsync(new IdentityRole(role.RoleName));

            if (roleResult.Succeeded) return Ok(new { result = $"Role added successfully" });
            else return BadRequest(new { error = $"An error occured while adding the role." });
        }
    }
}