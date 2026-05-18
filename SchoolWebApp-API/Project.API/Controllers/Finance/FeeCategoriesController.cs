using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs.Finance.FeeCategory;
using SchoolWebApp.Core.Entities.Finance;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.API.Controllers.Finance
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FeeCategoriesController : ControllerBase
    {
        private readonly ILogger<FeeCategoriesController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FeeCategoriesController(ILogger<FeeCategoriesController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger; _unitOfWork = unitOfWork; _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _unitOfWork.FeeCategories.Find(includeProperties: "IncomeAccount");
            return Ok(_mapper.Map<List<FeeCategoryDto>>(items));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _unitOfWork.FeeCategories.GetById(id, includeProperties: "IncomeAccount");
            if (item == null) return NotFound();
            return Ok(_mapper.Map<FeeCategoryDto>(item));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateFeeCategoryDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var item = _mapper.Map<FeeCategory>(model);
            _unitOfWork.FeeCategories.Create(item);
            await _unitOfWork.SaveChangesAsync();
            return Ok(_mapper.Map<FeeCategoryDto>(item));
        }

        [HttpPut]
        public async Task<IActionResult> Update(FeeCategoryDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var exists = await _unitOfWork.FeeCategories.ItemExistsAsync(a => a.Id == model.Id);
            if (!exists) return NotFound();
            var item = _mapper.Map<FeeCategory>(model);
            _unitOfWork.FeeCategories.Update(item);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _unitOfWork.FeeCategories.GetById(id);
            if (item == null) return NotFound();
            _unitOfWork.FeeCategories.Delete(item);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }
    }
}
