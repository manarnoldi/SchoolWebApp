using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.Academics.Subject;
using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.API.Controllers.Academics
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectsController : ControllerBase
    {
        private readonly ILogger<SubjectsController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SubjectsController(ILogger<SubjectsController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/subjects
        /// <summary>
        /// A method for retrieving all subjects
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SubjectDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(_mapper.Map<List<SubjectDto>>(await _unitOfWork.Subjects.GetAll()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all subjects list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/subjects/paginated
        /// <summary>
        /// A method for retrieving a list of paginated subjects
        /// </summary>
        /// <param name="pageNumber">The page number to return</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns></returns>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SubjectDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _unitOfWork.Subjects.GetPaginatedData(pageNumber ?? 1, pageSize ?? 4);
                var mappedData = _mapper.Map<List<SubjectDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<SubjectDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated subjects.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/subjects/5
        /// <summary>
        /// A method for retrieving a subject record by Id.
        /// </summary>
        /// <param name="id">The subject Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SubjectDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _unitOfWork.Subjects.GetById(id);

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<SubjectDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the subject details by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/subjects/bySubjectGroupId/5
        /// <summary>
        /// A method for retrieving subjects by subject group Id.
        /// </summary>
        /// <param name="id">The subject group Id whose subjects are to be retrieved</param>
        /// <returns></returns>
        [HttpGet("bySubjectGroupId/{subjectGroupId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SubjectDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSubjectsBySubjectGroupId(int subjectGroupId)
        {
            try
            {
                if (subjectGroupId <= 0) return BadRequest(subjectGroupId);
                var _item = await _unitOfWork.Subjects.GetBySubjectGroupId(subjectGroupId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<SubjectDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the subjects by subject group id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/subjects/byDepartmentId/5
        /// <summary>
        /// A method for retrieving subjects by department Id.
        /// </summary>
        /// <param name="id">The department Id whose subjects are to be retrieved</param>
        /// <returns></returns>
        [HttpGet("byDepartmentId/{departmentId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SubjectDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSubjectsByDepartmentId(int departmentId)
        {
            try
            {
                if (departmentId <= 0) return BadRequest(departmentId);
                var _item = await _unitOfWork.Subjects.GetByDepartmentId(departmentId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<SubjectDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the subjects by departmentId id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/subjects
        /// <summary>
        /// A method for creating a subject record.
        /// </summary>
        /// <param name="model">The subject record to be created</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SubjectDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateSubjectDto model)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.Subjects.ItemExistsAsync(s => s.Name == model.Name && s.SubjectGroupId == model.SubjectGroupId && s.DepartmentId == model.DepartmentId))
                    return Conflict(new { message = $"The subject details submitted already exist" });
                try
                {
                    var _item = _mapper.Map<Subject>(model);
                    _unitOfWork.Subjects.Create(_item);
                    await _unitOfWork.SaveChangesAsync();
                    var returnItem = _mapper.Map<SubjectDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the subject.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the subject - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/subjects/5
        /// <summary>
        /// A method for updating a subject record.
        /// </summary>
        /// <param name="model">The subject record to be updated</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(SubjectDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _unitOfWork.Subjects.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The subject of Id - '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var _item = _mapper.Map<Subject>(model);
                    _unitOfWork.Subjects.Update(_item);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the subject details.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the subject details.");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE api/subjects/5
        /// <summary>
        /// A method for deleting the subject record by Id.
        /// </summary>
        /// <param name="id">The subject Id to be retrieved</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _unitOfWork.Subjects.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The subject of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _unitOfWork.Subjects.GetById(id);
                _unitOfWork.Subjects.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the subject details.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the subject - " + ex.Message);
            }
        }
    }
}
