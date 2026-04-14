using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.Entities.Identity;

namespace SchoolWebApp.API.Controllers
{
    [Authorize(Policy = "AdminRole")]
    [ApiController]
    [Route("api/roles")]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly ILogger<RolesController> _logger;

        public RolesController(IMapper mapper, RoleManager<AppRole> roleManager, ILogger<RolesController> logger)
        {
            _mapper = mapper;
            _roleManager = roleManager;
            _logger = logger;
        }

        //GET: api/roles
        [HttpGet]
        public IActionResult GetAllRoles()
        {
            var roles = _roleManager.Roles.ToList();
            return Ok(_mapper.Map<IEnumerable<RoleToReturn>>(roles));
        }

        //GET: api/roles/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById(int id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null) return NotFound();
            return Ok(_mapper.Map<RoleToReturn>(role));
        }

        //POST: api/roles
        [HttpPost]
        public async Task<IActionResult> PostRole(RoleDTO role)
        {
            var roleExist = await _roleManager.RoleExistsAsync(role.Name);
            if (roleExist) return BadRequest(new { error = "Role already registered." });

            var roleToSave = new AppRole { Name = role.Name, NormalizedName = role.Name.ToUpper() };
            var roleResult = await _roleManager.CreateAsync(roleToSave);

            if (roleResult.Succeeded) return Ok(_mapper.Map<RoleToReturn>(roleToSave));
            else return BadRequest(new { error = "An error occurred while adding the role." });
        }

        //PUT: api/roles/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(int id, RoleDTO model)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(id.ToString());
                if (role == null) return NotFound();

                role.Name = model.Name;
                role.NormalizedName = model.Name.ToUpper();
                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded) return Ok(_mapper.Map<RoleToReturn>(role));
                return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role.");
                return StatusCode(500, new { message = "Error updating role." });
            }
        }

        //DELETE: api/roles/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(id.ToString());
                if (role == null) return NotFound();

                var result = await _roleManager.DeleteAsync(role);
                if (result.Succeeded) return Ok(new { message = $"Role {role.Name} deleted successfully." });
                return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting role.");
                return StatusCode(500, new { message = "Error deleting role." });
            }
        }
    }
}