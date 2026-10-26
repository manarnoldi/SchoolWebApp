using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.API.Services.CBE;
using SchoolWebApp.Core.DTOs.CBE;
using SchoolWebApp.Core.Entities.CBE.Assessments;

namespace SchoolWebApp.API.Controllers.CBE.Assessments
{
    /// <summary>
    /// Editing the parts of a curriculum design with no screen of their own:
    /// the design text and lists, each sub-strand's core competency statements,
    /// values, links to other learning areas and suggestions, and the rubrics.
    /// Strands, sub-strands, outcomes, experiences, questions and PCIs keep
    /// their own CBE Settings pages.
    /// </summary>
    [Route("api/curriculumDesign")]
    [ApiController]
    [Authorize(Roles = "Administrator,SuperAdministrator")]
    public class CurriculumDesignEditorController : ControllerBase
    {
        private const int MaxName = 255, MaxItem = 1000;
        private static readonly string[] Categories =
        {
            CurriculumDesignItem.Resource, CurriculumDesignItem.AssessmentMethod,
            CurriculumDesignItem.AssessmentTool, CurriculumDesignItem.NonFormalProgramme
        };

        private readonly ApplicationDbContext _db;

        public CurriculumDesignEditorController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET api/curriculumDesign?subjectId=12&learningLevelId=1
        [HttpGet]
        public async Task<IActionResult> Get(int subjectId, int learningLevelId)
        {
            if (!await _db.Subjects.AnyAsync(s => s.Id == subjectId) || !await _db.LearningLevels.AnyAsync(l => l.Id == learningLevelId))
                return NotFound(new { message = "Choose the subject and grade." });

            var design = await _db.CurriculumDesigns.AsNoTracking().Include(d => d.Items).Include(d => d.Sections)
                .FirstOrDefaultAsync(d => d.SubjectId == subjectId && d.LearningLevelId == learningLevelId);
            var strands = await _db.Strands.AsNoTracking()
                .Where(s => s.SubjectId == subjectId && s.LearningLevelId == learningLevelId)
                .Include(s => s.Rubrics)
                .Include(s => s.SubStrands).ThenInclude(ss => ss.CoreCompetencies)
                .Include(s => s.SubStrands).ThenInclude(ss => ss.Values)
                .Include(s => s.SubStrands).ThenInclude(ss => ss.LearningAreaLinks)
                .AsSplitQuery()
                .OrderBy(s => s.Rank).ThenBy(s => s.Code).ThenBy(s => s.Id)
                .ToListAsync();

            return Ok(new CurriculumDesignEditDto
            {
                SubjectId = subjectId,
                LearningLevelId = learningLevelId,
                Title = design?.Title,
                Source = design?.Source,
                EssenceStatement = design?.EssenceStatement,
                LessonsPerWeek = design?.LessonsPerWeek,
                Items = (design?.Items ?? new()).OrderBy(i => Array.IndexOf(Categories, i.Category)).ThenBy(i => i.Rank)
                    .Select(i => new EditItemDto { Category = i.Category, Name = i.Name }).ToList(),
                Sections = (design?.Sections ?? new()).OrderBy(s => s.Rank)
                    .Select(s => new EditSectionDto { Title = s.Title, Content = s.Content }).ToList(),
                Strands = strands.Select(s => new EditStrandDto
                {
                    Id = s.Id,
                    Code = s.Code,
                    Name = s.Name,
                    Rubrics = s.Rubrics.OrderBy(r => r.Rank).Select(r => new EditRubricDto
                    {
                        SubStrandId = r.SubStrandId, Indicator = r.Indicator,
                        ExceedsExpectations = r.ExceedsExpectations, MeetsExpectations = r.MeetsExpectations,
                        ApproachesExpectations = r.ApproachesExpectations, BelowExpectations = r.BelowExpectations
                    }).ToList(),
                    SubStrands = s.SubStrands.OrderBy(ss => ss.Rank).ThenBy(ss => ss.Code).ThenBy(ss => ss.Id).Select(ss => new EditSubStrandDto
                    {
                        Id = ss.Id,
                        Code = ss.Code,
                        Name = ss.Name,
                        CoreCompetencies = ss.CoreCompetencies.OrderBy(x => x.Rank)
                            .Select(x => new EditStatementDto { Name = x.Name, Description = x.Description, LinkId = x.CompetencyId }).ToList(),
                        Values = ss.Values.OrderBy(x => x.Rank)
                            .Select(x => new EditStatementDto { Name = x.Name, Description = x.Description, LinkId = x.ValueId }).ToList(),
                        LearningAreaLinks = ss.LearningAreaLinks.OrderBy(x => x.Rank)
                            .Select(x => new EditStatementDto { Name = x.LearningArea, Description = x.Description, LinkId = x.SubjectId }).ToList(),
                        SuggestedResources = ss.SuggestedResources,
                        SuggestedAssessmentMethods = ss.SuggestedAssessmentMethods,
                        SuggestedNonFormalActivities = ss.SuggestedNonFormalActivities
                    }).ToList()
                }).ToList(),
                Competencies = await _db.Competencies.OrderBy(c => c.Rank).ThenBy(c => c.Name)
                    .Select(c => new LookupDto { Id = c.Id, Name = c.Name }).ToListAsync(),
                Values = await _db.Values.OrderBy(v => v.Rank).ThenBy(v => v.Name)
                    .Select(v => new LookupDto { Id = v.Id, Name = v.Name }).ToListAsync(),
                Subjects = await _db.Subjects.OrderBy(s => s.Rank).ThenBy(s => s.Name)
                    .Select(s => new LookupDto { Id = s.Id, Name = s.Name }).ToListAsync()
            });
        }

        // PUT api/curriculumDesign/design - title, source, essence, lessons per week, appendix lists, other sections.
        [HttpPut("design")]
        public async Task<IActionResult> SaveDesign(CurriculumDesignEditDto model)
        {
            if (!await _db.Subjects.AnyAsync(s => s.Id == model.SubjectId) || !await _db.LearningLevels.AnyAsync(l => l.Id == model.LearningLevelId))
                return BadRequest(new { message = "Choose the subject and grade." });
            if (model.Title?.Length > MaxName) return BadRequest(new { message = $"The title is longer than {MaxName} characters." });
            if (model.Source?.Length > MaxName) return BadRequest(new { message = $"The source is longer than {MaxName} characters." });
            if (model.LessonsPerWeek is < 1 or > 40) return BadRequest(new { message = "Lessons per week must be between 1 and 40." });
            var items = model.Items.Where(i => !string.IsNullOrWhiteSpace(i.Name)).ToList();
            if (items.Any(i => !Categories.Contains(i.Category))) return BadRequest(new { message = "Pick the list for every appendix item." });
            if (items.Any(i => i.Name.Trim().Length > MaxItem)) return BadRequest(new { message = $"An appendix item is longer than {MaxItem} characters." });
            var sections = model.Sections.Where(s => !string.IsNullOrWhiteSpace(s.Title) || !string.IsNullOrWhiteSpace(s.Content)).ToList();
            if (sections.Any(s => string.IsNullOrWhiteSpace(s.Title))) return BadRequest(new { message = "Give every section its heading." });
            if (sections.Any(s => s.Title.Trim().Length > MaxName)) return BadRequest(new { message = $"A section heading is longer than {MaxName} characters." });

            var design = await _db.CurriculumDesigns.Include(d => d.Items).Include(d => d.Sections)
                .FirstOrDefaultAsync(d => d.SubjectId == model.SubjectId && d.LearningLevelId == model.LearningLevelId);
            if (design == null)
            {
                design = new CurriculumDesign { SubjectId = model.SubjectId, LearningLevelId = model.LearningLevelId };
                _db.CurriculumDesigns.Add(design);
            }
            design.Title = Blank(model.Title);
            design.Source = Blank(model.Source);
            design.EssenceStatement = Blank(model.EssenceStatement);
            design.LessonsPerWeek = model.LessonsPerWeek;
            _db.CurriculumDesignItems.RemoveRange(design.Items);
            _db.CurriculumDesignSections.RemoveRange(design.Sections);
            design.Items = items.Select((i, n) => new CurriculumDesignItem { Category = i.Category, Name = i.Name.Trim(), Rank = n + 1 }).ToList();
            design.Sections = sections.Select((s, n) => new CurriculumDesignSection { Title = s.Title.Trim(), Content = Blank(s.Content), Rank = n + 1 }).ToList();

            // The design's time allocation is the subject's lesson allocation for the grade.
            if (model.LessonsPerWeek is int perWeek)
            {
                var allocation = await _db.Set<LessonAllocation>()
                    .FirstOrDefaultAsync(a => a.SubjectId == model.SubjectId && a.LearningLevelId == model.LearningLevelId);
                if (allocation == null)
                {
                    allocation = new LessonAllocation { SubjectId = model.SubjectId, LearningLevelId = model.LearningLevelId };
                    _db.Set<LessonAllocation>().Add(allocation);
                }
                allocation.LessonsPerWeek = perWeek;
                allocation.Description ??= "From the curriculum design";
            }
            await _db.SaveChangesAsync();
            return Ok();
        }

        // PUT api/curriculumDesign/subStrands/5 - its competency statements, values, links and suggestions.
        [HttpPut("subStrands/{id}")]
        public async Task<IActionResult> SaveSubStrand(int id, EditSubStrandDto model)
        {
            var sub = await _db.SubStrands
                .Include(ss => ss.CoreCompetencies).Include(ss => ss.Values).Include(ss => ss.LearningAreaLinks)
                .Include(ss => ss.SpecificOutcomes).ThenInclude(o => o.Competencies)
                .FirstOrDefaultAsync(ss => ss.Id == id);
            if (sub == null) return NotFound();
            var all = model.CoreCompetencies.Concat(model.Values).Concat(model.LearningAreaLinks).Where(x => !string.IsNullOrWhiteSpace(x.Name)).ToList();
            if (all.Any(x => x.Name.Trim().Length > MaxName))
                return BadRequest(new { message = $"A name is longer than {MaxName} characters - put the rest in its statement." });

            var competencies = await _db.Competencies.ToListAsync();
            var values = await _db.Values.Select(v => new { v.Id, v.Name }).ToListAsync();
            var subjects = await _db.Subjects.Select(s => new { s.Id, s.Name }).ToListAsync();
            // An entry left unlinked is linked by its name, as on upload.
            int? Match(EditStatementDto x, IEnumerable<(int Id, string Name)> list) =>
                x.LinkId is int lid && list.Any(l => l.Id == lid) ? lid
                    : list.Where(l => CurriculumWorkbookService.Key(l.Name) == CurriculumWorkbookService.Key(x.Name.Split(':')[0]))
                        .Select(l => (int?)l.Id).FirstOrDefault();

            _db.SubStrandCompetencies.RemoveRange(sub.CoreCompetencies);
            _db.SubStrandValues.RemoveRange(sub.Values);
            _db.SubStrandLearningAreaLinks.RemoveRange(sub.LearningAreaLinks);
            sub.CoreCompetencies = model.CoreCompetencies.Where(x => !string.IsNullOrWhiteSpace(x.Name)).Select((x, n) => new SubStrandCompetency
            {
                Name = x.Name.Trim(), Description = Blank(x.Description), Rank = n + 1,
                CompetencyId = Match(x, competencies.Select(c => (c.Id, c.Name)))
            }).ToList();
            sub.Values = model.Values.Where(x => !string.IsNullOrWhiteSpace(x.Name)).Select((x, n) => new SubStrandValue
            {
                Name = x.Name.Trim(), Description = Blank(x.Description), Rank = n + 1,
                ValueId = Match(x, values.Select(v => (v.Id, v.Name)))
            }).ToList();
            sub.LearningAreaLinks = model.LearningAreaLinks.Where(x => !string.IsNullOrWhiteSpace(x.Name)).Select((x, n) => new SubStrandLearningAreaLink
            {
                LearningArea = x.Name.Trim(), Description = Blank(x.Description), Rank = n + 1,
                SubjectId = x.LinkId is int sid && subjects.Any(s => s.Id == sid) ? sid
                    : CurriculumWorkbookService.SubjectFor(x.Name, subjects.Select(s => (s.Id, s.Name)))
            }).ToList();
            sub.SuggestedResources = Blank(model.SuggestedResources);
            sub.SuggestedAssessmentMethods = Blank(model.SuggestedAssessmentMethods);
            sub.SuggestedNonFormalActivities = Blank(model.SuggestedNonFormalActivities);

            // The sub-strand's outcomes develop the competencies listed under it.
            var linked = competencies.Where(c => sub.CoreCompetencies.Any(x => x.CompetencyId == c.Id)).ToList();
            foreach (var o in sub.SpecificOutcomes)
            {
                o.Competencies.Clear();
                o.Competencies.AddRange(linked);
            }
            await _db.SaveChangesAsync();
            return Ok();
        }

        // PUT api/curriculumDesign/strands/5/rubrics - replaces the strand's rubrics.
        [HttpPut("strands/{id}/rubrics")]
        public async Task<IActionResult> SaveRubrics(int id, List<EditRubricDto> model)
        {
            var strand = await _db.Strands.Include(s => s.Rubrics).Include(s => s.SubStrands).FirstOrDefaultAsync(s => s.Id == id);
            if (strand == null) return NotFound();
            var rows = model.Where(r => !string.IsNullOrWhiteSpace(r.Indicator) || !string.IsNullOrWhiteSpace(r.ExceedsExpectations)
                || !string.IsNullOrWhiteSpace(r.MeetsExpectations)).ToList();
            if (rows.Any(r => string.IsNullOrWhiteSpace(r.Indicator))) return BadRequest(new { message = "Every rubric row needs its indicator." });
            if (rows.Any(r => r.SubStrandId != null && !strand.SubStrands.Any(ss => ss.Id == r.SubStrandId)))
                return BadRequest(new { message = "A rubric row names a sub-strand that is not in this strand." });

            _db.AssessmentRubrics.RemoveRange(strand.Rubrics);
            strand.Rubrics = rows.Select((r, n) => new AssessmentRubric
            {
                StrandId = strand.Id, SubStrandId = r.SubStrandId, Rank = n + 1, Indicator = r.Indicator.Trim(),
                ExceedsExpectations = Blank(r.ExceedsExpectations), MeetsExpectations = Blank(r.MeetsExpectations),
                ApproachesExpectations = Blank(r.ApproachesExpectations), BelowExpectations = Blank(r.BelowExpectations)
            }).ToList();
            await _db.SaveChangesAsync();
            return Ok();
        }

        private static string? Blank(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();
    }
}
