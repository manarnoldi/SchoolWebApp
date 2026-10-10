using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.API.Services.Communications;
using SchoolWebApp.Core.DTOs.Communications;
using SchoolWebApp.Core.Entities.Communications;

namespace SchoolWebApp.API.Controllers.Communications
{
    /// <summary>
    /// Message templates. The system ones are the standard message types; their
    /// wording, channel and on/off state can change but they cannot be deleted
    /// or re-coded.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessageTemplatesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public MessageTemplatesController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _db.MessageTemplates.AsNoTracking()
                .OrderByDescending(t => t.IsSystem).ThenBy(t => t.Name)
                .ToListAsync();
            return Ok(items.Select(ToDto));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var t = await _db.MessageTemplates.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return t == null ? NotFound() : Ok(ToDto(t));
        }

        [HttpGet("placeholders")]
        public IActionResult Placeholders(string? code) => Ok(MessagePlaceholders.For(code));

        [HttpPost]
        [Authorize(Roles = "Administrator,SuperAdministrator")]
        public async Task<IActionResult> Create(MessageTemplateDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Name)) return BadRequest("Enter the template name.");
            var t = new MessageTemplate
            {
                Name = model.Name.Trim(),
                Description = model.Description,
                IsSystem = false,
                Channel = model.Channel,
                IsActive = model.IsActive,
                SmsBody = model.SmsBody,
                EmailSubject = model.EmailSubject,
                EmailBody = model.EmailBody
            };
            _db.MessageTemplates.Add(t);
            await _db.SaveChangesAsync();
            return Ok(ToDto(t));
        }

        [HttpPut]
        [Authorize(Roles = "Administrator,SuperAdministrator")]
        public async Task<IActionResult> Update(MessageTemplateDto model)
        {
            var t = await _db.MessageTemplates.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (t == null) return NotFound();
            if (string.IsNullOrWhiteSpace(model.Name)) return BadRequest("Enter the template name.");
            if (t.IsSystem && model.IsActive && model.Channel == MessageChannel.None)
                return BadRequest("Choose SMS, email or both for an active message type.");
            if (t.IsSystem && model.Channel.HasFlag(MessageChannel.Sms) && model.Channel != MessageChannel.None && string.IsNullOrWhiteSpace(model.SmsBody))
                return BadRequest("Enter the SMS text, or send this message type by email only.");
            if (t.IsSystem && model.Channel.HasFlag(MessageChannel.Email) && model.Channel != MessageChannel.None
                && (string.IsNullOrWhiteSpace(model.EmailSubject) || string.IsNullOrWhiteSpace(model.EmailBody)))
                return BadRequest("Enter the email subject and body, or send this message type by SMS only.");

            t.Name = model.Name.Trim();
            t.Description = model.Description;
            t.Channel = model.Channel;
            t.IsActive = model.IsActive;
            t.SmsBody = model.SmsBody;
            t.EmailSubject = model.EmailSubject;
            t.EmailBody = model.EmailBody;
            await _db.SaveChangesAsync();
            return Ok(ToDto(t));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator,SuperAdministrator")]
        public async Task<IActionResult> Delete(int id)
        {
            var t = await _db.MessageTemplates.FirstOrDefaultAsync(x => x.Id == id);
            if (t == null) return NotFound();
            if (t.IsSystem) return BadRequest("A standard message type cannot be deleted; switch it off instead.");
            _db.MessageTemplates.Remove(t);
            await _db.SaveChangesAsync();
            return Ok();
        }

        private static MessageTemplateDto ToDto(MessageTemplate t) => new()
        {
            Id = t.Id,
            Code = t.Code,
            Name = t.Name,
            Description = t.Description,
            IsSystem = t.IsSystem,
            Channel = t.Channel,
            IsActive = t.IsActive,
            SmsBody = t.SmsBody,
            EmailSubject = t.EmailSubject,
            EmailBody = t.EmailBody,
            Modified = t.Modified,
            ModifiedBy = t.ModifiedBy
        };
    }
}
