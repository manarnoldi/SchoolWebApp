using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.Academics.Exam;
using SchoolWebApp.Core.Entities.CBE.Exams;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.API.Controllers.Academics
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ExamsController : ControllerBase
    {
        private readonly ILogger<ExamsController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ExamsController(ILogger<ExamsController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/exams
        /// <summary>
        /// A method for retrieving all exams
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ExamDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(_mapper.Map<List<ExamDto>>(await _unitOfWork.Exams
                    .Find(includeProperties: "ExamType,SchoolClass,Session,Subject")));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all exams list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/exams/paginated
        /// <summary>
        /// A method for retrieving a list of paginated exams
        /// </summary>
        /// <param name="pageNumber">The page number to return</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns></returns>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ExamDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _unitOfWork.Exams.GetPaginatedData(pageNumber ?? 1, pageSize ?? 4);
                var mappedData = _mapper.Map<List<ExamDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<ExamDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated exams.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/exams/bySchoolClassId/5
        /// <summary>
        /// A method for retrieving exams by schoolclass Id.
        /// </summary>
        /// <param name="id">The school class Id whose exams it to be retrieved</param>
        /// <returns></returns>
        [HttpGet("bySchoolClassId/{schoolClassId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExamDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetExamsBySchoolClassId(int schoolClassId)
        {
            try
            {
                if (schoolClassId <= 0) return BadRequest(schoolClassId);
                var _item = await _unitOfWork.Exams.GetBySchoolClassId(schoolClassId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<ExamDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the exams by school class id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/exams/bySessionId/5
        /// <summary>
        /// A method for retrieving exams by session Id.
        /// </summary>
        /// <param name="id">The session Id whose exams it to be retrieved</param>
        /// <returns></returns>
        [HttpGet("bySessionId/{sessionId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExamDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetExamsBySessionId(int sessionId)
        {
            try
            {
                if (sessionId <= 0) return BadRequest(sessionId);
                var _item = await _unitOfWork.Exams.GetBySessionId(sessionId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<ExamDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the exams by session id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/exams/examSearch?academicYearId=5&curriculumId=5&sessionId=5&schoolClassId=5&subjectId=5&examTypeId=5&examName='Exam 1'
        /// <summary>
        /// A method for retrieving exams by searching.
        /// </summary>
        /// <param name="academicYearId">The academic year Id whose exams is to be retrieved</param>
        /// <param name="curriculumId">The curriculum Id whose exams is to be retrieved</param>
        /// <param name="sessionId">The session Id whose exams is to be retrieved</param>
        /// <param name="schoolClassId">The school class Id whose exams is to be retrieved</param>
        /// <param name="subjectId">The subject Id whose exams it to be retrieved</param>
        /// <returns></returns>
        [HttpGet("examSearch")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExamDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExamsSearch(int academicYearId, int curriculumId, int sessionId, int? schoolClassId = null,
            int? subjectId = null, int? examTypeId = null)
        {
            try
            {
                if (academicYearId <= 0) return BadRequest(academicYearId);
                if (curriculumId <= 0) return BadRequest(curriculumId);
                if (sessionId <= 0) return BadRequest(sessionId);
                if (schoolClassId <= 0) return BadRequest(schoolClassId);
                var _item = await _unitOfWork.Exams.SearchForExam(academicYearId, curriculumId, sessionId, schoolClassId, subjectId, examTypeId);
                //if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<ExamDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the exams by session items.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/exams/5
        /// <summary>
        /// A method for retrieving an exam record by Id.
        /// </summary>
        /// <param name="id">The exam Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExamDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _unitOfWork.Exams.GetById(id, includeProperties: "ExamName,SchoolClass,Session,Subject");

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<ExamDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the exam details by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/exams
        /// <summary>
        /// A method for creating an exam record.
        /// </summary>
        /// <param name="model">The exam record to be created</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExamDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateExamDto model)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.Exams.ItemExistsAsync(s => s.ExamTypeId == model.ExamTypeId && s.SchoolClassId == model.SchoolClassId
                && s.SessionId == model.SessionId && s.SubjectId == model.SubjectId))
                    return Conflict(new { message = $"The exam details submitted already exist." });
                try
                {
                    var _item = _mapper.Map<Exam>(model);
                    _unitOfWork.Exams.Create(_item);
                    await _unitOfWork.SaveChangesAsync();
                    var returnItem = _mapper.Map<ExamDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the exam.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the exam - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/exams/5
        /// <summary>
        /// A method for updating an exam record.
        /// </summary>
        /// <param name="model">The exam record to be updated</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(ExamDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _unitOfWork.Exams.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The exam of Id - '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var _item = _mapper.Map<Exam>(model);
                    _unitOfWork.Exams.Update(_item);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the exam.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the exam.");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE api/exams/5
        /// <summary>
        /// A method for deleting the exam record by Id.
        /// </summary>
        /// <param name="id">The exam Id to be retrieved</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _unitOfWork.Exams.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The exam of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _unitOfWork.Exams.GetById(id);
                _unitOfWork.Exams.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the exam.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the exam - " + ex.Message);
            }
        }
    }
}
