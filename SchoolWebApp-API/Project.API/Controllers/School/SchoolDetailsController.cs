using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs.School.SchoolDetails;
using SchoolWebApp.Core.Interfaces.IServices.School;

namespace SchoolWebApp.API.Controllers.School
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolDetailsController : ControllerBase
    {
        private readonly ILogger<SchoolDetailsController> _logger;
        private readonly ISchoolDetailsService _schoolDetailsService;

        public SchoolDetailsController(ILogger<SchoolDetailsController> logger, ISchoolDetailsService schoolDetailsService)
        {
            _logger = logger;
            _schoolDetailsService = schoolDetailsService;
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
                var schoolDetails = await _schoolDetailsService.GetPaginatedSchoolDetails(pageNumberValue, pageSizeValue);
                return Ok(schoolDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving school details.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
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
                var schoolDetails = await _schoolDetailsService.GetSchoolDetails();
                return Ok(schoolDetails);
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
                var data = await _schoolDetailsService.GetSchoolDetail(id);
                if (data == null) return NotFound();
                return Ok(data);
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
                if (await _schoolDetailsService.IsExists("Name", model.Name))
                    return Conflict(new { message = $"The school detail name- '{model.Name}' already exists" });
                if (await _schoolDetailsService.IsExists("Email", model.Email))
                    return Conflict(new { message = $"The school detail email- '{model.Email}' already exists" });
                try
                {
                    var data = await _schoolDetailsService.Create(model);
                    return Ok(data);
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
                var schoolDetailsExist = await _schoolDetailsService.IsExists("Id", model.Id.ToString());
                if (!schoolDetailsExist) 
                    return BadRequest($"The school detail of Id- '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    await _schoolDetailsService.Update(model);
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
                await _schoolDetailsService.Delete(id);
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
