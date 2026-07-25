using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.DTOs.Academics.SubjectAnalysisNote;
using SchoolWebApp.Core.Entities.Academics;

namespace SchoolWebApp.API.Controllers.Academics
{
    /// <summary>
    /// Persists the teacher-authored action plan shown on the Subject Performance
    /// Trend report. One note per (year, class, subject); everything else on that
    /// report is recomputed on load.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectAnalysisNotesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly ILogger<SubjectAnalysisNotesController> _logger;

        public SubjectAnalysisNotesController(
            ApplicationDbContext db,
            IMapper mapper,
            ILogger<SubjectAnalysisNotesController> logger)
        {
            _db = db;
            _mapper = mapper;
            _logger = logger;
        }

        // GET api/subjectAnalysisNotes/byParams?academicYearId=&schoolClassId=&subjectId=
        /// <summary>
        /// Returns the saved action-plan note for a (year, class, subject), or an
        /// empty 200 body when none has been saved yet.
        /// </summary>
        [HttpGet("byParams")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SubjectAnalysisNoteDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByParams(
            [FromQuery] int academicYearId,
            [FromQuery] int schoolClassId,
            [FromQuery] int subjectId,
            [FromQuery] int schoolExamId)
        {
            // subjectId == 0 is the class-level note (Class Performance Trend);
            // a real subject id is a subject-level note. schoolExamId fixes the
            // exam/term the plan is written for.
            if (academicYearId <= 0 || schoolClassId <= 0 || subjectId < 0 || schoolExamId <= 0)
                return BadRequest("academicYearId, schoolClassId and schoolExamId are required.");
            try
            {
                var note = await _db.SubjectAnalysisNotes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(n => n.AcademicYearId == academicYearId
                        && n.SchoolClassId == schoolClassId
                        && n.SubjectId == subjectId
                        && n.SchoolExamId == schoolExamId);
                if (note == null) return Ok();
                return Ok(_mapper.Map<SubjectAnalysisNoteDto>(note));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the subject analysis note.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // PUT api/subjectAnalysisNotes/upsert
        /// <summary>
        /// Creates or updates the action-plan note for a (year, class, subject).
        /// </summary>
        [HttpPut("upsert")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SubjectAnalysisNoteDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Upsert(SubjectAnalysisNoteDto model)
        {
            if (model == null || model.AcademicYearId <= 0 || model.SchoolClassId <= 0 || model.SubjectId < 0 || model.SchoolExamId <= 0)
                return BadRequest("academicYearId, schoolClassId and schoolExamId are required.");
            try
            {
                var existing = await _db.SubjectAnalysisNotes
                    .FirstOrDefaultAsync(n => n.AcademicYearId == model.AcademicYearId
                        && n.SchoolClassId == model.SchoolClassId
                        && n.SubjectId == model.SubjectId
                        && n.SchoolExamId == model.SchoolExamId);

                if (existing != null)
                {
                    existing.ActionPlan = model.ActionPlan;
                    existing.Modified = DateTime.Now;
                }
                else
                {
                    existing = new SubjectAnalysisNote
                    {
                        AcademicYearId = model.AcademicYearId,
                        SchoolClassId = model.SchoolClassId,
                        SubjectId = model.SubjectId,
                        SchoolExamId = model.SchoolExamId,
                        ActionPlan = model.ActionPlan,
                        Created = DateTime.Now
                    };
                    _db.SubjectAnalysisNotes.Add(existing);
                }

                await _db.SaveChangesAsync();
                return Ok(_mapper.Map<SubjectAnalysisNoteDto>(existing));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the subject analysis note.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
