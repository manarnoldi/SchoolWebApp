using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs.Academics.Subject;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.DTOs.Academics.ExamResult;
using SchoolWebApp.Core.DTOs.Students.StudentSubjects;
using SchoolWebApp.Core.Entities.Students;

namespace SchoolWebApp.API.Controllers.Academics
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ExamResultsController : ControllerBase
    {
        private readonly ILogger<ExamResultsController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ExamResultsController(ILogger<ExamResultsController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/examResults
        /// <summary>
        /// A method for retrieving all exam results
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ExamResultDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(_mapper.Map<List<ExamResultDto>>(await _unitOfWork.ExamResults.Find(includeProperties: "StudentSubject,Exam")));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all exam results list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/examResults/paginated
        /// <summary>
        /// A method for retrieving a list of paginated exam results
        /// </summary>
        /// <param name="pageNumber">The page number to return</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns></returns>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ExamResultDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _unitOfWork.ExamResults.GetPaginatedData(pageNumber ?? 1, pageSize ?? 4);
                var mappedData = _mapper.Map<List<ExamResultDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<ExamResultDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated exam results.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/examResults/5
        /// <summary>
        /// A method for retrieving an exam result record by Id.
        /// </summary>
        /// <param name="id">The exam result Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExamResultDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _unitOfWork.ExamResults.GetById(id, includeProperties: "StudentSubject,Exam");

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<ExamResultDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the exam result details by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/examResults/byStudentSubjectId/5
        /// <summary>
        /// A method for retrieving exam results by student subject Id.
        /// </summary>
        /// <param name="id">The exam result student subject Id whose exam results are to be retrieved</param>
        /// <returns></returns>
        [HttpGet("byStudentSubjectId/{studentSubjectId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExamResultDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetExamResultsByStudentSubjectId(int studentSubjectId)
        {
            try
            {
                if (studentSubjectId <= 0) return BadRequest(studentSubjectId);
                var _item = await _unitOfWork.ExamResults.GetByStudentSubjectId(studentSubjectId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<ExamResultDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the exam results by student subject id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/examResults/byExamId/5
        /// <summary>
        /// A method for retrieving exam results by exam Id.
        /// </summary>
        /// <param name="id">The exam Id whose exam results are to be retrieved</param>
        /// <returns></returns>
        [HttpGet("byExamId/{examId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExamResultDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetExamResultsByExamId(int examId)
        {
            try
            {
                if (examId <= 0) return BadRequest(examId);
                var _item = await _unitOfWork.ExamResults.GetByExamId(examId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<ExamResultDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the exam results by exam id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/examResults
        /// <summary>
        /// A method for creating a exam result record.
        /// </summary>
        /// <param name="model">The exam result record to be created</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExamResultDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateExamResultDto model)
        {
            if (ModelState.IsValid)
            {
                if (!await _unitOfWork.StudentSubjects.ItemExistsAsync(s => s.Id == model.StudentSubjectId))
                    return Conflict(new { message = $"The student subject details submitted does not exist." });
                if (!await _unitOfWork.Exams.ItemExistsAsync(s => s.Id == model.ExamId))
                    return Conflict(new { message = $"The exam details submitted do not exist." });
                try
                {
                    var _item = _mapper.Map<ExamResult>(model);
                    _unitOfWork.ExamResults.Create(_item);
                    await _unitOfWork.SaveChangesAsync();
                    var returnItem = _mapper.Map<ExamResultDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the exam result.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the exam result - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // POST api/examResults/batch
        /// <summary>
        /// A method for creating multiple exam results records as a batch
        /// </summary>
        /// <param name="model">The list of exam results to be posted</param>
        /// <returns></returns>
        [HttpPost("batch")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateMany(List<ExamResultDto> model)
        {
            if (model == null || !model.Any())
            {
                return BadRequest("No exam results provided.");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    foreach (var item in model)
                    {
                        if(item.Score != null)
                        {
                            var existingExamResult = await _unitOfWork.ExamResults.GetByStudentSubjectExamId(item.StudentSubjectId, item.ExamId);

                            if (existingExamResult != null)
                            {
                                existingExamResult.Score = (float) item.Score;
                                _unitOfWork.ExamResults.Update(existingExamResult);
                            }
                            else
                            {
                                var _item = _mapper.Map<ExamResult>(item);
                                _unitOfWork.ExamResults.Create(_item);
                            }
                        }
                        
                    }
                    await _unitOfWork.SaveChangesAsync();
                    return Ok("Exam results updated successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the exam results.");
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
            return BadRequest(ModelState);
        }


        // PUT api/examResults/5
        /// <summary>
        /// A method for updating a exam result record.
        /// </summary>
        /// <param name="model">The exam result record to be updated</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(ExamResultDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _unitOfWork.ExamResults.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The exam result of Id - '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var _item = _mapper.Map<ExamResult>(model);
                    _unitOfWork.ExamResults.Update(_item);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the exam result details.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the exam result details.");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE api/examResults/5
        /// <summary>
        /// A method for deleting the exam result record by Id.
        /// </summary>
        /// <param name="id">The exam result Id to be deleted</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _unitOfWork.ExamResults.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The exam result of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _unitOfWork.ExamResults.GetById(id);
                _unitOfWork.ExamResults.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the exam result details.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the exam result - " + ex.Message);
            }
        }
    }
}
