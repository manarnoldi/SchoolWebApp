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
    public class EarningTypesController : ControllerBase
    {
        private readonly ILogger<EarningTypesController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EarningTypesController(ILogger<EarningTypesController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger; _unitOfWork = unitOfWork; _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _unitOfWork.EarningTypes.Find();
            return Ok(_mapper.Map<List<EarningTypeDto>>(items));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _unitOfWork.EarningTypes.GetById(id);
            if (item == null) return NotFound();
            return Ok(_mapper.Map<EarningTypeDto>(item));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateEarningTypeDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            // Code is unique in the database - report the clash rather than let the
            // insert fail with a raw constraint violation.
            if (await _unitOfWork.EarningTypes.ItemExistsAsync(e => e.Code == model.Code))
                return Conflict(new { message = $"An earning type with the code '{model.Code}' already exists." });
            var item = _mapper.Map<EarningType>(model);
            _unitOfWork.EarningTypes.Create(item);
            await _unitOfWork.SaveChangesAsync();
            return Ok(_mapper.Map<EarningTypeDto>(item));
        }

        [HttpPut]
        public async Task<IActionResult> Update(EarningTypeDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var existing = await _unitOfWork.EarningTypes.GetById(model.Id);
            if (existing == null) return NotFound();
            if (await _unitOfWork.EarningTypes.ItemExistsAsync(e => e.Code == model.Code && e.Id != model.Id))
                return Conflict(new { message = $"Another earning type already uses the code '{model.Code}'." });

            // Copy the edit onto the record loaded from the database. Mapping the DTO to
            // a new entity instead lost the Id - the only map from a DTO is the create
            // map, which has none - so EF Core saw a new record and INSERTed a copy.
            _mapper.Map<CreateEarningTypeDto, EarningType>(model, existing);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _unitOfWork.EarningTypes.GetById(id);
            if (item == null) return NotFound();
            _unitOfWork.EarningTypes.Delete(item);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }
    }
}
