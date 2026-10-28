using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.API.Services.CBE;
using SchoolWebApp.API.Licensing;

namespace SchoolWebApp.API.Controllers.CBE.Assessments
{
    /// <summary>
    /// A subject's curriculum for one grade in Excel: download the template
    /// (pre-filled with what is saved), fill it from the KICD curriculum design,
    /// upload it - checked first, saved on confirm.
    /// </summary>
    [Route("api/curriculumImports")]
    [ApiController]
    [Authorize(Roles = "Administrator,SuperAdministrator")]
    [RequiresModule(AppModules.Academics)]
    public class CurriculumImportsController : ControllerBase
    {
        private const string XlsxType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        private readonly CurriculumWorkbookService _workbooks;

        public CurriculumImportsController(CurriculumWorkbookService workbooks)
        {
            _workbooks = workbooks;
        }

        // GET api/curriculumImports/template?subjectId=12&learningLevelId=1
        [HttpGet("template")]
        public async Task<IActionResult> Template(int subjectId, int learningLevelId)
        {
            var file = await _workbooks.TemplateAsync(subjectId, learningLevelId);
            if (file == null) return BadRequest(new { message = "Choose the subject and grade." });
            return File(file.Value.Content, XlsxType, file.Value.FileName);
        }

        // POST api/curriculumImports?commit=false - checks; commit=true saves when there are no errors.
        [HttpPost]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<IActionResult> Upload(IFormFile file, bool commit = false)
        {
            if (file == null || file.Length == 0) return BadRequest(new { message = "Choose the workbook to upload." });
            if (!file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { message = "Upload the Excel workbook (.xlsx) downloaded from ShuleNova." });
            await using var stream = file.OpenReadStream();
            return Ok(await _workbooks.ImportAsync(stream, commit));
        }
    }
}
