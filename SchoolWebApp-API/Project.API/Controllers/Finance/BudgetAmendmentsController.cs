using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs.Finance.BudgetAmendment;
using SchoolWebApp.Core.Entities.Approvals;
using SchoolWebApp.Core.Entities.Finance;
using SchoolWebApp.Core.Helpers;
using SchoolWebApp.Core.Interfaces.IRepositories;
using System.Security.Claims;

namespace SchoolWebApp.API.Controllers.Finance
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BudgetAmendmentsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BudgetAmendmentsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork; _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _unitOfWork.BudgetAmendments.Find(includeProperties: "Lines,ApprovedBy");
            return Ok(_mapper.Map<List<BudgetAmendmentDto>>(items));
        }

        [HttpGet("byBudgetId/{budgetId}")]
        public async Task<IActionResult> GetByBudgetId(int budgetId)
        {
            var items = await _unitOfWork.BudgetAmendments.GetByBudgetId(budgetId);
            return Ok(_mapper.Map<List<BudgetAmendmentDto>>(items));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _unitOfWork.BudgetAmendments.GetByIdWithLines(id);
            if (item == null) return NotFound();
            return Ok(_mapper.Map<BudgetAmendmentDto>(item));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBudgetAmendmentDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var amendment = new BudgetAmendment
            {
                BudgetId = model.BudgetId,
                ReferenceNumber = string.IsNullOrWhiteSpace(model.ReferenceNumber)
                    ? $"AMD-{DateTime.Now:yyyyMMddHHmmss}"
                    : model.ReferenceNumber,
                AmendmentDate = model.AmendmentDate == default ? DateTime.Now : model.AmendmentDate,
                Reason = model.Reason,
                Status = BudgetAmendmentStatus.Pending
            };
            foreach (var l in model.Lines)
            {
                amendment.Lines.Add(new BudgetAmendmentLine
                {
                    AccountId = l.AccountId,
                    PreviousAmount = l.PreviousAmount,
                    NewAmount = l.NewAmount,
                    Delta = l.NewAmount - l.PreviousAmount,
                    Notes = l.Notes
                });
            }
            _unitOfWork.BudgetAmendments.Create(amendment);
            await _unitOfWork.SaveChangesAsync();
            var saved = await _unitOfWork.BudgetAmendments.GetByIdWithLines(amendment.Id);
            return Ok(_mapper.Map<BudgetAmendmentDto>(saved));
        }

        // Approve / reject
        [HttpPost("{id}/approve")]
        public async Task<IActionResult> Approve(int id, [FromQuery] int? approvedById)
        {
            var item = await _unitOfWork.BudgetAmendments.GetByIdWithLines(id);
            if (item == null) return NotFound();
            // Create any net-new budget lines referenced in the amendment (PreviousAmount = 0)
            var budget = await _unitOfWork.Budgets.GetByIdWithLines(item.BudgetId);
            if (budget != null)
            {
                var existingAccountIds = budget.Lines.Select(x => x.AccountId).ToHashSet();
                foreach (var al in item.Lines)
                {
                    if (!existingAccountIds.Contains(al.AccountId))
                    {
                        _unitOfWork.BudgetLines.Create(new SchoolWebApp.Core.Entities.Finance.BudgetLine
                        {
                            BudgetId = budget.Id,
                            AccountId = al.AccountId,
                            BudgetedAmount = 0,
                            Notes = al.Notes
                        });
                        existingAccountIds.Add(al.AccountId);
                    }
                }
            }
            item.Status = BudgetAmendmentStatus.Approved;
            item.ApprovedById = approvedById;
            item.ApprovedDate = DateTime.Now;
            _unitOfWork.BudgetAmendments.Update(item);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> Reject(int id, [FromQuery] int? approvedById)
        {
            var item = await _unitOfWork.BudgetAmendments.GetById(id);
            if (item == null) return NotFound();
            item.Status = BudgetAmendmentStatus.Rejected;
            item.ApprovedById = approvedById;
            item.ApprovedDate = DateTime.Now;
            _unitOfWork.BudgetAmendments.Update(item);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _unitOfWork.BudgetAmendments.GetByIdWithLines(id);
            if (item == null) return NotFound();
            if (await ApprovalLockHelper.IsLockedAsync(_unitOfWork, "BudgetAmendment", id))
                return BadRequest(new { message = "This amendment is locked by an active approval workflow." });
            int.TryParse(User.FindFirstValue("userid"), out var _uid);
            if (await ApprovalLockHelper.IsApproverAsync(_unitOfWork, "BudgetAmendment", id, _uid))
                return BadRequest(new { message = "Approvers cannot edit or delete submitted items. Use the approval workflow." });
            foreach (var l in item.Lines.ToList())
                _unitOfWork.BudgetAmendmentLines.Delete(l);
            _unitOfWork.BudgetAmendments.Delete(item);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }
    }
}
