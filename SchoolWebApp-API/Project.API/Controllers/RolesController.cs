using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.Entities.Identity;

namespace SchoolWebApp.API.Controllers
{
    /// <summary>
    /// A class that manages roles. It requires a user with an authorization policy AdminRole
    /// </summary>
    [Authorize(Policy = "AdminRole")]
    [ApiController]
    [Route("api/roles")]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IMapper _mapper;

        /// <summary>
        /// The class constructor that injects the dependent services used in the class
        /// </summary>
        /// <param name="mapper">The automapper service</param>
        /// <param name="roleManager">The role manager service</param>
        public RolesController(IMapper mapper, RoleManager<AppRole> roleManager)
        {
            _mapper = mapper;
            _roleManager = roleManager;
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
            return Ok(_mapper.Map<IEnumerable<RoleToReturn>>(roles));
        }
        // public async Task<ActionResult<UserToReturn>> PostUser(UserDTO user)

        //POST: api/roles/
        [HttpPost]
        public async Task<IActionResult> PostRole(RoleDTO role)
        {
            var roleExist = await _roleManager.RoleExistsAsync(role.Name);

            if (roleExist) return BadRequest(new { error = "Role already registered." });

            var roleToSave = new AppRole { Name = role.Name, NormalizedName = role.Name.ToUpper() };

            var roleResult = await _roleManager.CreateAsync(roleToSave);

            if (roleResult.Succeeded) return Ok(new { result = $"Role added {role.Name} successfully" });
            else return BadRequest(new { error = $"An error occured while adding the role." });
        }
    }
}