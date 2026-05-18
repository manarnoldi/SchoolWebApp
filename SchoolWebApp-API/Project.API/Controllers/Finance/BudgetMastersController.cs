using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs.Finance.BudgetMaster;
using SchoolWebApp.Core.Entities.Finance;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.API.Controllers.Finance
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BudgetMastersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BudgetMastersController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork; _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _unitOfWork.BudgetMasters.Find(includeProperties: "AcademicYear,Budgets");
            return Ok(_mapper.Map<List<BudgetMasterDto>>(items));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _unitOfWork.BudgetMasters.GetById(id, includeProperties: "AcademicYear,Budgets");
            if (item == null) return NotFound();
            return Ok(_mapper.Map<BudgetMasterDto>(item));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBudgetMasterDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var item = _mapper.Map<BudgetMaster>(model);

            // Auto-generate code using BudgetMasterCodePrefix setting (default "BUD")
            if (string.IsNullOrWhiteSpace(item.Code))
            {
                var prefixSetting = (await _unitOfWork.Repository<SchoolWebApp.Core.Entities.Settings.GlobalSetting>()
                    .Find(s => s.Module == "General" && s.SettingKey == "BudgetMasterCodePrefix")).FirstOrDefault();
                var prefix = !string.IsNullOrWhiteSpace(prefixSetting?.SettingValue) ? prefixSetting!.SettingValue!.Trim() : "BUD";
                var all = await _unitOfWork.BudgetMasters.Find();
                var maxSeq = 0;
                foreach (var m in all)
                {
                    if (string.IsNullOrEmpty(m.Code) || !m.Code.StartsWith(prefix)) continue;
                    var tail = m.Code.Substring(prefix.Length);
                    if (int.TryParse(tail, out var n) && n > maxSeq) maxSeq = n;
                }
                item.Code = $"{prefix}{(maxSeq + 1).ToString("D3")}";
            }

            _unitOfWork.BudgetMasters.Create(item);
            await _unitOfWork.SaveChangesAsync();
            return Ok(_mapper.Map<BudgetMasterDto>(item));
        }

        [HttpPut]
        public async Task<IActionResult> Update(BudgetMasterDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var existing = await _unitOfWork.BudgetMasters.GetById(model.Id);
            if (existing == null) return NotFound();
            existing.Name = model.Name;
            existing.Description = model.Description;
            existing.AcademicYearId = model.AcademicYearId;
            existing.StartDate = model.StartDate;
            existing.EndDate = model.EndDate;
            existing.Status = model.Status;
            _unitOfWork.BudgetMasters.Update(existing);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _unitOfWork.BudgetMasters.GetById(id);
            if (item == null) return NotFound();
            _unitOfWork.BudgetMasters.Delete(item);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }
    }
}
