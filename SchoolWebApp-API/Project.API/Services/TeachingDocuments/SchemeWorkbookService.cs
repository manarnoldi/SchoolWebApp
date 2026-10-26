using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.DTOs.TeachingDocuments;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.TeachingDocuments;
using System.Text.RegularExpressions;

namespace SchoolWebApp.API.Services.TeachingDocuments
{
    /// <summary>
    /// Schemes of work as an Excel workbook, for teachers who would rather type
    /// in Excel (or paste from their Word schemes) than on screen.
    ///
    /// Download: one workbook per teacher and term (or for one scheme), with an
    /// Instructions sheet, a sheet per subject and class laid out week by week
    /// (pre-filled with what is already saved), and a protected curriculum
    /// sheet per subject and grade that feeds the Strand / Sub-strand dropdowns.
    /// Each scheme sheet carries its teacher, subject, class and term in a
    /// hidden first row, so the upload knows where it belongs.
    ///
    /// Upload: every scheme sheet is checked and previewed; on confirm, sheets
    /// without errors are saved - lessons updated in place by week and lesson,
    /// so lesson plans and the record of work stay linked.
    /// </summary>
    public class SchemeWorkbookService
    {
        private const string Marker = "SHULENOVA-SCHEME-OF-WORK";
        private const int KeyRow = 1, TitleRow = 2, InfoRow = 3, RefRow = 4, NoteRow = 5, HeaderRow = 6, FirstDataRow = 7;
        // Blank rows left under the lessons for extra lessons.
        private const int SpareRows = 20;
        private const int MaxWeeks = 20, MaxLessons = 20, MaxName = 255;
        private const string WeekHeader = "Week";

        private static readonly (string Header, double Width)[] Columns =
        {
            ("Week", 6), ("Lesson", 7), ("Strand", 22), ("Sub-strand", 26),
            ("Lesson Learning Outcomes", 45), ("Learning Experiences", 45), ("Key Inquiry Question(s)", 30),
            ("Learning Resources", 30), ("Assessment Methods", 24)
        };
        private const int ColWeek = 1, ColLesson = 2, ColStrand = 3, ColSub = 4, ColOutcomes = 5, ColExperiences = 6,
            ColQuestions = 7, ColResources = 8, ColAssessment = 9;

        private const string OutcomesHeading = "By the end of the lesson, the learner should be able to:";
        private const string ExperiencesHeading = "The learner is guided to:";

        private readonly ApplicationDbContext _db;
        private readonly TeachingDocumentService _svc;

        public SchemeWorkbookService(ApplicationDbContext db, TeachingDocumentService svc)
        {
            _db = db;
            _svc = svc;
        }

        /// <summary>A subject x class x term for one teacher - an existing scheme or one the upload would create.</summary>
        private class Target
        {
            public int StaffDetailsId { get; set; }
            public string StaffName { get; set; } = "";
            public int SubjectId { get; set; }
            public string SubjectName { get; set; } = "";
            public string SubjectAbbr { get; set; } = "";
            public int SubjectRank { get; set; }
            public int DepartmentId { get; set; }
            public int SchoolClassId { get; set; }
            public string ClassName { get; set; } = "";
            public int LearningLevelId { get; set; }
            public string LearningLevelName { get; set; } = "";
            public Session Session { get; set; } = null!;
            public SchemeOfWork? Scheme { get; set; }
        }

        // ================= Download =================

        public async Task<(byte[] Content, string FileName)?> TeacherWorkbookAsync(TeachingUser me, int sessionId, int staffDetailsId)
        {
            var session = await _db.Sessions.AsNoTracking().Include(s => s.AcademicYear).FirstOrDefaultAsync(s => s.Id == sessionId);
            if (session == null) return null;
            var staffName = await _db.StaffDetails.Where(s => s.Id == staffDetailsId).Select(s => s.FullName).FirstOrDefaultAsync();
            if (staffName == null) return null;

            var allocations = await _db.StaffSubjects.AsNoTracking()
                .Where(a => a.StaffDetailsId == staffDetailsId && a.SchoolClass!.AcademicYearId == session.AcademicYearId)
                .Select(a => new { a.SubjectId, a.SchoolClassId }).ToListAsync();
            var schemes = await _db.SchemesOfWork.AsNoTracking().Include(s => s.Lessons)
                .Where(s => s.StaffDetailsId == staffDetailsId && s.SessionId == sessionId).ToListAsync();
            var keys = allocations.Select(a => (a.SubjectId, a.SchoolClassId))
                .Concat(schemes.Select(s => (s.SubjectId, s.SchoolClassId))).Distinct().ToList();

            var targets = new List<Target>();
            foreach (var (subjectId, classId) in keys)
            {
                var t = await TargetAsync(staffDetailsId, subjectId, classId, session);
                if (t == null || !me.CanUploadFor(staffDetailsId, t.DepartmentId)) continue;
                t.Scheme = schemes.FirstOrDefault(s => s.SubjectId == subjectId && s.SchoolClassId == classId);
                targets.Add(t);
            }
            if (targets.Count == 0) return null;
            targets = targets.OrderBy(t => t.LearningLevelName, StringComparer.OrdinalIgnoreCase)
                .ThenBy(t => t.ClassName).ThenBy(t => t.SubjectRank).ThenBy(t => t.SubjectName).ToList();
            return (await BuildAsync(targets), $"Schemes of work - {staffName} - {session.SessionName} {session.AcademicYear?.Name}.xlsx");
        }

        public async Task<(byte[] Content, string FileName)?> SchemeWorkbookAsync(TeachingUser me, int schemeId)
        {
            var s = await _db.SchemesOfWork.AsNoTracking().Include(x => x.Lessons).FirstOrDefaultAsync(x => x.Id == schemeId);
            if (s == null) return null;
            var session = await _db.Sessions.AsNoTracking().Include(x => x.AcademicYear).FirstAsync(x => x.Id == s.SessionId);
            var t = await TargetAsync(s.StaffDetailsId, s.SubjectId, s.SchoolClassId, session);
            if (t == null || !me.CanUploadFor(s.StaffDetailsId, t.DepartmentId)) return null;
            t.Scheme = s;
            return (await BuildAsync(new List<Target> { t }),
                $"Scheme of work - {t.SubjectName} {t.ClassName} - {session.SessionName} {session.AcademicYear?.Name}.xlsx");
        }

        private async Task<Target?> TargetAsync(int staffId, int subjectId, int classId, Session session)
        {
            var staff = await _db.StaffDetails.AsNoTracking().Where(s => s.Id == staffId).Select(s => s.FullName).FirstOrDefaultAsync();
            var subject = await _db.Subjects.AsNoTracking().FirstOrDefaultAsync(s => s.Id == subjectId);
            var cls = await _db.SchoolClasses.AsNoTracking().Include(c => c.LearningLevel).FirstOrDefaultAsync(c => c.Id == classId);
            if (staff == null || subject == null || cls == null) return null;
            return new Target
            {
                StaffDetailsId = staffId, StaffName = staff,
                SubjectId = subject.Id, SubjectName = subject.Name, SubjectAbbr = subject.Abbr, SubjectRank = subject.Rank, DepartmentId = subject.DepartmentId,
                SchoolClassId = cls.Id, ClassName = cls.Name, LearningLevelId = cls.LearningLevelId,
                LearningLevelName = cls.LearningLevel?.Name ?? "", Session = session
            };
        }

        private async Task<byte[]> BuildAsync(List<Target> targets)
        {
            using var wb = new XLWorkbook();
            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            Instructions(wb.AddWorksheet(Unique("Instructions", names)), targets);

            // One curriculum sheet per subject and grade, shared by its classes.
            var curriculumSheets = new Dictionary<(int, int), (string Sheet, int SubRows, int StrandRows)>();
            var schemeSheets = new List<(IXLWorksheet Ws, Target T)>();
            foreach (var t in targets)
            {
                var name = Unique(SheetName($"{Cut(t.SubjectName, 16)} {ShortClass(t.ClassName)}"), names);
                schemeSheets.Add((wb.AddWorksheet(name), t));
            }
            foreach (var t in targets)
            {
                var key = (t.SubjectId, t.LearningLevelId);
                if (curriculumSheets.ContainsKey(key)) continue;
                var curriculum = await _svc.CurriculumAsync(t.SubjectId, t.LearningLevelId, t.Session.Id);
                if (curriculum.Count == 0) continue;
                var name = Unique(SheetName($"Curr {Cut(t.SubjectName, 16)} {ShortLevel(t.LearningLevelName)}"), names);
                curriculumSheets[key] = CurriculumSheet(wb.AddWorksheet(name), t, curriculum);
            }

            foreach (var (ws, t) in schemeSheets)
            {
                var defaultPerWeek = t.Scheme?.LessonsPerWeek ?? await _svc.DefaultLessonsPerWeekAsync(t.SubjectId, t.LearningLevelId);
                var weeks = t.Scheme?.NumberOfWeeks ?? TeachingDocumentService.TermWeeks(t.Session);
                curriculumSheets.TryGetValue((t.SubjectId, t.LearningLevelId), out var curr);
                SchemeSheet(ws, t, weeks, defaultPerWeek, curr.Sheet == null ? null : curr);
            }

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return ms.ToArray();
        }

        private static void Instructions(IXLWorksheet ws, List<Target> targets)
        {
            var t = targets[0];
            var lines = new[]
            {
                $"Schemes of work - {t.Session.SessionName} {t.Session.AcademicYear?.Name}",
                "",
                "How to fill this workbook",
                "1. Each subject and class you teach has its own sheet. Fill one row per lesson, week by week.",
                "2. Pick the Strand from its dropdown, then the Sub-strand (the list shows that strand's sub-strands).",
                "   You may type your own strand or sub-strand if it is not in the list - Excel will ask you to confirm.",
                "3. Leave Lesson Learning Outcomes, Learning Experiences or Key Inquiry Questions blank to have them",
                "   filled from the curriculum when you upload - or type / paste your own.",
                "4. Press Alt+Enter for a new line inside a cell.",
                "5. Extra lesson in a week? Use the blank rows at the bottom, or insert a row, and give it the week and",
                "   lesson number. A week with no lessons (half-term, exams)? Clear or delete its rows.",
                "6. You can copy a table from your Word scheme and paste it in, as long as the columns line up.",
                "7. The 'Curr ...' sheets list the curriculum for each subject and grade, for reference.",
                "",
                "Uploading",
                "- In ShuleNova go to Teaching Documents > Schemes of Work and choose Upload workbook.",
                "- Every sheet is checked first and you see any problems by row before anything is saved.",
                "- Uploading replaces the scheme's lessons with the sheet's. Schemes already submitted or approved are not changed.",
                "- Do not rename the header rows or move the columns. Renaming a sheet is fine.",
                "",
                "Sheets in this workbook"
            };
            for (var i = 0; i < lines.Length; i++) ws.Cell(i + 1, 1).Value = lines[i];
            ws.Cell(1, 1).Style.Font.SetBold().Font.SetFontSize(14);
            foreach (var r in new[] { 3, 15, 21 }) ws.Cell(r, 1).Style.Font.SetBold();
            var row = lines.Length + 1;
            foreach (var x in targets)
            {
                ws.Cell(row, 1).Value = $"- {x.SubjectName}, {x.ClassName} ({x.StaffName})"
                    + (x.Scheme == null ? " - new" : x.Scheme.Lessons.Any(HasContent) ? " - pre-filled with what is saved" : "");
                row++;
            }
            ws.Column(1).Width = 110;
        }

        private static (string Sheet, int SubRows, int StrandRows) CurriculumSheet(IXLWorksheet ws, Target t, List<CurriculumStrandDto> curriculum)
        {
            ws.Cell(1, 1).Value = $"{t.SubjectName} - {t.LearningLevelName} curriculum. Pick strands and sub-strands from these in the scheme sheets; "
                + "outcomes, experiences and questions left blank there are filled from here on upload.";
            ws.Cell(1, 1).Style.Font.SetBold();
            string[] headers = { "Strand", "Sub-strand", "Lessons", "Specific learning outcomes", "Learning experiences", "Key inquiry questions", "", "Strands" };
            for (var c = 0; c < headers.Length; c++) ws.Cell(2, c + 1).Value = headers[c];
            ws.Range(2, 1, 2, 6).Style.Font.SetBold().Fill.SetBackgroundColor(XLColor.LightGray);
            ws.Cell(2, 8).Style.Font.SetBold().Fill.SetBackgroundColor(XLColor.LightGray);

            // Rows grouped by strand, so a strand's sub-strands are one block -
            // the scheme sheets' sub-strand dropdown takes that block.
            var row = 3;
            foreach (var s in curriculum)
            {
                foreach (var ss in s.SubStrands)
                {
                    ws.Cell(row, 1).Value = s.Name;
                    ws.Cell(row, 2).Value = ss.Name;
                    ws.Cell(row, 3).Value = ss.Lessons;
                    ws.Cell(row, 4).Value = string.Join("\n", ss.SpecificOutcomes.Select(o => "- " + o));
                    ws.Cell(row, 5).Value = string.Join("\n", ss.LearningExperiences.Select(o => "- " + o));
                    ws.Cell(row, 6).Value = string.Join("\n", ss.KeyQuestions);
                    row++;
                }
            }
            var subRows = row - 3;
            for (var i = 0; i < curriculum.Count; i++) ws.Cell(3 + i, 8).Value = curriculum[i].Name;

            ws.Column(1).Width = 26; ws.Column(2).Width = 30; ws.Column(3).Width = 8;
            ws.Column(4).Width = 55; ws.Column(5).Width = 55; ws.Column(6).Width = 35; ws.Column(7).Width = 3; ws.Column(8).Width = 30;
            ws.Range(3, 1, Math.Max(row, 3), 8).Style.Alignment.SetWrapText().Alignment.SetVertical(XLAlignmentVerticalValues.Top);
            ws.SheetView.FreezeRows(2);
            ws.Protect().AllowElement(XLSheetProtectionElements.FormatColumns | XLSheetProtectionElements.FormatRows);
            return (ws.Name, subRows, curriculum.Count);
        }

        private static void SchemeSheet(IXLWorksheet ws, Target t, int weeks, int perWeek, (string Sheet, int SubRows, int StrandRows)? curr)
        {
            // Hidden key row: where the sheet belongs.
            ws.Cell(KeyRow, 1).Value = Marker;
            ws.Cell(KeyRow, 2).Value = t.StaffDetailsId;
            ws.Cell(KeyRow, 3).Value = t.SubjectId;
            ws.Cell(KeyRow, 4).Value = t.SchoolClassId;
            ws.Cell(KeyRow, 5).Value = t.Session.Id;
            ws.Row(KeyRow).Hide();

            ws.Cell(TitleRow, 1).Value = $"{t.Session.AcademicYear?.Name} {t.LearningLevelName} {t.SubjectName} Schemes of Work - {t.Session.SessionName}".ToUpperInvariant();
            ws.Range(TitleRow, 1, TitleRow, Columns.Length).Merge().Style.Font.SetBold().Font.SetFontSize(13);
            ws.Cell(InfoRow, 1).Value = $"Teacher: {t.StaffName}      Class: {t.ClassName}      Term: {t.Session.SessionName} {t.Session.AcademicYear?.Name}"
                + $" ({t.Session.StartDate:d MMM} - {t.Session.EndDate:d MMM yyyy})";
            ws.Range(InfoRow, 1, InfoRow, Columns.Length).Merge();
            ws.Cell(RefRow, 1).Value = "References:";
            ws.Cell(RefRow, 1).Style.Font.SetBold();
            ws.Cell(RefRow, 2).Value = t.Scheme?.References ?? "";
            ws.Range(RefRow, 2, RefRow, Columns.Length).Merge().Style.Fill.SetBackgroundColor(XLColor.LightYellow);
            ws.Cell(NoteRow, 1).Value = "One row per lesson. Pick Strand / Sub-strand from the lists or type your own. "
                + "Blank outcomes, experiences and questions are filled from the curriculum on upload. See the Instructions sheet.";
            ws.Range(NoteRow, 1, NoteRow, Columns.Length).Merge().Style.Font.SetItalic().Font.SetFontColor(XLColor.Gray);

            for (var c = 0; c < Columns.Length; c++)
            {
                ws.Cell(HeaderRow, c + 1).Value = Columns[c].Header;
                ws.Column(c + 1).Width = Columns[c].Width;
            }
            ws.Range(HeaderRow, 1, HeaderRow, Columns.Length).Style
                .Font.SetBold().Fill.SetBackgroundColor(XLColor.LightGray)
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin).Border.SetInsideBorder(XLBorderStyleValues.Thin)
                .Alignment.SetWrapText();

            // What is saved, else an empty week x lesson grid.
            var lessons = t.Scheme != null && t.Scheme.Lessons.Count > 0
                ? t.Scheme.Lessons.OrderBy(l => l.WeekNo).ThenBy(l => l.LessonNo).ThenBy(l => l.Id).ToList()
                : TeachingDocumentService.BlankGrid(weeks, perWeek);
            var row = FirstDataRow;
            foreach (var l in lessons)
            {
                ws.Cell(row, ColWeek).Value = l.WeekNo;
                ws.Cell(row, ColLesson).Value = l.LessonNo;
                ws.Cell(row, ColStrand).Value = l.StrandName ?? "";
                ws.Cell(row, ColSub).Value = l.SubStrandName ?? "";
                ws.Cell(row, ColOutcomes).Value = l.LearningOutcomes ?? "";
                ws.Cell(row, ColExperiences).Value = l.LearningExperiences ?? "";
                ws.Cell(row, ColQuestions).Value = l.KeyInquiryQuestions ?? "";
                ws.Cell(row, ColResources).Value = l.LearningResources ?? "";
                ws.Cell(row, ColAssessment).Value = l.AssessmentMethods ?? "";
                // Shade alternate weeks so the weeks read apart.
                if (l.WeekNo % 2 == 0) ws.Range(row, 1, row, Columns.Length).Style.Fill.SetBackgroundColor(XLColor.FromHtml("#F2F6FC"));
                row++;
            }
            var lastRow = row - 1 + SpareRows;
            var data = ws.Range(FirstDataRow, 1, lastRow, Columns.Length);
            data.Style.Alignment.SetWrapText().Alignment.SetVertical(XLAlignmentVerticalValues.Top)
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin).Border.SetInsideBorder(XLBorderStyleValues.Hair);
            data.Style.Protection.SetLocked(false);
            ws.Cell(RefRow, 2).Style.Protection.SetLocked(false);

            var numbers = ws.Range(FirstDataRow, ColWeek, lastRow, ColLesson).CreateDataValidation();
            numbers.WholeNumber.Between(1, MaxWeeks);
            numbers.ErrorTitle = "Week / lesson";
            numbers.ErrorMessage = $"Enter a number from 1 to {MaxWeeks}.";

            if (curr is var (sheet, subRows, strandRows) && strandRows > 0)
            {
                var q = $"'{sheet.Replace("'", "''")}'";
                var strand = ws.Range(FirstDataRow, ColStrand, lastRow, ColStrand).CreateDataValidation();
                strand.List(ws.Workbook.Worksheet(sheet).Range(3, 8, 2 + strandRows, 8), true);
                Lenient(strand, "Strand", "This strand is not in the curriculum list. Keep it anyway?");
                if (subRows > 0)
                {
                    // The sub-strands of the strand chosen in the same row.
                    var a = $"{q}!$A$3:$A${2 + subRows}";
                    var first = $"C{FirstDataRow}";
                    var sub = ws.Range(FirstDataRow, ColSub, lastRow, ColSub).CreateDataValidation();
                    sub.List($"OFFSET({q}!$B$3,MATCH({first},{a},0)-1,0,COUNTIF({a},{first}),1)", true);
                    Lenient(sub, "Sub-strand", "This sub-strand is not in the curriculum list for the strand. Keep it anyway?");
                }
            }

            ws.SheetView.FreezeRows(HeaderRow);
            ws.PageSetup.PageOrientation = XLPageOrientation.Landscape;
            ws.PageSetup.FitToPages(1, 0);
            ws.PageSetup.SetRowsToRepeatAtTop(HeaderRow, HeaderRow);
            ws.Protect().AllowElement(XLSheetProtectionElements.InsertRows | XLSheetProtectionElements.DeleteRows
                | XLSheetProtectionElements.FormatRows | XLSheetProtectionElements.FormatColumns
                | XLSheetProtectionElements.SelectLockedCells | XLSheetProtectionElements.SelectUnlockedCells);
        }

        // A dropdown that suggests but lets the teacher keep their own text.
        private static void Lenient(IXLDataValidation dv, string title, string message)
        {
            dv.IgnoreBlanks = true;
            dv.ShowErrorMessage = true;
            dv.ErrorStyle = XLErrorStyle.Warning;
            dv.ErrorTitle = title;
            dv.ErrorMessage = message;
        }

        // ================= Upload =================

        private class ParsedRow
        {
            public int Row { get; set; }
            public SchemeOfWorkLessonDto Lesson { get; set; } = new();
        }

        private class ParsedSheet
        {
            public SchemeImportSheetDto Result { get; set; } = new();
            public Target? Target { get; set; }
            public string? References { get; set; }
            public List<ParsedRow> Rows { get; set; } = new();
        }

        public async Task<SchemeImportResultDto> ImportAsync(TeachingUser me, Stream file, bool commit)
        {
            var result = new SchemeImportResultDto { Committed = commit };
            XLWorkbook wb;
            try { wb = new XLWorkbook(file); }
            catch
            {
                result.FileErrors.Add("The file could not be read as an Excel workbook (.xlsx).");
                return result;
            }
            using (wb)
            {
                var seen = new HashSet<(int, int, int, int)>();
                foreach (var ws in wb.Worksheets)
                {
                    if (Text(ws.Cell(KeyRow, 1)) != Marker) continue;
                    var parsed = await ParseAsync(me, ws, seen);
                    result.Sheets.Add(parsed.Result);
                    if (commit && parsed.Result.Action is "Create" or "Update")
                    {
                        await ApplyAsync(parsed);
                        parsed.Result.Saved = true;
                    }
                }
            }
            if (result.Sheets.Count == 0)
                result.FileErrors.Add("No scheme of work sheets were found. Use a workbook downloaded from Teaching Documents > Schemes of Work.");
            return result;
        }

        private async Task<ParsedSheet> ParseAsync(TeachingUser me, IXLWorksheet ws, HashSet<(int, int, int, int)> seen)
        {
            var p = new ParsedSheet { Result = new SchemeImportSheetDto { SheetName = ws.Name } };
            var r = p.Result;
            void Error(int? row, string m) => r.Errors.Add(new ImportIssueDto { Row = row, Message = m });
            void Warn(int? row, string m) => r.Warnings.Add(new ImportIssueDto { Row = row, Message = m });

            // ---- Where the sheet belongs ----
            int[] key = new int[4];
            for (var i = 0; i < 4; i++)
            {
                if (!int.TryParse(Text(ws.Cell(KeyRow, i + 2)), out key[i]))
                {
                    Error(null, "The sheet's hidden first row has been changed. Download a fresh template.");
                    return p;
                }
            }
            var session = await _db.Sessions.AsNoTracking().Include(s => s.AcademicYear).FirstOrDefaultAsync(s => s.Id == key[3]);
            var t = session == null ? null : await TargetAsync(key[0], key[1], key[2], session);
            if (t == null)
            {
                Error(null, "The teacher, subject, class or term on this sheet no longer exists.");
                return p;
            }
            p.Target = t;
            r.StaffName = t.StaffName;
            r.SubjectName = t.SubjectName;
            r.ClassName = t.ClassName;
            if (!seen.Add((key[0], key[1], key[2], key[3])))
            {
                Error(null, $"Another sheet in this workbook is also {t.SubjectName} {t.ClassName} - only the first is used.");
                return p;
            }
            if (!me.CanUploadFor(t.StaffDetailsId, t.DepartmentId))
            {
                Error(null, $"You cannot upload schemes for {t.StaffName}'s {t.SubjectName} - only the teacher, the head of department or an administrator can.");
                return p;
            }
            t.Scheme = await _db.SchemesOfWork.AsNoTracking().Include(s => s.Lessons).ThenInclude(l => l.RecordOfWorkEntry)
                .FirstOrDefaultAsync(s => s.StaffDetailsId == t.StaffDetailsId && s.SubjectId == t.SubjectId
                    && s.SchoolClassId == t.SchoolClassId && s.SessionId == t.Session.Id);
            r.SchemeOfWorkId = t.Scheme?.Id;
            if (t.Scheme == null && !await _db.StaffSubjects.AnyAsync(a =>
                    a.StaffDetailsId == t.StaffDetailsId && a.SubjectId == t.SubjectId && a.SchoolClassId == t.SchoolClassId))
                Error(null, $"{t.StaffName} is no longer allocated {t.SubjectName} for {t.ClassName}.");
            if (t.Scheme != null && await _svc.IsLockedAsync(TeachingDocumentService.EntityScheme, t.Scheme.Id))
                Error(null, "This scheme has been submitted or approved, so it cannot be replaced. Ask the approver to return it first.");
            if (!string.Equals(Text(ws.Cell(HeaderRow, ColWeek)), WeekHeader, StringComparison.OrdinalIgnoreCase)
                || !Text(ws.Cell(HeaderRow, ColOutcomes)).StartsWith("Lesson Learning", StringComparison.OrdinalIgnoreCase))
            {
                Error(null, $"The column headings on row {HeaderRow} have been moved or changed. Download a fresh template.");
                return p;
            }

            p.References = Text(ws.Cell(RefRow, 2));
            if (p.References.Length > 500) Error(RefRow, "References are longer than 500 characters.");

            // ---- The lessons ----
            var curriculum = await _svc.CurriculumAsync(t.SubjectId, t.LearningLevelId, null);
            var strandsByName = curriculum.GroupBy(s => Norm(s.Name)).ToDictionary(g => g.Key, g => g.First());
            var lastRow = ws.LastRowUsed()?.RowNumber() ?? 0;
            var keys = new Dictionary<(int, int), int>();
            var filled = 0;
            for (var row = FirstDataRow; row <= lastRow; row++)
            {
                string Cell(int c) => Text(ws.Cell(row, c));
                var weekText = Cell(ColWeek);
                var lessonText = Cell(ColLesson);
                var dto = new SchemeOfWorkLessonDto
                {
                    StrandName = Cell(ColStrand),
                    SubStrandName = Cell(ColSub),
                    LearningOutcomes = Cell(ColOutcomes),
                    LearningExperiences = Cell(ColExperiences),
                    KeyInquiryQuestions = Cell(ColQuestions),
                    LearningResources = Cell(ColResources),
                    AssessmentMethods = Cell(ColAssessment)
                };
                var hasText = new[] { dto.StrandName, dto.SubStrandName, dto.LearningOutcomes, dto.LearningExperiences,
                    dto.KeyInquiryQuestions, dto.LearningResources, dto.AssessmentMethods }.Any(v => v.Length > 0);
                if (!hasText && weekText.Length == 0 && lessonText.Length == 0) continue;

                if (!int.TryParse(weekText, out var week) || week < 1 || week > MaxWeeks)
                {
                    Error(row, weekText.Length == 0 ? "Week is missing." : $"Week '{weekText}' is not a number from 1 to {MaxWeeks}.");
                    continue;
                }
                if (!int.TryParse(lessonText, out var lesson) || lesson < 1 || lesson > MaxLessons)
                {
                    Error(row, lessonText.Length == 0 ? "Lesson is missing." : $"Lesson '{lessonText}' is not a number from 1 to {MaxLessons}.");
                    continue;
                }
                if (keys.TryGetValue((week, lesson), out var firstRow))
                {
                    Error(row, $"Week {week} lesson {lesson} is already on row {firstRow}.");
                    continue;
                }
                keys[(week, lesson)] = row;
                dto.WeekNo = week;
                dto.LessonNo = lesson;
                if (dto.StrandName!.Length > MaxName) Error(row, $"Strand is longer than {MaxName} characters.");
                if (dto.SubStrandName!.Length > MaxName) Error(row, $"Sub-strand is longer than {MaxName} characters.");

                // ---- Match to the curriculum ----
                CurriculumStrandDto? strand = null;
                CurriculumSubStrandDto? sub = null;
                if (dto.StrandName.Length > 0) strandsByName.TryGetValue(Norm(dto.StrandName), out strand);
                if (dto.SubStrandName.Length > 0)
                {
                    sub = (strand?.SubStrands ?? new()).FirstOrDefault(x => Norm(x.Name) == Norm(dto.SubStrandName));
                    if (sub == null && strand == null)
                    {
                        // Strand left blank or typed differently: find the sub-strand anywhere in the subject.
                        var hits = curriculum.SelectMany(s => s.SubStrands.Select(x => (s, x)))
                            .Where(h => Norm(h.x.Name) == Norm(dto.SubStrandName)).ToList();
                        if (hits.Count == 1 && dto.StrandName.Length == 0) (strand, sub) = hits[0];
                    }
                }
                if (curriculum.Count > 0)
                {
                    if (dto.StrandName.Length > 0 && strand == null)
                        Warn(row, $"Strand '{dto.StrandName}' is not in the curriculum - kept as typed.");
                    else if (dto.SubStrandName.Length > 0 && sub == null)
                        Warn(row, $"Sub-strand '{dto.SubStrandName}' is not under {(strand == null ? "any strand" : $"'{strand.Name}'")} in the curriculum - kept as typed.");
                }
                if (strand != null) { dto.StrandId = strand.Id; dto.StrandName = strand.Name; }
                if (sub != null)
                {
                    dto.SubStrandId = sub.Id;
                    dto.SubStrandName = sub.Name;
                    r.CurriculumMatched++;
                    if (dto.LearningOutcomes!.Length == 0 && sub.SpecificOutcomes.Count > 0)
                    {
                        dto.LearningOutcomes = TeachingDocumentService.Lines(sub.SpecificOutcomes, OutcomesHeading);
                        filled++;
                    }
                    if (dto.LearningExperiences!.Length == 0 && sub.LearningExperiences.Count > 0)
                        dto.LearningExperiences = TeachingDocumentService.Lines(sub.LearningExperiences, ExperiencesHeading);
                    if (dto.KeyInquiryQuestions!.Length == 0 && sub.KeyQuestions.Count > 0)
                        dto.KeyInquiryQuestions = string.Join("\n", sub.KeyQuestions);
                }
                if (hasText && dto.SubStrandName.Length == 0 && dto.LearningOutcomes!.Length == 0)
                    Warn(row, $"Week {week} lesson {lesson} has no sub-strand or outcomes.");

                p.Rows.Add(new ParsedRow { Row = row, Lesson = dto });
            }

            r.Rows = p.Rows.Count;
            r.LessonsFilled = p.Rows.Count(x => HasContent(x.Lesson));
            if (filled > 0) Warn(null, $"Outcomes for {filled} lesson(s) left blank were filled in from the curriculum.");
            // A sheet left as downloaded is passed over, not an error - teachers fill a subject at a time.
            if (r.LessonsFilled == 0 && r.Errors.Count == 0) { r.Action = "Empty"; return p; }

            // Lessons with teaching recorded that the upload would remove.
            if (t.Scheme != null)
            {
                foreach (var l in t.Scheme.Lessons.Where(l => !keys.ContainsKey((l.WeekNo, l.LessonNo))
                             && l.RecordOfWorkEntry != null && l.RecordOfWorkEntry.Status != LessonTaughtStatus.NotRecorded))
                    Warn(null, $"Week {l.WeekNo} lesson {l.LessonNo} has teaching recorded in the record of work but is not on the sheet - that record will be removed.");
            }

            r.Action = r.Errors.Count > 0 ? "Skip" : t.Scheme == null ? "Create" : "Update";
            return p;
        }

        private async Task ApplyAsync(ParsedSheet p)
        {
            var t = p.Target!;
            var scheme = t.Scheme == null ? null
                : await _db.SchemesOfWork.Include(s => s.Lessons).FirstAsync(s => s.Id == t.Scheme.Id);
            if (scheme == null)
            {
                scheme = new SchemeOfWork
                {
                    StaffDetailsId = t.StaffDetailsId,
                    SubjectId = t.SubjectId,
                    SchoolClassId = t.SchoolClassId,
                    SessionId = t.Session.Id,
                    LessonsPerWeek = await _svc.DefaultLessonsPerWeekAsync(t.SubjectId, t.LearningLevelId)
                };
                _db.SchemesOfWork.Add(scheme);
            }
            scheme.References = string.IsNullOrWhiteSpace(p.References) ? null : p.References.Trim();
            scheme.NumberOfWeeks = Math.Clamp(p.Rows.Select(x => x.Lesson.WeekNo).DefaultIfEmpty(1).Max(), 1, MaxWeeks);

            // Update in place by week and lesson, so plans and records stay linked.
            var existing = scheme.Lessons.GroupBy(l => (l.WeekNo, l.LessonNo)).ToDictionary(g => g.Key, g => g.First());
            var kept = new HashSet<SchemeOfWorkLesson>();
            foreach (var x in p.Rows)
            {
                if (!existing.TryGetValue((x.Lesson.WeekNo, x.Lesson.LessonNo), out var row))
                {
                    row = new SchemeOfWorkLesson();
                    scheme.Lessons.Add(row);
                }
                kept.Add(row);
                var d = x.Lesson;
                row.WeekNo = d.WeekNo;
                row.LessonNo = d.LessonNo;
                row.StrandId = d.StrandId;
                row.SubStrandId = d.SubStrandId;
                row.StrandName = Blank(d.StrandName);
                row.SubStrandName = Blank(d.SubStrandName);
                row.LearningOutcomes = Blank(d.LearningOutcomes);
                row.LearningExperiences = Blank(d.LearningExperiences);
                row.KeyInquiryQuestions = Blank(d.KeyInquiryQuestions);
                row.LearningResources = Blank(d.LearningResources);
                row.AssessmentMethods = Blank(d.AssessmentMethods);
            }
            _db.SchemeOfWorkLessons.RemoveRange(scheme.Lessons.Where(l => !kept.Contains(l) && l.Id > 0));
            await _db.SaveChangesAsync();
            p.Result.SchemeOfWorkId = scheme.Id;
        }

        // ================= Helpers =================

        private static string Text(IXLCell cell)
        {
            var v = cell.HasFormula ? cell.CachedValue.ToString() : cell.GetFormattedString();
            return (v ?? "").Replace("\r\n", "\n").Replace('\r', '\n').Trim();
        }

        private static string Norm(string? s) => Regex.Replace((s ?? "").Trim().ToLowerInvariant(), @"\s+", " ");

        private static string? Blank(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();

        private static bool HasContent(SchemeOfWorkLesson l) =>
            !string.IsNullOrWhiteSpace(l.SubStrandName) || !string.IsNullOrWhiteSpace(l.StrandName)
            || !string.IsNullOrWhiteSpace(l.LearningOutcomes) || !string.IsNullOrWhiteSpace(l.LearningExperiences);

        private static bool HasContent(SchemeOfWorkLessonDto l) =>
            !string.IsNullOrWhiteSpace(l.SubStrandName) || !string.IsNullOrWhiteSpace(l.StrandName)
            || !string.IsNullOrWhiteSpace(l.LearningOutcomes) || !string.IsNullOrWhiteSpace(l.LearningExperiences);

        // First words of a subject name that fit, without any "(...)" note -
        // "Creative Arts (Art/Crafts, Music ...)" -> "Creative Arts".
        private static string Cut(string s, int max)
        {
            s = Regex.Replace(s, @"\s*\(.*?\)", "").Trim();
            if (s.Length <= max) return s;
            var cut = s[..max];
            var space = cut.LastIndexOf(' ');
            return (space > 3 ? cut[..space] : cut).Trim().TrimEnd('&', ',').Trim();
        }

        // "Grade 1" -> "G1", to leave room for the subject in a sheet name.
        private static string ShortLevel(string name) => Regex.Replace(name, @"^Grade\s*", "G", RegexOptions.IgnoreCase);

        // "Grade 1E 2026" -> "Grade 1E": the year is in the title already.
        private static string ShortClass(string name) => Regex.Replace(name, @"\s+\d{4}$", "");

        private static string SheetName(string s)
        {
            s = Regex.Replace(s, @"[\[\]\:\*\?\/\\']", " ").Trim();
            s = Regex.Replace(s, @"\s+", " ");
            return s.Length > 26 ? s[..26].Trim() : s;
        }

        private static string Unique(string name, HashSet<string> used)
        {
            var candidate = name;
            for (var i = 2; !used.Add(candidate); i++) candidate = $"{name} ({i})";
            return candidate;
        }
    }
}
