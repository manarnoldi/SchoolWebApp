using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs.Finance.FeeStructure;
using SchoolWebApp.Core.Entities.Finance;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.API.Controllers.Finance
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FeeStructuresController : ControllerBase
    {
        private readonly ILogger<FeeStructuresController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FeeStructuresController(ILogger<FeeStructuresController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger; _unitOfWork = unitOfWork; _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _unitOfWork.FeeStructures.Find(includeProperties: "AcademicYear,Session,LearningLevel,Items.FeeCategory");
            return Ok(_mapper.Map<List<FeeStructureDto>>(items));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _unitOfWork.FeeStructures.GetByIdWithItems(id);
            if (item == null) return NotFound();
            return Ok(_mapper.Map<FeeStructureDto>(item));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateFeeStructureDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var item = _mapper.Map<FeeStructure>(model);
            _unitOfWork.FeeStructures.Create(item);
            await _unitOfWork.SaveChangesAsync();
            return Ok(_mapper.Map<FeeStructureDto>(await _unitOfWork.FeeStructures.GetByIdWithItems(item.Id)));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CreateFeeStructureDto model)
        {
            var existing = await _unitOfWork.FeeStructures.GetByIdWithItems(id);
            if (existing == null) return NotFound();

            existing.Name = model.Name;
            existing.AcademicYearId = model.AcademicYearId;
            existing.SessionId = model.SessionId;
            existing.LearningLevelId = model.LearningLevelId;
            existing.Description = model.Description;
            existing.IsActive = model.IsActive;

            // Remove deleted items
            var keepIds = model.Items.Where(i => i.Id.HasValue).Select(i => i.Id!.Value).ToHashSet();
            var toDelete = existing.Items.Where(i => !keepIds.Contains(i.Id)).ToList();
            foreach (var del in toDelete) _unitOfWork.FeeStructureItems.Delete(del);

            // Upsert items
            foreach (var item in model.Items)
            {
                if (item.Id.HasValue && item.Id.Value > 0)
                {
                    var existingItem = existing.Items.FirstOrDefault(i => i.Id == item.Id.Value);
                    if (existingItem != null)
                    {
                        existingItem.FeeCategoryId = item.FeeCategoryId;
                        existingItem.Amount = item.Amount;
                        existingItem.IsMandatory = item.IsMandatory;
                    }
                }
                else
                {
                    existing.Items.Add(new FeeStructureItem
                    {
                        FeeCategoryId = item.FeeCategoryId,
                        Amount = item.Amount,
                        IsMandatory = item.IsMandatory
                    });
                }
            }

            _unitOfWork.FeeStructures.Update(existing);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _unitOfWork.FeeStructures.GetById(id);
            if (item == null) return NotFound();
            _unitOfWork.FeeStructures.Delete(item);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }
    }
}
