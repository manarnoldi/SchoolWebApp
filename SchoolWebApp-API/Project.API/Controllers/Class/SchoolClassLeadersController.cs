using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.Class.SchoolClass;
using SchoolWebApp.Core.DTOs.Class.SchoolClassLeaders;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.API.Controllers.Class
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolClassLeadersController : ControllerBase
    {
        private readonly ILogger<SchoolClassLeadersController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public SchoolClassLeadersController(ILogger<SchoolClassLeadersController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/schoolClassLeaders
        /// <summary>
        /// A method for retrieving all school class leaders
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SchoolClassLeadersDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var schoolClassLeaders = await _unitOfWork.SchoolClassLeaders.Find(includeProperties: "SchoolClass,ClassLeadershipRole");
                return Ok(_mapper.Map<List<SchoolClassLeadersDto>>(schoolClassLeaders));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all school class leaders list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/schoolClassLeaders/paginated
        /// <summary>
        /// A method for retrieving a list of paginated school class leaders
        /// </summary>
        /// <param name="pageNumber">The page number to return</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns></returns>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SchoolClassLeadersDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _unitOfWork.SchoolClassLeaders.GetPaginatedData(pageNumber ?? 1, pageSize ?? 4);
                var mappedData = _mapper.Map<List<SchoolClassLeadersDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<SchoolClassLeadersDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated school class leaders.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/schoolClassLeaders/bySchoolClassId/5
        /// <summary>
        /// A method for retrieving school class leaders by school class Id.
        /// </summary>
        /// <param name="id">The school class Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("bySchoolClassId/{schoolClassId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SchoolClassLeadersDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBySchoolClassId(int schoolClassId)
        {
            try
            {
                if (schoolClassId <= 0) return BadRequest(schoolClassId);
                var _item = await _unitOfWork.SchoolClassLeaders.GetBySchoolClassId(schoolClassId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<SchoolClassLeadersDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the school class leaders by school class id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/schoolClassLeaders/5
        /// <summary>
        /// A method for retrieving a school class leaders record by Id.
        /// </summary>
        /// <param name="id">The school class leader Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SchoolClassLeadersDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _unitOfWork.SchoolClassLeaders.GetById(id, includeProperties: "SchoolClass,ClassLeadershipRole");

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<SchoolClassLeadersDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the school class leaders details by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/schoolClassLeaders
        /// <summary>
        /// A method for creating a school class leaders record.
        /// </summary>
        /// <param name="model">The school class leaders record to be created</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SchoolClassLeadersDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateSchoolClassLeadersDto model)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.SchoolClassLeaders.ItemExistsAsync(s => s.SchoolClassId == model.SchoolClassId && s.PersonId == model.PersonId
                && s.ClassLeadershipRoleId == model.ClassLeadershipRoleId))
                    return Conflict(new { message = $"The school class leaders details submitted already exists" });
                try
                {
                    var _item = _mapper.Map<SchoolClassLeaders>(model);
                    _unitOfWork.SchoolClassLeaders.Create(_item);
                    await _unitOfWork.SaveChangesAsync();
                    var returnItem = _mapper.Map<SchoolClassLeadersDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the school class leaders.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the school class leaders - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/schoolClassLeaders/5
        /// <summary>
        /// A method for updating a school class leaders record.
        /// </summary>
        /// <param name="model">The school class leaders record to be updated</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(SchoolClassLeadersDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _unitOfWork.SchoolClassLeaders.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The school class leader of Id - '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var _item = _mapper.Map<SchoolClassLeaders>(model);
                    _unitOfWork.SchoolClassLeaders.Update(_item);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the school class leader.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the school class leader.");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE api/schoolClassLeaders/5
        /// <summary>
        /// A method for deleting the school class leaders record by Id.
        /// </summary>
        /// <param name="id">The school class leaders Id to be deleted</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _unitOfWork.SchoolClassLeaders.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The school class leaders of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _unitOfWork.SchoolClassLeaders.GetById(id);
                _unitOfWork.SchoolClassLeaders.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the school class leaders.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the school class leaders - " + ex.Message);
            }
        }
    }
}
