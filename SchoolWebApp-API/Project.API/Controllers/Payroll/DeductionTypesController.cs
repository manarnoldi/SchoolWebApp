using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs.Payroll;
using SchoolWebApp.Core.Entities.Payroll;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.API.Controllers.Payroll
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DeductionTypesController : ControllerBase
    {
        private readonly ILogger<DeductionTypesController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DeductionTypesController(ILogger<DeductionTypesController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger; _unitOfWork = unitOfWork; _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _unitOfWork.DeductionTypes.Find();
            return Ok(_mapper.Map<List<DeductionTypeDto>>(items));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _unitOfWork.DeductionTypes.GetById(id);
            if (item == null) return NotFound();
            return Ok(_mapper.Map<DeductionTypeDto>(item));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateDeductionTypeDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            // Code is unique in the database - report the clash rather than let the
            // insert fail with a raw constraint violation.
            if (await _unitOfWork.DeductionTypes.ItemExistsAsync(d => d.Code == model.Code))
                return Conflict(new { message = $"A deduction type with the code '{model.Code}' already exists." });
            var item = _mapper.Map<DeductionType>(model);
            _unitOfWork.DeductionTypes.Create(item);
            await _unitOfWork.SaveChangesAsync();
            return Ok(_mapper.Map<DeductionTypeDto>(item));
        }

        [HttpPut]
        public async Task<IActionResult> Update(DeductionTypeDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var existing = await _unitOfWork.DeductionTypes.GetById(model.Id);
            if (existing == null) return NotFound();
            if (await _unitOfWork.DeductionTypes.ItemExistsAsync(d => d.Code == model.Code && d.Id != model.Id))
                return Conflict(new { message = $"Another deduction type already uses the code '{model.Code}'." });

            // Copy the edit onto the record loaded from the database. Mapping the DTO to
            // a new entity instead lost the Id - the only map from a DTO is the create
            // map, which has none - so EF Core saw a new record and INSERTed a copy.
            _mapper.Map<CreateDeductionTypeDto, DeductionType>(model, existing);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _unitOfWork.DeductionTypes.GetById(id);
            if (item == null) return NotFound();
            _unitOfWork.DeductionTypes.Delete(item);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }
    }
}
