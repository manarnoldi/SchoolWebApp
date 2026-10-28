using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.DTOs.CBE;
using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.API.Licensing;
using SchoolWebApp.API.Security;

namespace SchoolWebApp.API.Controllers.CBE.Assessments
{
    /// <summary>
    /// A subject's whole curriculum design for one grade, for reading: strands,
    /// sub-strands with everything under them, rubrics, and the design's other
    /// parts. Open to every signed-in user - teachers plan from it.
    /// </summary>
    [Route("api/curriculum")]
    [ApiController]
    [Authorize]
    [RequiresModule(AppModules.Academics)]
    [RequiresMenu(MenuGroup.Academics, WritesOnly = true, LogOnly = true)]
    public class CurriculumController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public CurriculumController(ApplicationDbContext db)
        {
            _db = db;
        }

        private static readonly (string Category, string Label)[] AppendixLabels =
        {
            (CurriculumDesignItem.Resource, "Suggested Resources"),
            (CurriculumDesignItem.AssessmentMethod, "Suggested Assessment Methods"),
            (CurriculumDesignItem.AssessmentTool, "Suggested Assessment Tools"),
            (CurriculumDesignItem.NonFormalProgramme, "Non-formal Programmes to Support Learning")
        };

        // GET api/curriculum/subjects?learningLevelId=3 - the subjects taught at a grade:
        // those set for its education level (in any academic year) and any with curriculum loaded for it.
        [HttpGet("subjects")]
        public async Task<IActionResult> Subjects(int learningLevelId)
        {
            var level = await _db.LearningLevels.AsNoTracking().FirstOrDefaultAsync(l => l.Id == learningLevelId);
            if (level == null) return NotFound(new { message = "Choose the grade." });
            var ids = await _db.EducationLevelSubjects.AsNoTracking()
                .Where(e => e.EducationLevelId == level.EducationLevelId).Select(e => e.SubjectId)
                .Union(_db.Strands.AsNoTracking().Where(s => s.LearningLevelId == learningLevelId).Select(s => s.SubjectId))
                .ToListAsync();
            var subjects = await _db.Subjects.AsNoTracking().Where(s => ids.Contains(s.Id))
                .OrderBy(s => s.Rank).ThenBy(s => s.Name)
                .Select(s => new { s.Id, s.Name, s.Code }).ToListAsync();
            return Ok(subjects);
        }

        // GET api/curriculum?subjectId=12&learningLevelId=1
        [HttpGet]
        public async Task<IActionResult> Get(int subjectId, int learningLevelId)
        {
            var subject = await _db.Subjects.AsNoTracking().FirstOrDefaultAsync(s => s.Id == subjectId);
            var level = await _db.LearningLevels.AsNoTracking().FirstOrDefaultAsync(l => l.Id == learningLevelId);
            if (subject == null || level == null) return NotFound(new { message = "Choose the subject and grade." });

            var strands = await _db.Strands.AsNoTracking()
                .Where(s => s.SubjectId == subjectId && s.LearningLevelId == learningLevelId)
                .Include(s => s.Rubrics)
                .Include(s => s.SubStrands).ThenInclude(ss => ss.SpecificOutcomes).ThenInclude(o => o.BroadOutcome)
                .Include(s => s.SubStrands).ThenInclude(ss => ss.SpecificOutcomes).ThenInclude(o => o.GeneralOutcome)
                .Include(s => s.SubStrands).ThenInclude(ss => ss.LearningExperiences)
                .Include(s => s.SubStrands).ThenInclude(ss => ss.Questions)
                .Include(s => s.SubStrands).ThenInclude(ss => ss.PCIs)
                .Include(s => s.SubStrands).ThenInclude(ss => ss.CoreCompetencies)
                .Include(s => s.SubStrands).ThenInclude(ss => ss.Values)
                .Include(s => s.SubStrands).ThenInclude(ss => ss.LearningAreaLinks)
                .AsSplitQuery()
                .OrderBy(s => s.Rank).ThenBy(s => s.Code).ThenBy(s => s.Id)
                .ToListAsync();
            var design = await _db.CurriculumDesigns.AsNoTracking().Include(d => d.Items).Include(d => d.Sections)
                .FirstOrDefaultAsync(d => d.SubjectId == subjectId && d.LearningLevelId == learningLevelId);
            var subjectOutcomes = await _db.BroadOutcomes.AsNoTracking()
                .Where(o => o.SubjectId == subjectId && o.EducationLevelId == level.EducationLevelId)
                .OrderBy(o => o.Rank).ThenBy(o => o.Id).Select(o => o.Name).ToListAsync();

            static CurriculumRubricDto Rubric(AssessmentRubric r) => new()
            {
                Indicator = r.Indicator,
                ExceedsExpectations = r.ExceedsExpectations,
                MeetsExpectations = r.MeetsExpectations,
                ApproachesExpectations = r.ApproachesExpectations,
                BelowExpectations = r.BelowExpectations
            };

            var view = new CurriculumViewDto
            {
                SubjectId = subject.Id,
                SubjectName = subject.Name,
                LearningLevelId = level.Id,
                LearningLevelName = level.Name,
                Title = design?.Title,
                Source = design?.Source,
                EssenceStatement = design?.EssenceStatement,
                LessonsPerWeek = design?.LessonsPerWeek,
                SubjectOutcomes = subjectOutcomes,
                Strands = strands.Select(s => new CurriculumStrandViewDto
                {
                    Id = s.Id,
                    Code = s.Code,
                    Name = s.Name,
                    Rubrics = s.Rubrics.Where(r => r.SubStrandId == null).OrderBy(r => r.Rank).Select(Rubric).ToList(),
                    SubStrands = s.SubStrands.OrderBy(ss => ss.Rank).ThenBy(ss => ss.Code).ThenBy(ss => ss.Id).Select(ss =>
                    {
                        var outcomes = ss.SpecificOutcomes.OrderBy(o => o.Rank).ThenBy(o => o.Id).ToList();
                        return new CurriculumSubStrandViewDto
                        {
                            Id = ss.Id,
                            Code = ss.Code,
                            Name = ss.Name,
                            Lessons = ss.LessonNo,
                            SpecificOutcomes = outcomes.Select(o => o.Name).ToList(),
                            LearningExperiences = ss.LearningExperiences.OrderBy(x => x.Rank).ThenBy(x => x.Id).Select(x => x.Name).ToList(),
                            KeyQuestions = ss.Questions.OrderBy(x => x.Rank).ThenBy(x => x.Id).Select(x => x.Name).ToList(),
                            CoreCompetencies = ss.CoreCompetencies.OrderBy(x => x.Rank)
                                .Select(x => new CurriculumStatementDto { Name = x.Name, Description = x.Description }).ToList(),
                            Values = ss.Values.OrderBy(x => x.Rank)
                                .Select(x => new CurriculumStatementDto { Name = x.Name, Description = x.Description }).ToList(),
                            Pcis = ss.PCIs.OrderBy(x => x.Rank).ThenBy(x => x.Id)
                                .Select(x => new CurriculumStatementDto { Name = x.Name, Description = x.Description }).ToList(),
                            LearningAreaLinks = ss.LearningAreaLinks.OrderBy(x => x.Rank)
                                .Select(x => new CurriculumStatementDto { Name = x.LearningArea, Description = x.Description }).ToList(),
                            SubjectOutcome = outcomes.Select(o => o.BroadOutcome?.Name).FirstOrDefault(),
                            LevelOutcome = outcomes.Select(o => o.GeneralOutcome?.Name).FirstOrDefault(),
                            SuggestedResources = ss.SuggestedResources,
                            SuggestedAssessmentMethods = ss.SuggestedAssessmentMethods,
                            SuggestedNonFormalActivities = ss.SuggestedNonFormalActivities,
                            Rubrics = s.Rubrics.Where(r => r.SubStrandId == ss.Id).OrderBy(r => r.Rank).Select(Rubric).ToList()
                        };
                    }).ToList()
                }).ToList(),
                Appendix = AppendixLabels
                    .Select(a => new CurriculumAppendixListDto
                    {
                        Label = a.Label,
                        Items = (design?.Items ?? new()).Where(i => i.Category == a.Category).OrderBy(i => i.Rank).Select(i => i.Name).ToList()
                    })
                    .Where(a => a.Items.Count > 0).ToList(),
                Sections = (design?.Sections ?? new()).OrderBy(s => s.Rank)
                    .Select(s => new CurriculumSectionDto { Title = s.Title, Content = s.Content }).ToList()
            };
            view.TotalLessons = view.Strands.Sum(s => s.SubStrands.Sum(ss => ss.Lessons));
            return Ok(view);
        }
    }
}
