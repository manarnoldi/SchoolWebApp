using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs.Finance.Account;
using SchoolWebApp.Core.Entities.Finance;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.API.Controllers.Finance
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountsController : ControllerBase
    {
        private readonly ILogger<AccountsController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AccountsController(ILogger<AccountsController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var accounts = await _unitOfWork.Accounts.Find(includeProperties: "ParentAccount");
                return Ok(_mapper.Map<List<AccountDto>>(accounts));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching accounts.");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var account = await _unitOfWork.Accounts.GetById(id, includeProperties: "ParentAccount");
            if (account == null) return NotFound();
            return Ok(_mapper.Map<AccountDto>(account));
        }

        [HttpGet("byType/{type}")]
        public async Task<IActionResult> GetByType(AccountType type)
        {
            var accounts = await _unitOfWork.Accounts.GetByType(type);
            return Ok(_mapper.Map<List<AccountDto>>(accounts));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAccountDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var account = _mapper.Map<Account>(model);
                _unitOfWork.Accounts.Create(account);
                await _unitOfWork.SaveChangesAsync();
                return Ok(_mapper.Map<AccountDto>(account));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating account.");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(AccountDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var exists = await _unitOfWork.Accounts.ItemExistsAsync(a => a.Id == model.Id);
            if (!exists) return NotFound();
            try
            {
                var account = _mapper.Map<Account>(model);
                _unitOfWork.Accounts.Update(account);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating account.");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var account = await _unitOfWork.Accounts.GetById(id);
            if (account == null) return NotFound();
            try
            {
                _unitOfWork.Accounts.Delete(account);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting account.");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
