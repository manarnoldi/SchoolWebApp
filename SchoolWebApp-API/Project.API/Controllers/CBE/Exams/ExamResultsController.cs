using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.Academics.ExamResult;
using SchoolWebApp.Core.DTOs.Reports.Academics;
using SchoolWebApp.Core.Entities.CBE.Exams;
using SchoolWebApp.Core.Entities.Identity;
using SchoolWebApp.Core.Interfaces.IRepositories;

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
        private readonly UserManager<AppUser> _userManager;

        public ExamResultsController(
            ILogger<ExamResultsController> logger,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UserManager<AppUser> userManager)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        /// <summary>
        /// Enforces that the current user may write results for the given exam(s).
        /// Administrators and SuperAdministrators bypass the check. Teachers must
        /// have a StaffSubject row linking them to the exam's (SubjectId,
        /// SchoolClassId) pair. Returns null when allowed, or an IActionResult
        /// the caller should return immediately when blocked.
        /// </summary>
        /// <remarks>
        /// Defense-in-depth alongside the frontend banner in
        /// ExamResultsComponent. The frontend disables the Save button when a
        /// teacher is not allocated; this server-side gate stops a tampered or
        /// scripted request from bypassing the UI.
        /// </remarks>
        private async Task<IActionResult> CheckCanWriteResultsAsync(IEnumerable<int> examIds)
        {
            // Resolve the current user from the stable `userid` claim, then read
            // their roles authoritatively from the database. Reading roles from
            // the JWT directly is fragile: the claim type depends on the JWT
            // inbound-claim mapping (so `User.Claims["roles"]` can come back
            // empty), and a token minted before a role change is stale. The DB
            // is the source of truth and covers both cases.
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userid")?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return StatusCode(StatusCodes.Status403Forbidden,
                    new { message = "Unable to identify the current user." });

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return StatusCode(StatusCodes.Status403Forbidden,
                    new { message = "Unable to identify the current user." });

            var roles = (await _userManager.GetRolesAsync(user)).ToHashSet();

            if (roles.Contains("Administrator") || roles.Contains("SuperAdministrator"))
                return null;

            if (!roles.Contains("Teacher"))
                return StatusCode(StatusCodes.Status403Forbidden,
                    new { message = "Only teachers and administrators can submit exam results." });

            if (user.PersonId == null)
                return StatusCode(StatusCodes.Status403Forbidden,
                    new { message = "Your account is not linked to a staff record." });

            var staffDetailsId = user.PersonId.Value;

            // Resolve each distinct exam to its (SubjectId, SchoolClassId) pair
            // so we know exactly which allocations to check.
            var distinctExamIds = examIds.Where(id => id > 0).Distinct().ToList();
            if (distinctExamIds.Count == 0) return null;

            var requiredPairs = new HashSet<(int subjectId, int schoolClassId)>();
            foreach (var eid in distinctExamIds)
            {
                var exam = await _unitOfWork.Exams.GetById(eid);
                if (exam == null) continue; // existing handlers will return their own error for unknown exam
                requiredPairs.Add((exam.SubjectId, exam.SchoolClassId));
            }
            if (requiredPairs.Count == 0) return null;

            // One DB roundtrip for the teacher's full allocation list, then local filter.
            var allocations = await _unitOfWork.StaffSubjects.GetByStaffDetailsId(staffDetailsId);
            foreach (var pair in requiredPairs)
            {
                var ok = allocations.Any(a => a.SubjectId == pair.subjectId
                                              && a.SchoolClassId == pair.schoolClassId);
                if (!ok)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new
                    {
                        message = "You are not allocated to one or more of the (subject, class) combinations being submitted."
                    });
                }
            }
            return null;
        }

        /// <summary>
        /// Resolves the StudentSubject (allocation) a result for (examId,
        /// studentId) belongs to. Returns null when the student is not allocated
        /// the exam's subject in the exam's class - in which case a result must
        /// not be created (it would be an orphan). An optional per-exam cache
        /// avoids re-querying allocations for every row of a batch.
        /// </summary>
        private async Task<int?> ResolveStudentSubjectIdAsync(
            int examId, int studentId, Dictionary<int, Dictionary<int, int>> cache = null)
        {
            if (cache == null || !cache.TryGetValue(examId, out var byStudent))
            {
                byStudent = new Dictionary<int, int>();
                var exam = await _unitOfWork.Exams.GetById(examId);
                if (exam != null)
                {
                    var allocations = await _unitOfWork.StudentSubjects
                        .GetBySchoolClassSubjectId(exam.SchoolClassId, exam.SubjectId);
                    foreach (var a in allocations)
                    {
                        var sid = a.StudentClass?.StudentId;
                        if (sid != null) byStudent[sid.Value] = a.Id;
                    }
                }
                cache?.Add(examId, byStudent);
            }
            return byStudent.TryGetValue(studentId, out var ssId) ? ssId : (int?)null;
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
                return Ok(_mapper.Map<List<ExamResultDto>>(await _unitOfWork.ExamResults.Find(includeProperties: "Student,Exam")));
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
                var _item = await _unitOfWork.ExamResults.GetById(id, includeProperties: "Student,Exam");

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

        // GET api/examResults/byStudentSubjectId/5/5
        /// <summary>
        /// A method for retrieving exam results by student subject Id.
        /// </summary>
        /// <param name="studentId">The exam result student Id whose exam results are to be retrieved</param>
        /// <param name="subjectId">The exam result subject Id whose exam results are to be retrieved</param>
        /// <returns></returns>
        [HttpGet("byStudentSubjectId/{studentId}/{subjectId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExamResultDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetExamResultsByStudentSubjectId(int studentId, int subjectId)
        {
            try
            {
                if (studentId <= 0) return BadRequest(studentId);
                if (subjectId <= 0) return BadRequest(subjectId);
                var _item = await _unitOfWork.ExamResults.GetByStudentSubjectId(studentId, subjectId);
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

        // GET api/examResults/missingMarks?academicYearId=5&curriculumId=5&sessionId=5&examTypeId=5
        /// <summary>
        /// Returns, in one query, every allocated student who has no result
        /// recorded yet for the exams matching the selection. Replaces the
        /// frontend's per-exam request fan-out that overwhelmed the host.
        /// </summary>
        [HttpGet("missingMarks")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ExamResultDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMissingMarks(int academicYearId, int curriculumId, int sessionId, int? examTypeId = null)
        {
            try
            {
                if (academicYearId <= 0) return BadRequest(academicYearId);
                if (curriculumId <= 0) return BadRequest(curriculumId);
                if (sessionId <= 0) return BadRequest(sessionId);

                var missing = await _unitOfWork.ExamResults.GetMissingMarks(academicYearId, curriculumId, sessionId, examTypeId);
                return Ok(_mapper.Map<List<ExamResultDto>>(missing));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the missing marks.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/examResults/byAllocation/5
        /// <summary>
        /// Exam results attached to a single subject allocation (StudentSubject).
        /// Used by the deallocation dialog to show what will cascade-delete.
        /// </summary>
        [HttpGet("byAllocation/{studentSubjectId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ExamResultDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByAllocation(int studentSubjectId)
        {
            try
            {
                if (studentSubjectId <= 0) return BadRequest(studentSubjectId);
                var items = await _unitOfWork.ExamResults.GetByAllocationId(studentSubjectId);
                return Ok(_mapper.Map<List<ExamResultDto>>(items));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving results by allocation.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST api/examResults/countByAllocations
        /// <summary>
        /// Total exam results attached to a set of allocations - used by the
        /// bulk deallocation dialog to warn how many results will be deleted.
        /// </summary>
        [HttpPost("countByAllocations")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CountByAllocations([FromBody] List<int> studentSubjectIds)
        {
            try
            {
                return Ok(await _unitOfWork.ExamResults.CountByAllocationIds(studentSubjectIds));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while counting results by allocations.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/examResults/getStudentPerformance/5/5/5/5
        /// <summary>
        /// A method for retrieving student performance.
        /// </summary>
        /// <param name="sessionId">The exam result session Id whose exam results are to be retrieved</param>
        /// <param name="schoolClassId">The exam result school class Id whose exam results are to be retrieved</param>
        /// <param name="examNameId">The exam result exam name Id whose exam results are to be retrieved</param>
        /// <param name="studentId">The exam result student Id whose exam results are to be retrieved</param>
        /// <returns></returns>
        [HttpGet("getStudentPerformance/{sessionId}/{schoolClassId}/{examNameId}/{studentId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentPerformanceDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStudentExamPerformance(int sessionId, int schoolClassId, int examNameId, int studentId)
        {
            try
            {
                if (sessionId <= 0) return BadRequest(sessionId);
                if (schoolClassId <= 0) return BadRequest(schoolClassId);
                if (examNameId <= 0) return BadRequest(examNameId);
                if (studentId <= 0) return BadRequest(studentId);
                var _item = await _unitOfWork.ExamResults.GetStudentPerformace(sessionId, schoolClassId, examNameId, studentId);
                if (_item == null) return NotFound();
                return Ok(_item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the exam results for a student.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/examResults/getBroadSheet/5/5/5
        /// <summary>
        /// A method for retrieving broadsheet.
        /// </summary>
        /// <param name="sessionId">The exam result session Id whose exam results are to be retrieved</param>
        /// <param name="schoolClassId">The exam result school class Id whose exam results are to be retrieved</param>
        /// <param name="examTypeId">The exam result exam type Id whose exam results are to be retrieved</param>
        /// <param name="examNameId">The exam result exam name Id whose exam results are to be retrieved</param>
        /// <returns></returns>
        [HttpGet("getBroadSheet/{sessionId}/{schoolClassId}/{examTypeId}/{examNameId?}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<RankedStudentExamTypePerformanceDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBroadSheet(int sessionId, int schoolClassId, int examTypeId, int? examNameId)
        {
            try
            {
                if (sessionId <= 0) return BadRequest(sessionId);
                if (schoolClassId <= 0) return BadRequest(schoolClassId);
                if (examNameId <= 0) return BadRequest(examNameId);
                var _item = await _unitOfWork.ExamResults.GetClassExamTypeRanking(sessionId, schoolClassId, examTypeId);
                if (_item == null) return NotFound();
                return Ok(_item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the exam results.");
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
                if (!await _unitOfWork.Students.ItemExistsAsync(s => s.Id == model.StudentId))
                    return Conflict(new { message = $"The student details submitted does not exist." });
                if (!await _unitOfWork.Exams.ItemExistsAsync(s => s.Id == model.ExamId))
                    return Conflict(new { message = $"The exam details submitted do not exist." });

                var authBlock = await CheckCanWriteResultsAsync(new[] { model.ExamId });
                if (authBlock != null) return authBlock;

                // A result must belong to a subject allocation.
                var studentSubjectId = await ResolveStudentSubjectIdAsync(model.ExamId, model.StudentId);
                if (studentSubjectId == null)
                    return Conflict(new { message = "The student is not allocated this exam's subject, so a result cannot be recorded." });

                try
                {
                    var _item = _mapper.Map<ExamResult>(model);
                    _item.StudentSubjectId = studentSubjectId.Value;
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

            var authBlock = await CheckCanWriteResultsAsync(model.Select(m => m.ExamId));
            if (authBlock != null) return authBlock;

            if (ModelState.IsValid)
            {
                try
                {
                    // Cache (studentId -> studentSubjectId) per exam so we resolve
                    // each exam's allocations once, not per row.
                    var allocationCache = new Dictionary<int, Dictionary<int, int>>();

                    foreach (var item in model)
                    {
                        if (item.Score != null)
                        {
                            var existingExamResult = await _unitOfWork.ExamResults.GetByStudentExamId(item.StudentId, item.ExamId);

                            if (existingExamResult != null)
                            {
                                existingExamResult.Score = (float)item.Score;
                                existingExamResult.Description = item.Description;
                                _unitOfWork.ExamResults.Update(existingExamResult);
                            }
                            else
                            {
                                // A result must belong to a subject allocation. If
                                // the student isn't allocated the exam's subject in
                                // its class, skip the row (don't create an orphan).
                                var studentSubjectId = await ResolveStudentSubjectIdAsync(item.ExamId, item.StudentId, allocationCache);
                                if (studentSubjectId == null) continue;

                                var _item = _mapper.Map<ExamResult>(item);
                                _item.StudentSubjectId = studentSubjectId.Value;
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

                var authBlock = await CheckCanWriteResultsAsync(new[] { itemExists.ExamId });
                if (authBlock != null) return authBlock;

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
