using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.Academics.Curriculum;
using SchoolWebApp.Core.DTOs.Academics.EducationLevelSubject;
using SchoolWebApp.Core.DTOs.Academics.Exam;
using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.API.Controllers.Academics
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EducationLevelSubjectsController : ControllerBase
    {
        private readonly ILogger<CurriculaController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public EducationLevelSubjectsController(ILogger<CurriculaController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        // GET: api/educationLevelSubjects
        /// <summary>
        /// A method for retrieving all education level subjects
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<EducationLevelSubjectDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(_mapper.Map<List<EducationLevelSubjectDto>>(await _unitOfWork.EducationLevelSubjects
                    .Find(includeProperties: "EducationLevel,Subject,AcademicYear")));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all education level subjects list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        // GET: api/educationLevelSubjects/paginated
        /// <summary>
        /// A method for retrieving a list of paginated education level subjects
        /// </summary>
        /// <param name="pageNumber">The page number to return</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns></returns>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<EducationLevelSubjectDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _unitOfWork.EducationLevelSubjects.GetPaginatedData(pageNumber ?? 1, pageSize ?? 4);
                var mappedData = _mapper.Map<List<EducationLevelSubjectDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<EducationLevelSubjectDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated education level Subjects.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/educationLevelSubjects/byEducationLevelId/5
        /// <summary>
        /// A method for retrieving education level subjects by education level Id.
        /// </summary>
        /// <param name="educationLevelId">The education level Id whose education level subjects are to be retrieved</param>
        /// <returns></returns>
        [HttpGet("byEducationLevelId/{educationLevelId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EducationLevelSubjectDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByEducationLevelId(int educationLevelId)
        {
            try
            {
                if (educationLevelId <= 0) return BadRequest(educationLevelId);
                var _item = await _unitOfWork.EducationLevelSubjects.GetByEducationLevelId(educationLevelId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<EducationLevelSubjectDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving education level subjects by education level id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/educationLevelSubjects/bySubjectId/5
        /// <summary>
        /// A method for retrieving education level subjects by subject Id.
        /// </summary>
        /// <param name="subjectId">The subject Id whose education level subjects are to be retrieved</param>
        /// <returns></returns>
        [HttpGet("bySubjectId/{subjectId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EducationLevelSubjectDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBySubjectId(int subjectId)
        {
            try
            {
                if (subjectId <= 0) return BadRequest(subjectId);
                var _item = await _unitOfWork.EducationLevelSubjects.GetBySubjectId(subjectId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<EducationLevelSubjectDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving education level subjects by subject id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/educationLevelSubjects/byAcademicYearId/5
        /// <summary>
        /// A method for retrieving education level subjects by academic year Id.
        /// </summary>
        /// <param name="academicYearId">The academic year Id whose education level subjects are to be retrieved</param>
        /// <returns></returns>
        [HttpGet("byAcademicYearId/{academicYearId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EducationLevelSubjectDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByAcademicYearId(int academicYearId)
        {
            try
            {
                if (academicYearId <= 0) return BadRequest(academicYearId);
                var _item = await _unitOfWork.EducationLevelSubjects.GetByAcademicYearId(academicYearId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<EducationLevelSubjectDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving education level subjects by academic year id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/educationLevelSubjects/byEducationLevelYearId/5/5
        /// <summary>
        /// A method for retrieving education level subjects by education level id and academic year Id.
        /// </summary>
        /// <param name="academicYearId">The academic year Id whose education level subjects are to be retrieved</param>
        /// <param name="educationLevelId">The education level Id whose education level subjects are to be retrieved</param>
        /// <returns></returns>
        [HttpGet("byEducationLevelYearId/{educationLevelId}/{academicYearId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EducationLevelSubjectDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByEducationLevelYearId(int educationLevelId, int academicYearId)
        {
            try
            {
                if (academicYearId <= 0) return BadRequest(academicYearId);
                if (educationLevelId <= 0) return BadRequest(educationLevelId);
                var _item = await _unitOfWork.EducationLevelSubjects.GetByEducationLevelYearId(educationLevelId, academicYearId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<EducationLevelSubjectDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving education level subjects by academic year id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        //GET api/educationLevelSubjects/searchIfExists/5/5/5
        /// <summary>
        /// A method that checks for the existence of an education level subject by education level, academic year and subject ids.
        /// </summary>
        /// <param name="educationLevelId">The education level id to be used for searching</param>
        /// <param name="academicYearId">The academic year id to be used for searching</param>
        /// <param name="subjectId">The subject id to be used for searching</param>
        /// <returns></returns>
        [HttpGet("checkIfExists/{educationLevelId}/{academicYearId}/{subjectId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CheckIfItemExists(int educationLevelId, int academicYearId, int subjectId)
        {
            try
            {
                if (academicYearId <= 0) return BadRequest(academicYearId);
                if (educationLevelId <= 0) return BadRequest(educationLevelId);
                if (subjectId <= 0) return BadRequest(subjectId);
                if (await _unitOfWork.EducationLevelSubjects.ItemExistsAsync(s => s.EducationLevelId == educationLevelId
            && s.SubjectId == subjectId && s.AcademicYearId == academicYearId))
                    return Ok(true);
                else return Ok(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving education level subjects by education level, academic year and subject id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }

        // GET api/educationLevelSubjects/byEducationLevelYearSubjectId/5/5/5
        /// <summary>
        /// A method for retrieving education level subjects by education level id, academic year Id and subject id.
        /// </summary>
        /// <param name="academicYearId">The academic year Id whose education level subjects are to be retrieved</param>
        /// <param name="educationLevelId">The education level Id whose education level subjects are to be retrieved</param>
        /// <param name="subjectId">The subject Id whose education level subjects are to be retrieved</param>
        /// <returns></returns>
        [HttpGet("byEducationLevelYearSubjectId/{educationLevelId}/{academicYearId}/{subjectId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EducationLevelSubjectDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByEducationLevelYearSubjectId(int educationLevelId, int academicYearId, int subjectId)
        {
            try
            {
                if (academicYearId <= 0) return BadRequest(academicYearId);
                if (educationLevelId <= 0) return BadRequest(educationLevelId);
                if (subjectId <= 0) return BadRequest(subjectId);
                var _item = await _unitOfWork.EducationLevelSubjects.GetByEducationLevelYearSubjectId(educationLevelId, academicYearId, subjectId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<EducationLevelSubjectDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving education level subjects by education level, academic year and subject id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/educationLevelSubjects/5
        /// <summary>
        /// A method for retrieving of education level subject record by Id.
        /// </summary>
        /// <param name="id">The education level subject Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EducationLevelSubjectDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _unitOfWork.EducationLevelSubjects.GetById(id, includeProperties: "EducationLevel,Subject,AcademicYear");

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<EducationLevelSubjectDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the education level subject details by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/educationLevelSubjects/batch
        /// <summary>
        /// A method for creating multiple education level subjects records
        /// </summary>
        /// <param name="model">The list of education level subjects</param>
        /// <returns></returns>
        [HttpPost("batch")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateMany(List<EducationLevelSubjectDto> model)
        {
            if (model == null || !model.Any())
            {
                return BadRequest("No education level subjects provided.");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    foreach (var item in model)
                    {
                        var existingEducationLevelSubject = await _unitOfWork.EducationLevelSubjects.GetByEducationLevelYearSubjectId(item.EducationLevelId, item.AcademicYearId, item.SubjectId);

                        if (existingEducationLevelSubject != null)
                        {
                            _unitOfWork.EducationLevelSubjects.Update(existingEducationLevelSubject);
                        }
                        else
                        {
                            var _item = _mapper.Map<EducationLevelSubject>(item);
                            _unitOfWork.EducationLevelSubjects.Create(_item);
                        }
                    }
                    await _unitOfWork.SaveChangesAsync();
                    return Ok("Education level subjects updated successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the education level subjects.");
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
            return BadRequest(ModelState);
        }

        // POST api/educationLevelSubjects
        /// <summary>
        /// A method for creating an education level subject record.
        /// </summary>
        /// <param name="model">The education level subject record to be created</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EducationLevelSubjectDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateEducationLevelSubjectDto model)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.EducationLevelSubjects.ItemExistsAsync(s => s.EducationLevelId == model.EducationLevelId && s.SubjectId == model.SubjectId && s.AcademicYearId == model.AcademicYearId))
                    return Conflict(new { message = $"The education level subject submitted already exists!" });
                try
                {
                    var _item = _mapper.Map<EducationLevelSubject>(model);
                    _unitOfWork.EducationLevelSubjects.Create(_item);
                    await _unitOfWork.SaveChangesAsync();
                    var returnItem = _mapper.Map<EducationLevelSubjectDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the education level subject.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the education level subject - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/educationLevelSubjects/5
        /// <summary>
        /// A method for updating the education level subject record.
        /// </summary>
        /// <param name="model">The education level subject record to be updated</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(EducationLevelSubjectDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _unitOfWork.EducationLevelSubjects.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The education level subject of Id- '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var _item = _mapper.Map<EducationLevelSubject>(model);
                    _unitOfWork.EducationLevelSubjects.Update(_item);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the education level subject.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the curriculum.");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE api/educationLevelSubjects/5
        /// <summary>
        /// A method for deleting the education level subject record by Id.
        /// </summary>
        /// <param name="id">The education level subject Id to be deleted</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _unitOfWork.EducationLevelSubjects.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The education level subject of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _unitOfWork.EducationLevelSubjects.GetById(id);
                _unitOfWork.EducationLevelSubjects.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the education level subject.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the education level subject - " + ex.Message);
            }
        }
    }

}
