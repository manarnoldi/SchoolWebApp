using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.DTOs.Sponsorships;
using SchoolWebApp.Core.Entities.Sponsorships;
using SchoolWebApp.API.Licensing;
using SchoolWebApp.API.Security;

namespace SchoolWebApp.API.Controllers.Sponsorships
{
    [Authorize]
    [Route("api/sponsors")]
    [ApiController]
    [RequiresModule(AppModules.Finance)]
    [RequiresMenu(MenuGroup.Finance)]
    public class SponsorsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        public SponsorsController(ApplicationDbContext db, IMapper mapper) { _db = db; _mapper = mapper; }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _db.Sponsors
                .Include(s => s.ReceivableAccount)
                .OrderBy(s => s.Name)
                .ToListAsync();
            return Ok(_mapper.Map<List<SponsorDto>>(list));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _db.Sponsors.Include(s => s.ReceivableAccount).FirstOrDefaultAsync(s => s.Id == id);
            if (item == null) return NotFound();
            return Ok(_mapper.Map<SponsorDto>(item));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSponsorDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var item = _mapper.Map<Sponsor>(model);
            _db.Sponsors.Add(item);
            await _db.SaveChangesAsync();
            return Ok(_mapper.Map<SponsorDto>(item));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateSponsorDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var item = await _db.Sponsors.FirstOrDefaultAsync(s => s.Id == id);
            if (item == null) return NotFound();
            item.Name = model.Name;
            item.Description = model.Description;
            item.SponsorType = model.SponsorType;
            item.ContactName = model.ContactName;
            item.Email = model.Email;
            item.Phone = model.Phone;
            item.Address = model.Address;
            item.ReceivableAccountId = model.ReceivableAccountId;
            item.IsActive = model.IsActive;
            await _db.SaveChangesAsync();
            return Ok(_mapper.Map<SponsorDto>(item));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _db.Sponsors.FindAsync(id);
            if (item == null) return NotFound();
            var inUse = await _db.Sponsorships.AnyAsync(s => s.SponsorId == id)
                || await _db.SponsorPayments.AnyAsync(p => p.SponsorId == id);
            if (inUse) return BadRequest(new { message = "Sponsor has sponsorships or payments and cannot be deleted." });
            _db.Sponsors.Remove(item);
            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}
