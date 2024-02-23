using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.School.SchoolDetails;
using SchoolWebApp.Core.Entities.School;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.API.Controllers.School
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolDetailsController : ControllerBase
    {
        private readonly ILogger<SchoolDetailsController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public SchoolDetailsController(ILogger<SchoolDetailsController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/schooldetails
        /// <summary>
        /// A method for retrieving a list of school details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SchoolDetailsDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var schoolDetailsList = await _unitOfWork.SchoolDetails.GetAll();
                var returnSchoolDetailsList = _mapper.Map<List<SchoolDetailsDto>>(schoolDetailsList);
                return Ok(returnSchoolDetailsList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving school details.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/schooldetails/paginated
        /// <summary>
        /// A method for retrieving a list of paginated school details
        /// </summary>
        /// <param name="pageNumber">The page number to return</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns></returns>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SchoolDetailsDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                int pageSizeValue = (pageSize ?? 4);
                int pageNumberValue = (pageNumber ?? 1);
                var paginatedData = await _unitOfWork.SchoolDetails.GetPaginatedData(pageNumberValue, pageSizeValue);
                var mappedData = _mapper.Map<List<SchoolDetailsDto>>(paginatedData.Data);
                var paginatedDataViewModel = new PaginatedDto<SchoolDetailsDto>(mappedData.ToList(), paginatedData.TotalCount);
                return Ok(paginatedData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving school details.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }      

        // GET api/schooldetails/5
        /// <summary>
        /// A method for retrieving a record of school details by Id.
        /// </summary>
        /// <param name="id">The school Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SchoolDetailsDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _schoolDetail = await _unitOfWork.SchoolDetails.GetById(id);
                
                if (_schoolDetail == null) return NotFound();
                var _schoolDetailDto = _mapper.Map<SchoolDetailsDto>(_schoolDetail);
                return Ok(_schoolDetailDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the provided school details.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        // POST api/schooldetails
        /// <summary>
        /// A method for creating school details record.
        /// </summary>
        /// <param name="model">The school details record to be created</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SchoolDetailsDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateSchoolDetailsDto model)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.SchoolDetails.IsExists("Name", model.Name))
                    return Conflict(new { message = $"The school detail name - '{model.Name}' already exists" });
                if (await _unitOfWork.SchoolDetails.IsExists("Email", model.Email))
                    return Conflict(new { message = $"The school detail email - '{model.Email}' already exists" });
                try
                {
                    var _schoolDetail = _mapper.Map<SchoolDetails>(model);
                    _unitOfWork.SchoolDetails.Create(_schoolDetail);
                    await _unitOfWork.SaveChangesAsync();
                    var returnSchoolDetails = _mapper.Map<SchoolDetailsDto>(_schoolDetail);
                    return Ok(returnSchoolDetails);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the school details");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the school details- {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/schooldetails/5
        /// <summary>
        /// A method for updating school details record.
        /// </summary>
        /// <param name="model">The school details record to be updated</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(SchoolDetailsDto model)
        {
            if (ModelState.IsValid)
            {
                var schoolDetailsExist = await _unitOfWork.SchoolDetails.ItemExistsAsync(m => m.Id == model.Id);
                if (!schoolDetailsExist)
                    return BadRequest($"The school detail of Id- '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var existingData = await _unitOfWork.SchoolDetails.GetById(model.Id);
                    //Manual mapping
                    existingData.Name = model.Name;
                    existingData.Address = model.Address;
                    existingData.SchoolLevelId = model.SchoolLevelId;
                    existingData.Telephone = model.Telephone;
                    existingData.Email = model.Email;
                    existingData.Initials = model.Initials;
                    existingData.LogoUrl = model.LogoUrl;
                    existingData.Motto = model.Motto;
                    existingData.Vision = model.Vision;
                    existingData.Website = model.Website;

                    _unitOfWork.SchoolDetails.Update(existingData);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the school details.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the school details.");
                }
            }
            return BadRequest(ModelState);
        }


        // DELETE api/schooldetails/5
        /// <summary>
        /// A method for deleting the school details record by Id.
        /// </summary>
        /// <param name="id">The school Id to be retrieved</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var schoolDetailsExist = await _unitOfWork.SchoolDetails.GetById(id);
                if (schoolDetailsExist == null)
                    return BadRequest($"The school detail of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _unitOfWork.SchoolDetails.GetById(id);
                _unitOfWork.SchoolDetails.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the school details");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the school details- " + ex.Message);
            }
        }
    }
}
