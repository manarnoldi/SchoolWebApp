using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.API.Services.TeachingDocuments;
using SchoolWebApp.API.Licensing;

namespace SchoolWebApp.API.Controllers.TeachingDocuments
{
    /// <summary>
    /// Schemes of work in Excel: download a workbook (per teacher and term, or
    /// for one scheme), fill it in, and upload it - checked and previewed first,
    /// saved on confirm. The teacher, the head of the subject's department and
    /// administrators can download and upload.
    /// </summary>
    [Route("api/schemeImports")]
    [ApiController]
    [Authorize]
    [RequiresModule(AppModules.Academics)]
    public class SchemeImportsController : ControllerBase
    {
        private const string XlsxType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        private const long MaxUploadBytes = 10 * 1024 * 1024;

        private readonly TeachingDocumentService _svc;
        private readonly SchemeWorkbookService _workbooks;

        public SchemeImportsController(TeachingDocumentService svc, SchemeWorkbookService workbooks)
        {
            _svc = svc;
            _workbooks = workbooks;
        }

        // GET api/schemeImports/teacherWorkbook?sessionId=6&staffDetailsId=2678
        // Every subject and class the teacher has that term; a head of department
        // gets only the ones in their departments. Without staffDetailsId, the caller's own.
        [HttpGet("teacherWorkbook")]
        public async Task<IActionResult> TeacherWorkbook(int sessionId, int? staffDetailsId)
        {
            var me = await _svc.CurrentAsync();
            if (me == null) return Unauthorized();
            var staffId = staffDetailsId ?? me.StaffDetailsId;
            if (staffId == null) return BadRequest(new { message = "Your account is not linked to a staff record." });
            var file = await _workbooks.TeacherWorkbookAsync(me, sessionId, staffId.Value);
            if (file == null) return BadRequest(new { message = "There are no subjects for this teacher and term that you can prepare schemes for." });
            return File(file.Value.Content, XlsxType, file.Value.FileName);
        }

        // GET api/schemeImports/schemeWorkbook/5
        [HttpGet("schemeWorkbook/{schemeId}")]
        public async Task<IActionResult> SchemeWorkbook(int schemeId)
        {
            var me = await _svc.CurrentAsync();
            if (me == null) return Unauthorized();
            var file = await _workbooks.SchemeWorkbookAsync(me, schemeId);
            if (file == null) return BadRequest(new { message = "You cannot download this scheme for editing." });
            return File(file.Value.Content, XlsxType, file.Value.FileName);
        }

        // POST api/schemeImports?commit=false - checks the workbook and reports
        // by sheet and row; commit=true then saves the sheets without errors.
        [HttpPost]
        [RequestSizeLimit(MaxUploadBytes)]
        public async Task<IActionResult> Upload(IFormFile file, bool commit = false)
        {
            var me = await _svc.CurrentAsync();
            if (me == null) return Unauthorized();
            if (file == null || file.Length == 0) return BadRequest(new { message = "Choose the workbook to upload." });
            if (!file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { message = "Upload the Excel workbook (.xlsx) downloaded from ShuleNova." });
            await using var stream = file.OpenReadStream();
            return Ok(await _workbooks.ImportAsync(me, stream, commit));
        }
    }
}
