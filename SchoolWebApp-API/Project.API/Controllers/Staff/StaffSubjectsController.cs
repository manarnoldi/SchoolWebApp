﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.Staff.StaffSubject;
using SchoolWebApp.Core.Entities.Staff;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.API.Controllers.Staff
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StaffSubjectsController : ControllerBase
    {
        private readonly ILogger<StaffSubjectsController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public StaffSubjectsController(ILogger<StaffSubjectsController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/staffSubjects
        /// <summary>
        /// A method for retrieving all staff subjects
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StaffSubjectDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(_mapper.Map<List<StaffSubjectDto>>(await _unitOfWork.StaffSubjects.GetAll()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all staff subjects list.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/staffSubjects/paginated
        /// <summary>
        /// A method for retrieving a list of paginated staff subjects
        /// </summary>
        /// <param name="pageNumber">The page number to return</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns></returns>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StaffSubjectDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            try
            {
                var paginatedData = await _unitOfWork.StaffSubjects.GetPaginatedData(pageNumber ?? 1, pageSize ?? 4);
                var mappedData = _mapper.Map<List<StaffSubjectDto>>(paginatedData.Data);
                return Ok(new PaginatedDto<StaffSubjectDto>(mappedData.ToList(), paginatedData.TotalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated staff subjects.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/staffSubjects/byAcademicYearId/5
        /// <summary>
        /// A method for retrieving staff subjects by academic year Id.
        /// </summary>
        /// <param name="academicYearId">The academic year Id whose staff subjects will be retrieved</param>
        /// <returns></returns>
        [HttpGet("byAcademicYearId/{academicYearId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StaffSubjectDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStaffSubjectsByAcedemicYearId(int academicYearId)
        {
            try
            {
                if (academicYearId <= 0) return BadRequest(academicYearId);
                var _item = await _unitOfWork.StaffSubjects.GetByAcademicYearId(academicYearId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<StaffSubjectDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the staff subjects by academic year id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/staffSubjects/bySchoolClassId/5
        /// <summary>
        /// A method for retrieving staff subjects by school class Id.
        /// </summary>
        /// <param name="schoolClassId">The school class Id whose staff subjects will be retrieved</param>
        /// <returns></returns>
        [HttpGet("bySchoolClassId/{schoolClassId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StaffSubjectDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStaffSubjectsBySchoolClassId(int schoolClassId)
        {
            try
            {
                if (schoolClassId <= 0) return BadRequest(schoolClassId);
                var _item = await _unitOfWork.StaffSubjects.GetBySchoolClassId(schoolClassId);
                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<List<StaffSubjectDto>>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the staff subjects by school class id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/staffSubjects/5
        /// <summary>
        /// A method for retrieving of staff subjects record by Id.
        /// </summary>
        /// <param name="id">The staff subjects Id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StaffSubjectDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(id);
                var _item = await _unitOfWork.StaffSubjects.GetById(id);

                if (_item == null) return NotFound();
                var _itemDto = _mapper.Map<StaffSubjectDto>(_item);
                return Ok(_itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the staff subjects details by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/staffSubjects
        /// <summary>
        /// A method for creating a staff subject record.
        /// </summary>
        /// <param name="model">The staff subject record to be created</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StaffSubjectDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateStaffSubjectDto model)
        {
            if (ModelState.IsValid)
            {
                if (!await _unitOfWork.StaffDetails.ItemExistsAsync(s => s.Id == model.StaffDetailsId))
                    return Conflict(new { message = $"The staff details submitted do not exist." });
                if (!await _unitOfWork.Subjects.ItemExistsAsync(s => s.Id == model.SubjectId))
                    return Conflict(new { message = $"The subject details submitted do not exist." });
                if (!await _unitOfWork.SchoolClasses.ItemExistsAsync(s => s.Id == model.SchoolClassId))
                    return Conflict(new { message = $"The school class details submitted do not exist." });
                if (!await _unitOfWork.AcademicYears.ItemExistsAsync(s => s.Id == model.AcademicYearId))
                    return Conflict(new { message = $"The academic year details submitted do not exist." });
                if (await _unitOfWork.StaffSubjects.ItemExistsAsync(s => s.StaffDetailsId == model.StaffDetailsId && s.SubjectId == model.SubjectId &&
                s.SchoolClassId == model.SchoolClassId && s.AcademicYearId == model.AcademicYearId && s.Description == model.Description))
                    return Conflict(new { message = $"The staff subject record already exists" });
                try
                {
                    var _item = _mapper.Map<StaffSubject>(model);
                    _unitOfWork.StaffSubjects.Create(_item);
                    await _unitOfWork.SaveChangesAsync();
                    var returnItem = _mapper.Map<StaffSubjectDto>(_item);
                    return Ok(returnItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the staff subject");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the staff subject - {ex.Message}");
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/staffSubjects/5
        /// <summary>
        /// A method for updating a staff subject record.
        /// </summary>
        /// <param name="model">The staff subject record to be updated</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(StaffSubjectDto model)
        {
            if (ModelState.IsValid)
            {
                var itemExist = await _unitOfWork.StaffSubjects.ItemExistsAsync(m => m.Id == model.Id);
                if (!itemExist)
                    return BadRequest($"The staff subject of Id- '{model.Id}' does not exist hence cannot be updated.");
                try
                {
                    var existingItem = await _unitOfWork.StaffSubjects.GetById(model.Id);
                    //Manual mapping
                    existingItem.StaffDetailsId = model.StaffDetailsId;
                    existingItem.SubjectId = model.SubjectId;
                    existingItem.SchoolClassId = model.SchoolClassId;
                    existingItem.AcademicYearId = model.AcademicYearId;
                    existingItem.Description = model.Description;
                    _unitOfWork.StaffSubjects.Update(existingItem);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the staff subject.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the staff subject.");
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE api/staffSubjects/5
        /// <summary>
        /// A method for deleting the staff subject record by Id.
        /// </summary>
        /// <param name="id">The staff subject Id to be retrieved</param>
        /// <returns></returns>
        [Authorize(Policy = "AdminRole")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemExists = await _unitOfWork.StaffSubjects.GetById(id);
                if (itemExists == null)
                    return BadRequest($"The staff subject of Id- '{id}' does not exist hence cannot be deleted.");
                var entity = await _unitOfWork.StaffSubjects.GetById(id);
                _unitOfWork.StaffSubjects.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the staff subject.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the staff subject - " + ex.Message);
            }
        }
    }
}
