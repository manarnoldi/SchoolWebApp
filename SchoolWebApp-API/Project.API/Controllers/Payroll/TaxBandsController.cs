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
    public class TaxBandsController : ControllerBase
    {
        private readonly ILogger<TaxBandsController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TaxBandsController(ILogger<TaxBandsController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger; _unitOfWork = unitOfWork; _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _unitOfWork.TaxBands.Find();
            return Ok(_mapper.Map<List<TaxBandDto>>(items));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _unitOfWork.TaxBands.GetById(id);
            if (item == null) return NotFound();
            return Ok(_mapper.Map<TaxBandDto>(item));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTaxBandDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var item = _mapper.Map<TaxBand>(model);
            _unitOfWork.TaxBands.Create(item);
            await _unitOfWork.SaveChangesAsync();
            return Ok(_mapper.Map<TaxBandDto>(item));
        }

        [HttpPut]
        public async Task<IActionResult> Update(TaxBandDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var existing = await _unitOfWork.TaxBands.GetById(model.Id);
            if (existing == null) return NotFound();

            // Copy the edit onto the record loaded from the database. Mapping the DTO to
            // a new entity instead lost the Id - the only map from a DTO is the create
            // map, which has none - so EF Core INSERTed a duplicate band rather than
            // updating this one. With no unique key to stop it, a doubled band would
            // have taxed that slice of income twice.
            _mapper.Map<CreateTaxBandDto, TaxBand>(model, existing);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _unitOfWork.TaxBands.GetById(id);
            if (item == null) return NotFound();
            _unitOfWork.TaxBands.Delete(item);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }
    }
}
