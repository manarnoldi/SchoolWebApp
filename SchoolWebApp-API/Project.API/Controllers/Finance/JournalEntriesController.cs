using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs.Finance.JournalEntry;
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
    public class JournalEntriesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public JournalEntriesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork; _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _unitOfWork.JournalEntries.Find(includeProperties: "Lines.Account");
            return Ok(_mapper.Map<List<JournalEntryDto>>(items));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _unitOfWork.JournalEntries.GetByIdWithLines(id);
            if (item == null) return NotFound();
            return Ok(_mapper.Map<JournalEntryDto>(item));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateJournalEntryDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var totalDebit = model.Lines.Sum(l => l.Debit);
            var totalCredit = model.Lines.Sum(l => l.Credit);
            if (Math.Round(totalDebit, 2) != Math.Round(totalCredit, 2))
                return BadRequest(new { message = $"Debits ({totalDebit}) must equal credits ({totalCredit})." });

            var entry = _mapper.Map<JournalEntry>(model);
            if (string.IsNullOrEmpty(entry.ReferenceNumber))
                entry.ReferenceNumber = $"JV-{DateTime.Now:yyyyMMddHHmmssfff}";
            entry.Status = JournalEntryStatus.Draft;
            entry.IsPosted = false;
            _unitOfWork.JournalEntries.Create(entry);
            await _unitOfWork.SaveChangesAsync();
            return Ok(_mapper.Map<JournalEntryDto>(await _unitOfWork.JournalEntries.GetByIdWithLines(entry.Id)));
        }

        [HttpPost("{id}/submit")]
        public async Task<IActionResult> Submit(int id)
        {
            var item = await _unitOfWork.JournalEntries.GetByIdWithLines(id);
            if (item == null) return NotFound();
            if (item.Status != JournalEntryStatus.Draft && item.Status != JournalEntryStatus.Rejected)
                return BadRequest("Only draft or rejected entries can be submitted.");
            if (!item.Lines.Any())
                return BadRequest("Cannot submit an entry with no lines.");
            var totalDebit = item.Lines.Sum(l => l.Debit);
            var totalCredit = item.Lines.Sum(l => l.Credit);
            if (Math.Round(totalDebit, 2) != Math.Round(totalCredit, 2))
                return BadRequest("Entry is not balanced.");
            item.Status = JournalEntryStatus.Submitted;
            _unitOfWork.JournalEntries.Update(item);
            await _unitOfWork.SaveChangesAsync();
            return Ok(new { message = "Journal entry submitted for approval." });
        }

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            var item = await _unitOfWork.JournalEntries.GetById(id);
            if (item == null) return NotFound();
            if (item.Status != JournalEntryStatus.Submitted)
                return BadRequest("Only submitted entries can be approved.");
            item.Status = JournalEntryStatus.Approved;
            item.IsPosted = true;
            _unitOfWork.JournalEntries.Update(item);
            await _unitOfWork.SaveChangesAsync();
            return Ok(new { message = "Journal entry approved and posted." });
        }

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> Reject(int id, [FromBody] RejectJournalDto model)
        {
            var item = await _unitOfWork.JournalEntries.GetById(id);
            if (item == null) return NotFound();
            if (item.Status != JournalEntryStatus.Submitted)
                return BadRequest("Only submitted entries can be rejected.");
            item.Status = JournalEntryStatus.Rejected;
            item.Description = (item.Description ?? "") + $" [Rejected: {model.Reason}]";
            _unitOfWork.JournalEntries.Update(item);
            await _unitOfWork.SaveChangesAsync();
            return Ok(new { message = "Journal entry rejected." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _unitOfWork.JournalEntries.GetByIdWithLines(id);
            if (item == null) return NotFound();
            if (item.Status == JournalEntryStatus.Approved)
                return BadRequest("Cannot delete an approved journal entry.");
            if (await ApprovalLockHelper.IsLockedAsync(_unitOfWork, "JournalEntry", id))
                return BadRequest(new { message = "This journal entry is locked by an active approval workflow." });
            int.TryParse(User.FindFirstValue("userid"), out var _uid);
            if (await ApprovalLockHelper.IsApproverAsync(_unitOfWork, "JournalEntry", id, _uid))
                return BadRequest(new { message = "Approvers cannot edit or delete submitted items. Use the approval workflow." });
            foreach (var l in item.Lines.ToList())
                _unitOfWork.JournalLines.Delete(l);
            _unitOfWork.JournalEntries.Delete(item);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }
    }
}
