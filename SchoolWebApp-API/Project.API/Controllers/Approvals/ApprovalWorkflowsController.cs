using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.DTOs.Approvals;
using SchoolWebApp.Core.Entities.Approvals;
using SchoolWebApp.Core.Entities.Identity;

namespace SchoolWebApp.API.Controllers.Approvals
{
    [Authorize]
    [Route("api/approvalWorkflows")]
    [ApiController]
    public class ApprovalWorkflowsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public ApprovalWorkflowsController(ApplicationDbContext db, IMapper mapper,
            RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
        {
            _db = db;
            _mapper = mapper;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _db.ApprovalWorkflows
                .Include(w => w.Steps.OrderBy(s => s.Rank))
                .ThenInclude(s => s.Role)
                .OrderBy(w => w.Name)
                .ToListAsync();
            return Ok(_mapper.Map<List<ApprovalWorkflowDto>>(list));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _db.ApprovalWorkflows
                .Include(w => w.Steps.OrderBy(s => s.Rank))
                .ThenInclude(s => s.Role)
                .FirstOrDefaultAsync(w => w.Id == id);
            if (item == null) return NotFound();
            return Ok(_mapper.Map<ApprovalWorkflowDto>(item));
        }

        [HttpGet("byFormKey/{formKey}")]
        public async Task<IActionResult> GetByFormKey(string formKey)
        {
            var item = await _db.ApprovalWorkflows
                .Include(w => w.Steps.OrderBy(s => s.Rank))
                .ThenInclude(s => s.Role)
                .FirstOrDefaultAsync(w => w.FormKey == formKey && w.IsActive);
            if (item == null) return NotFound(new { message = $"No active workflow for form '{formKey}'." });
            return Ok(_mapper.Map<ApprovalWorkflowDto>(item));
        }

        [HttpPost]
        [Authorize(Policy = "AdminRole")]
        public async Task<IActionResult> Create([FromBody] CreateApprovalWorkflowDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var exists = await _db.ApprovalWorkflows.AnyAsync(w => w.FormKey == model.FormKey && w.IsActive);
            if (exists)
                return Conflict(new { message = $"An active workflow already exists for form '{model.FormKey}'." });

            var wf = new ApprovalWorkflow
            {
                Name = model.Name,
                FormKey = model.FormKey,
                Description = model.Description,
                IsMakerChecker = model.IsMakerChecker,
                IsActive = model.IsActive,
                Steps = model.Steps.OrderBy(s => s.Rank).Select(s => new ApprovalWorkflowStep
                {
                    Rank = s.Rank,
                    Name = s.Name,
                    RoleId = s.RoleId,
                    IsFinal = s.IsFinal,
                    NotifyNextApprover = s.NotifyNextApprover,
                    NotifyPreviousApprover = s.NotifyPreviousApprover,
                    NotifyApplicant = s.NotifyApplicant
                }).ToList()
            };

            _db.ApprovalWorkflows.Add(wf);
            await _db.SaveChangesAsync();
            return Ok(_mapper.Map<ApprovalWorkflowDto>(wf));
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminRole")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateApprovalWorkflowDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var wf = await _db.ApprovalWorkflows.Include(w => w.Steps).FirstOrDefaultAsync(w => w.Id == id);
            if (wf == null) return NotFound();

            wf.Name = model.Name;
            wf.FormKey = model.FormKey;
            wf.Description = model.Description;
            wf.IsMakerChecker = model.IsMakerChecker;
            wf.IsActive = model.IsActive;

            // Replace steps
            _db.ApprovalWorkflowSteps.RemoveRange(wf.Steps);
            wf.Steps = model.Steps.OrderBy(s => s.Rank).Select(s => new ApprovalWorkflowStep
            {
                Rank = s.Rank,
                Name = s.Name,
                RoleId = s.RoleId,
                IsFinal = s.IsFinal,
                NotifyNextApprover = s.NotifyNextApprover,
                NotifyPreviousApprover = s.NotifyPreviousApprover,
                NotifyApplicant = s.NotifyApplicant
            }).ToList();

            await _db.SaveChangesAsync();
            return Ok(_mapper.Map<ApprovalWorkflowDto>(wf));
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminRole")]
        public async Task<IActionResult> Delete(int id)
        {
            var wf = await _db.ApprovalWorkflows.Include(w => w.Steps).FirstOrDefaultAsync(w => w.Id == id);
            if (wf == null) return NotFound();
            var inUse = await _db.ApprovalRequests.AnyAsync(r => r.ApprovalWorkflowId == id);
            if (inUse)
                return BadRequest(new { message = "This workflow is referenced by approval requests and cannot be deleted." });
            // Cascade delete is disabled at the DB level — remove owned steps explicitly.
            _db.ApprovalWorkflowSteps.RemoveRange(wf.Steps);
            _db.ApprovalWorkflows.Remove(wf);
            await _db.SaveChangesAsync();
            return Ok();
        }

        // GET api/approvalWorkflows/usersInRole/{roleId}
        [HttpGet("usersInRole/{roleId}")]
        public async Task<IActionResult> UsersInRole(int roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null) return NotFound(new { message = "Role not found." });

            var users = await _userManager.GetUsersInRoleAsync(role.Name ?? "");
            var result = users.Select(u => new UserInRoleDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName
            }).OrderBy(u => u.FirstName).ToList();
            return Ok(result);
        }
    }
}
