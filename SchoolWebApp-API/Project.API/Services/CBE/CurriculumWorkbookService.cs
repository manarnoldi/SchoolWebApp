using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.DTOs.CBE;
using SchoolWebApp.Core.Entities.CBE.Assessments;
using System.Text.RegularExpressions;

namespace SchoolWebApp.API.Services.CBE
{
    /// <summary>
    /// A subject's curriculum for one grade as an Excel workbook, laid out like
    /// the KICD curriculum design so whoever extracts it can copy table by table:
    /// one row per sub-strand with its outcomes, learning experiences, key
    /// questions, core competencies and PCIs, plus the subject's general learning
    /// outcomes. Downloaded pre-filled with what is saved; uploaded back with
    /// checks, and saved in place - existing strands, sub-strands and outcomes
    /// are matched by name and updated, so schemes and assessments that point at
    /// them keep working.
    /// </summary>
    public class CurriculumWorkbookService
    {
        private const string Marker = "SHULENOVA-CURRICULUM";
        private const string CurriculumSheet = "Curriculum", OutcomesSheet = "Subject Outcomes", ListsSheet = "Lists";
        private const int KeyRow = 1, TitleRow = 2, NoteRow = 3, HeaderRow = 4, FirstRow = 5;
        private const int OutcomesHeaderRow = 3, OutcomesFirstRow = 4, OutcomeSlots = 15;
        private const int SpareRows = 30, MaxName = 255, MaxDescription = 500;

        private static readonly (string Header, double Width)[] Columns =
        {
            ("Strand No.", 8), ("Strand", 22), ("Sub-strand No.", 9), ("Sub-strand", 26), ("Lessons", 8),
            ("Specific Learning Outcomes", 45), ("Suggested Learning Experiences", 55), ("Key Inquiry Question(s)", 30),
            ("Core Competencies", 40), ("Pertinent and Contemporary Issues (PCIs)", 40),
            ("Subject Outcome (letter)", 10), ("Level Learning Outcome", 40)
        };
        private const int CStrandNo = 1, CStrand = 2, CSubNo = 3, CSub = 4, CLessons = 5, COutcomes = 6, CExperiences = 7,
            CQuestions = 8, CCompetencies = 9, CPcis = 10, CSubjectOutcome = 11, CLevelOutcome = 12;

        private readonly ApplicationDbContext _db;

        public CurriculumWorkbookService(ApplicationDbContext db)
        {
            _db = db;
        }

        private class Scope
        {
            public int SubjectId { get; set; }
            public string SubjectName { get; set; } = "";
            public int LearningLevelId { get; set; }
            public string LearningLevelName { get; set; } = "";
            public int EducationLevelId { get; set; }
            public string EducationLevelName { get; set; } = "";
            public int EducationLevelTypeId { get; set; }
        }

        private async Task<Scope?> ScopeAsync(int subjectId, int learningLevelId)
        {
            var subject = await _db.Subjects.AsNoTracking().FirstOrDefaultAsync(s => s.Id == subjectId);
            var level = await _db.LearningLevels.AsNoTracking().Include(l => l.EducationLevel).FirstOrDefaultAsync(l => l.Id == learningLevelId);
            if (subject == null || level?.EducationLevel == null) return null;
            return new Scope
            {
                SubjectId = subject.Id, SubjectName = subject.Name,
                LearningLevelId = level.Id, LearningLevelName = level.Name,
                EducationLevelId = level.EducationLevelId, EducationLevelName = level.EducationLevel.Name,
                EducationLevelTypeId = level.EducationLevel.EducationLevelTypeId
            };
        }

        // ================= Download =================

        public async Task<(byte[] Content, string FileName)?> TemplateAsync(int subjectId, int learningLevelId)
        {
            var scope = await ScopeAsync(subjectId, learningLevelId);
            if (scope == null) return null;

            var subjectOutcomes = await _db.BroadOutcomes.AsNoTracking()
                .Where(o => o.SubjectId == subjectId && o.EducationLevelId == scope.EducationLevelId)
                .OrderBy(o => o.Rank).ThenBy(o => o.Id).ToListAsync();
            var levelOutcomes = await _db.GeneralOutcomes.AsNoTracking()
                .Where(o => o.EducationLevelTypeId == scope.EducationLevelTypeId)
                .OrderBy(o => o.Rank).ThenBy(o => o.Id).ToListAsync();
            var competencies = await _db.Competencies.AsNoTracking().OrderBy(c => c.Rank).ThenBy(c => c.Id).ToListAsync();
            var strands = await _db.Strands.AsNoTracking()
                .Where(s => s.SubjectId == subjectId && s.LearningLevelId == learningLevelId)
                .Include(s => s.SubStrands).ThenInclude(ss => ss.SpecificOutcomes).ThenInclude(o => o.Competencies)
                .Include(s => s.SubStrands).ThenInclude(ss => ss.LearningExperiences)
                .Include(s => s.SubStrands).ThenInclude(ss => ss.Questions)
                .Include(s => s.SubStrands).ThenInclude(ss => ss.PCIs)
                .OrderBy(s => s.Rank).ThenBy(s => s.Code).ThenBy(s => s.Id)
                .ToListAsync();

            using var wb = new XLWorkbook();
            Instructions(wb.AddWorksheet("Instructions"), scope);
            var ws = wb.AddWorksheet(CurriculumSheet);
            var outcomesWs = wb.AddWorksheet(OutcomesSheet);
            var lists = wb.AddWorksheet(ListsSheet);

            // ---- Subject general learning outcomes ----
            outcomesWs.Cell(1, 1).Value = $"Subject general learning outcomes - {scope.SubjectName}, {scope.EducationLevelName}";
            outcomesWs.Cell(1, 1).Style.Font.SetBold().Font.SetFontSize(13);
            outcomesWs.Cell(2, 1).Value = "From the design's 'Subject General Learning Outcomes' page. Shared by every grade of "
                + $"{scope.EducationLevelName}. Each sub-strand on the Curriculum sheet names the letter it serves.";
            outcomesWs.Cell(2, 1).Style.Font.SetItalic().Font.SetFontColor(XLColor.Gray);
            outcomesWs.Cell(OutcomesHeaderRow, 1).Value = "Letter";
            outcomesWs.Cell(OutcomesHeaderRow, 2).Value = "Outcome";
            outcomesWs.Range(OutcomesHeaderRow, 1, OutcomesHeaderRow, 2).Style.Font.SetBold().Fill.SetBackgroundColor(XLColor.LightGray);
            for (var i = 0; i < OutcomeSlots; i++)
            {
                outcomesWs.Cell(OutcomesFirstRow + i, 1).Value = Letter(i);
                if (i < subjectOutcomes.Count) outcomesWs.Cell(OutcomesFirstRow + i, 2).Value = subjectOutcomes[i].Name;
            }
            outcomesWs.Column(1).Width = 8;
            outcomesWs.Column(2).Width = 110;
            var outcomeCells = outcomesWs.Range(OutcomesFirstRow, 2, OutcomesFirstRow + OutcomeSlots - 1, 2);
            outcomeCells.Style.Alignment.SetWrapText().Protection.SetLocked(false).Fill.SetBackgroundColor(XLColor.LightYellow);
            outcomesWs.Protect().AllowElement(XLSheetProtectionElements.FormatRows | XLSheetProtectionElements.FormatColumns);

            // ---- Lists for the dropdowns ----
            lists.Cell(1, 1).Value = "Level learning outcomes";
            lists.Cell(1, 3).Value = "Core competencies";
            for (var i = 0; i < levelOutcomes.Count; i++) lists.Cell(2 + i, 1).Value = levelOutcomes[i].Name;
            for (var i = 0; i < competencies.Count; i++) lists.Cell(2 + i, 3).Value = competencies[i].Name;
            lists.Range(1, 1, 1, 3).Style.Font.SetBold();
            lists.Column(1).Width = 90; lists.Column(2).Width = 3; lists.Column(3).Width = 40;
            lists.Protect();

            // ---- The curriculum: a row per sub-strand ----
            ws.Cell(KeyRow, 1).Value = Marker;
            ws.Cell(KeyRow, 2).Value = scope.SubjectId;
            ws.Cell(KeyRow, 3).Value = scope.LearningLevelId;
            ws.Row(KeyRow).Hide();
            ws.Cell(TitleRow, 1).Value = $"{scope.SubjectName} - {scope.LearningLevelName} curriculum design".ToUpperInvariant();
            ws.Range(TitleRow, 1, TitleRow, Columns.Length).Merge().Style.Font.SetBold().Font.SetFontSize(13);
            ws.Cell(NoteRow, 1).Value = "One row per sub-strand, copied from the design's strand tables. Put each outcome, experience, "
                + "question, competency and PCI on its own line (Alt+Enter), or keep the design's a) / bullets / 1. markers. See Instructions.";
            ws.Range(NoteRow, 1, NoteRow, Columns.Length).Merge().Style.Font.SetItalic().Font.SetFontColor(XLColor.Gray);
            for (var c = 0; c < Columns.Length; c++)
            {
                ws.Cell(HeaderRow, c + 1).Value = Columns[c].Header;
                ws.Column(c + 1).Width = Columns[c].Width;
            }
            ws.Range(HeaderRow, 1, HeaderRow, Columns.Length).Style.Font.SetBold().Fill.SetBackgroundColor(XLColor.LightGray)
                .Alignment.SetWrapText().Alignment.SetVertical(XLAlignmentVerticalValues.Top)
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin).Border.SetInsideBorder(XLBorderStyleValues.Thin);

            var row = FirstRow;
            for (var si = 0; si < strands.Count; si++)
            {
                var s = strands[si];
                foreach (var ss in s.SubStrands.OrderBy(x => x.Rank).ThenBy(x => x.Code).ThenBy(x => x.Id))
                {
                    var outcomes = ss.SpecificOutcomes.OrderBy(o => o.Rank).ThenBy(o => o.Id).ToList();
                    ws.Cell(row, CStrandNo).Value = s.Code ?? $"{si + 1}.0";
                    ws.Cell(row, CStrand).Value = s.Name;
                    ws.Cell(row, CSubNo).Value = ss.Code ?? "";
                    ws.Cell(row, CSub).Value = ss.Name;
                    ws.Cell(row, CLessons).Value = ss.LessonNo;
                    ws.Cell(row, COutcomes).Value = string.Join("\n", outcomes.Select((o, i) => $"{Letter(i)}) {o.Name}"));
                    ws.Cell(row, CExperiences).Value = string.Join("\n", ss.LearningExperiences.OrderBy(x => x.Rank).ThenBy(x => x.Id).Select(x => "- " + x.Name));
                    ws.Cell(row, CQuestions).Value = string.Join("\n", ss.Questions.OrderBy(x => x.Rank).ThenBy(x => x.Id).Select((x, i) => $"{i + 1}. {x.Name}"));
                    ws.Cell(row, CCompetencies).Value = string.Join("\n", outcomes.SelectMany(o => o.Competencies).Select(c => c.Name).Distinct());
                    ws.Cell(row, CPcis).Value = string.Join("\n", ss.PCIs.OrderBy(x => x.Rank).ThenBy(x => x.Id)
                        .Select(p => string.IsNullOrWhiteSpace(p.Description) ? p.Name : $"{p.Name}: {p.Description}"));
                    var broad = outcomes.Select(o => o.BroadOutcomeId).FirstOrDefault();
                    var broadIndex = subjectOutcomes.FindIndex(o => o.Id == broad);
                    if (broadIndex >= 0) ws.Cell(row, CSubjectOutcome).Value = Letter(broadIndex);
                    var general = levelOutcomes.FirstOrDefault(o => o.Id == outcomes.Select(x => x.GeneralOutcomeId).FirstOrDefault());
                    if (general != null) ws.Cell(row, CLevelOutcome).Value = general.Name;
                    row++;
                }
            }
            var lastRow = row - 1 + SpareRows;
            var data = ws.Range(FirstRow, 1, lastRow, Columns.Length);
            data.Style.Alignment.SetWrapText().Alignment.SetVertical(XLAlignmentVerticalValues.Top)
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin).Border.SetInsideBorder(XLBorderStyleValues.Hair)
                .Protection.SetLocked(false);

            var lessons = ws.Range(FirstRow, CLessons, lastRow, CLessons).CreateDataValidation();
            lessons.WholeNumber.Between(0, 200);
            lessons.ErrorTitle = "Lessons";
            lessons.ErrorMessage = "Enter the number of lessons, e.g. 7 for '(7 lessons)'.";
            var letter = ws.Range(FirstRow, CSubjectOutcome, lastRow, CSubjectOutcome).CreateDataValidation();
            letter.List(outcomesWs.Range(OutcomesFirstRow, 1, OutcomesFirstRow + OutcomeSlots - 1, 1), true);
            letter.ErrorTitle = "Subject outcome";
            letter.ErrorMessage = $"Pick the letter of a subject general learning outcome on the '{OutcomesSheet}' sheet.";
            if (levelOutcomes.Count > 0)
            {
                var level = ws.Range(FirstRow, CLevelOutcome, lastRow, CLevelOutcome).CreateDataValidation();
                level.List(lists.Range(2, 1, 1 + levelOutcomes.Count, 1), true);
                level.ErrorTitle = "Level learning outcome";
                level.ErrorMessage = "Pick one of the level learning outcomes set up under CBE Settings > General Outcomes.";
            }

            ws.SheetView.FreezeRows(HeaderRow);
            ws.SheetView.FreezeColumns(CSub);
            ws.PageSetup.PageOrientation = XLPageOrientation.Landscape;
            ws.PageSetup.FitToPages(1, 0);
            ws.Protect().AllowElement(XLSheetProtectionElements.InsertRows | XLSheetProtectionElements.DeleteRows
                | XLSheetProtectionElements.FormatRows | XLSheetProtectionElements.FormatColumns
                | XLSheetProtectionElements.SelectLockedCells | XLSheetProtectionElements.SelectUnlockedCells);
            ws.SetTabActive();

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return (ms.ToArray(), $"Curriculum - {scope.SubjectName} - {scope.LearningLevelName}.xlsx");
        }

        private static void Instructions(IXLWorksheet ws, Scope scope)
        {
            var lines = new[]
            {
                $"Curriculum design - {scope.SubjectName}, {scope.LearningLevelName}",
                "",
                "What goes where",
                $"1. '{OutcomesSheet}': the design's SUBJECT GENERAL LEARNING OUTCOMES, one per row next to its letter (a, b, c ...).",
                $"2. '{CurriculumSheet}': one row per sub-strand from the strand tables (STRAND 1.0 ..., sub-strands 1.1, 1.2 ...):",
                "   - Strand No. and Strand (e.g. 1.0, Creation); Sub-strand No. and Sub-strand (e.g. 1.1, Our God).",
                "     The strand may be left blank on its second and later sub-strands - it carries down.",
                "   - Lessons: the number in '(7 lessons)'.",
                "   - Specific Learning Outcomes, Suggested Learning Experiences, Key Inquiry Question(s): one item per line.",
                "     Pasting straight from the PDF is fine - a), b) / bullets / 1., 2. mark where each item starts, and",
                "     headings such as 'By the end of the sub strand, the learner should be able to:' are dropped.",
                "   - Core Competencies: one per line, as in the design, e.g.",
                "     'Communication and Collaboration: Speaking engagingly: The learner ...' - the name before the first colon",
                $"     must be one of the core competencies (see the '{ListsSheet}' sheet).",
                "   - PCIs: one per line, e.g. 'Citizenship: Social cohesion: The learner displays ...'.",
                "   - Subject Outcome (letter): which subject general learning outcome the sub-strand serves - usually strand 1",
                "     serves a), strand 2 serves b) and so on.",
                "   - Level Learning Outcome: pick from the list (the level learning outcomes set up in ShuleNova).",
                "3. Values, links to other learning areas and assessment rubrics are not captured.",
                "",
                "Uploading",
                "- In ShuleNova: Settings > Dropdowns > CBE Settings > Curriculum Upload (Excel). The workbook is checked first and",
                "  every problem listed by row; nothing is saved until you confirm.",
                "- Strands, sub-strands and outcomes already in ShuleNova are matched by name and updated, not duplicated.",
                "  Uploading again after a correction is safe.",
                "- Do not rename the sheets or move the columns."
            };
            for (var i = 0; i < lines.Length; i++) ws.Cell(i + 1, 1).Value = lines[i];
            ws.Cell(1, 1).Style.Font.SetBold().Font.SetFontSize(14);
            ws.Cell(3, 1).Style.Font.SetBold();
            ws.Cell(21, 1).Style.Font.SetBold();
            ws.Column(1).Width = 120;
        }

        // ================= Upload =================

        private class SubStrandRow
        {
            public int Row { get; set; }
            public string StrandNo { get; set; } = "";
            public string Strand { get; set; } = "";
            public string SubNo { get; set; } = "";
            public string Sub { get; set; } = "";
            public int Lessons { get; set; }
            public List<string> Outcomes { get; set; } = new();
            public List<string> Experiences { get; set; } = new();
            public List<string> Questions { get; set; } = new();
            public List<int> CompetencyIds { get; set; } = new();
            public List<(string Name, string? Description)> Pcis { get; set; } = new();
            public int SubjectOutcomeIndex { get; set; } = -1;
            public int GeneralOutcomeId { get; set; }
        }

        public async Task<CurriculumImportResultDto> ImportAsync(Stream file, bool commit)
        {
            var r = new CurriculumImportResultDto { Committed = commit };
            void Error(string? sheet, int? row, string m) => r.Errors.Add(new CurriculumImportIssueDto { Sheet = sheet, Row = row, Message = m });
            void Warn(string? sheet, int? row, string m) => r.Warnings.Add(new CurriculumImportIssueDto { Sheet = sheet, Row = row, Message = m });

            XLWorkbook wb;
            try { wb = new XLWorkbook(file); }
            catch
            {
                Error(null, null, "The file could not be read as an Excel workbook (.xlsx).");
                return r;
            }
            using (wb)
            {
                var ws = wb.Worksheets.FirstOrDefault(w => Text(w.Cell(KeyRow, 1)) == Marker);
                if (ws == null || !wb.TryGetWorksheet(OutcomesSheet, out var outcomesWs))
                {
                    Error(null, null, "This is not a curriculum workbook from ShuleNova. Download the template from Curriculum Upload.");
                    return r;
                }
                if (!int.TryParse(Text(ws.Cell(KeyRow, 2)), out var subjectId) || !int.TryParse(Text(ws.Cell(KeyRow, 3)), out var levelId))
                {
                    Error(ws.Name, null, "The sheet's hidden first row has been changed. Download a fresh template.");
                    return r;
                }
                var scope = await ScopeAsync(subjectId, levelId);
                if (scope == null)
                {
                    Error(ws.Name, null, "The subject or grade this workbook is for no longer exists.");
                    return r;
                }
                r.SubjectName = scope.SubjectName;
                r.LearningLevelName = scope.LearningLevelName;
                if (!string.Equals(Text(ws.Cell(HeaderRow, CStrand)), "Strand", StringComparison.OrdinalIgnoreCase)
                    || !Text(ws.Cell(HeaderRow, COutcomes)).StartsWith("Specific", StringComparison.OrdinalIgnoreCase))
                {
                    Error(ws.Name, HeaderRow, "The column headings have been moved or changed. Download a fresh template.");
                    return r;
                }

                // ---- Subject general learning outcomes ----
                var subjectOutcomes = new List<string>();
                var outcomeLetters = new Dictionary<string, int>();
                for (var i = 0; i < OutcomeSlots; i++)
                {
                    var rowNo = OutcomesFirstRow + i;
                    var text = Clean(Text(outcomesWs.Cell(rowNo, 2)));
                    if (text.Length == 0) continue;
                    if (text.Length > MaxName) Error(OutcomesSheet, rowNo, $"The outcome is longer than {MaxName} characters.");
                    outcomeLetters[Text(outcomesWs.Cell(rowNo, 1)).Trim().TrimEnd(')').ToLowerInvariant()] = subjectOutcomes.Count;
                    subjectOutcomes.Add(text);
                }
                if (subjectOutcomes.Count == 0) Error(OutcomesSheet, null, "Enter the subject's general learning outcomes.");
                r.SubjectOutcomes = subjectOutcomes.Count;

                // ---- The sub-strands ----
                var generalOutcomes = await _db.GeneralOutcomes.AsNoTracking()
                    .Where(o => o.EducationLevelTypeId == scope.EducationLevelTypeId).ToListAsync();
                var competencies = await _db.Competencies.AsNoTracking().ToListAsync();
                var rows = new List<SubStrandRow>();
                string strandNo = "", strand = "";
                var lastRow = ws.LastRowUsed()?.RowNumber() ?? 0;
                for (var rowNo = FirstRow; rowNo <= lastRow; rowNo++)
                {
                    string Cell(int c) => Text(ws.Cell(rowNo, c));
                    if (Enumerable.Range(1, Columns.Length).All(c => Cell(c).Length == 0)) continue;
                    var x = new SubStrandRow { Row = rowNo };
                    // A strand carries down to its later sub-strands.
                    if (Cell(CStrand).Length > 0) { strand = Clean(Cell(CStrand)); strandNo = Cell(CStrandNo); }
                    else if (Cell(CStrandNo).Length > 0 && Cell(CStrandNo) != strandNo) strand = "";
                    x.Strand = strand;
                    x.StrandNo = Cell(CStrandNo).Length > 0 ? Cell(CStrandNo) : strandNo;
                    x.SubNo = Cell(CSubNo);
                    x.Sub = Clean(Cell(CSub));
                    if (x.Strand.Length == 0) Error(ws.Name, rowNo, "Strand is missing.");
                    if (x.Sub.Length == 0) Error(ws.Name, rowNo, "Sub-strand is missing.");
                    if (x.Strand.Length > MaxName) Error(ws.Name, rowNo, $"Strand is longer than {MaxName} characters.");
                    if (x.Sub.Length > MaxName) Error(ws.Name, rowNo, $"Sub-strand is longer than {MaxName} characters.");

                    var lessonsText = Cell(CLessons);
                    if (lessonsText.Length == 0) Warn(ws.Name, rowNo, $"No lessons given for '{x.Sub}'.");
                    else if (!int.TryParse(Regex.Match(lessonsText, @"\d+").Value, out var lessonCount)) Error(ws.Name, rowNo, $"Lessons '{lessonsText}' is not a number.");
                    else x.Lessons = lessonCount;

                    x.Outcomes = Items(Cell(COutcomes));
                    x.Experiences = Items(Cell(CExperiences));
                    x.Questions = Items(Cell(CQuestions));
                    if (x.Outcomes.Count == 0) Error(ws.Name, rowNo, $"'{x.Sub}' has no specific learning outcomes.");
                    if (x.Experiences.Count == 0) Warn(ws.Name, rowNo, $"'{x.Sub}' has no learning experiences.");
                    if (x.Questions.Count == 0) Warn(ws.Name, rowNo, $"'{x.Sub}' has no key inquiry questions.");
                    foreach (var (list, what) in new[] { (x.Outcomes, "A specific learning outcome"), (x.Experiences, "A learning experience"), (x.Questions, "A key inquiry question") })
                        foreach (var item in list.Where(i => i.Length > MaxName))
                            Error(ws.Name, rowNo, $"{what} is longer than {MaxName} characters - split it: \"{Cut(item, 60)}...\"");

                    foreach (var line in Items(Cell(CCompetencies)))
                    {
                        var name = line.Split(':')[0];
                        var hit = competencies.FirstOrDefault(c => Key(c.Name) == Key(name));
                        if (hit == null) Warn(ws.Name, rowNo, $"Core competency '{Cut(name, 60)}' is not one of the core competencies set up - not linked.");
                        else if (!x.CompetencyIds.Contains(hit.Id)) x.CompetencyIds.Add(hit.Id);
                    }
                    foreach (var line in Items(Cell(CPcis)))
                    {
                        var at = line.IndexOf("The learner", StringComparison.OrdinalIgnoreCase);
                        var name = (at > 0 ? line[..at] : line).Trim().TrimEnd(':').Trim();
                        var description = at > 0 ? line[at..].Trim() : null;
                        if (name.Length > MaxName || description?.Length > MaxDescription)
                            Warn(ws.Name, rowNo, $"PCI '{Cut(name, 40)}' is too long and was shortened.");
                        x.Pcis.Add((Cut(name, MaxName), description == null ? null : Cut(description, MaxDescription)));
                    }

                    var letter = Cell(CSubjectOutcome).Trim().TrimEnd(')').ToLowerInvariant();
                    if (letter.Length == 0) Error(ws.Name, rowNo, "Pick the subject outcome (letter) this sub-strand serves.");
                    else if (!outcomeLetters.TryGetValue(letter, out var index)) Error(ws.Name, rowNo, $"Subject outcome '{letter}' is not on the '{OutcomesSheet}' sheet.");
                    else x.SubjectOutcomeIndex = index;

                    var levelText = Cell(CLevelOutcome);
                    var general = generalOutcomes.FirstOrDefault(o => Key(o.Name) == Key(levelText));
                    if (levelText.Length == 0) Error(ws.Name, rowNo, "Pick the level learning outcome this sub-strand serves.");
                    else if (general == null) Error(ws.Name, rowNo, $"Level learning outcome '{Cut(levelText, 60)}' is not one of those set up for {scope.EducationLevelName}.");
                    else x.GeneralOutcomeId = general.Id;

                    if (rows.Any(p => Key(p.Strand) == Key(x.Strand) && Key(p.Sub) == Key(x.Sub)))
                        Error(ws.Name, rowNo, $"'{x.Sub}' appears twice under '{x.Strand}'.");
                    rows.Add(x);
                }
                if (rows.Count == 0) Error(ws.Name, null, "No sub-strands are filled in.");

                r.Strands = rows.Select(x => Key(x.Strand)).Distinct().Count();
                r.SubStrands = rows.Count;
                r.Lessons = rows.Sum(x => x.Lessons);
                r.SpecificOutcomes = rows.Sum(x => x.Outcomes.Count);
                r.LearningExperiences = rows.Sum(x => x.Experiences.Count);
                r.KeyQuestions = rows.Sum(x => x.Questions.Count);
                r.Pcis = rows.Sum(x => x.Pcis.Count);

                // What is in ShuleNova but not in the workbook stays as it is.
                var keep = rows.Select(x => (Key(x.Strand), Key(x.Sub))).ToHashSet();
                var existing = await _db.SubStrands.AsNoTracking().Include(ss => ss.Strand)
                    .Where(ss => ss.Strand!.SubjectId == subjectId && ss.Strand.LearningLevelId == levelId).ToListAsync();
                foreach (var ss in existing.Where(ss => !keep.Contains((Key(ss.Strand!.Name), Key(ss.Name)))))
                    Warn(null, null, $"'{ss.Strand!.Name} / {ss.Name}' is in ShuleNova but not in this workbook - it is left as it is.");

                if (commit && r.Errors.Count == 0)
                {
                    await SaveAsync(scope, subjectOutcomes, rows, r);
                    r.Saved = true;
                }
            }
            return r;
        }

        private async Task SaveAsync(Scope scope, List<string> subjectOutcomeTexts, List<SubStrandRow> rows, CurriculumImportResultDto r)
        {
            void Count(string what, int n = 1) => r.Changes[what] = r.Changes.GetValueOrDefault(what) + n;
            var curriculumId = await _db.Curricula.OrderBy(c => c.Id).Select(c => (int?)c.Id).FirstOrDefaultAsync();
            await using var tx = await _db.Database.BeginTransactionAsync();

            // Subject outcomes: shared by the education level's grades, matched by text.
            var broad = await _db.BroadOutcomes.Where(o => o.SubjectId == scope.SubjectId && o.EducationLevelId == scope.EducationLevelId).ToListAsync();
            var broadIds = new List<int>();
            for (var i = 0; i < subjectOutcomeTexts.Count; i++)
            {
                var o = broad.FirstOrDefault(b => Key(b.Name) == Key(subjectOutcomeTexts[i]));
                if (o == null)
                {
                    o = new SubjectOutcome { Name = subjectOutcomeTexts[i], SubjectId = scope.SubjectId, EducationLevelId = scope.EducationLevelId };
                    _db.BroadOutcomes.Add(o);
                    Count("Subject outcomes added");
                }
                o.Rank = i + 1;
                broad.Remove(o);
                await _db.SaveChangesAsync();
                broadIds.Add(o.Id);
            }

            var strands = await _db.Strands.Where(s => s.SubjectId == scope.SubjectId && s.LearningLevelId == scope.LearningLevelId)
                .Include(s => s.SubStrands).ThenInclude(ss => ss.SpecificOutcomes).ThenInclude(o => o.Competencies)
                .Include(s => s.SubStrands).ThenInclude(ss => ss.LearningExperiences)
                .Include(s => s.SubStrands).ThenInclude(ss => ss.Questions)
                .Include(s => s.SubStrands).ThenInclude(ss => ss.PCIs)
                .ToListAsync();
            var competencies = await _db.Competencies.ToListAsync();
            var assessed = await _db.StudentAssessments.Where(a => a.SpecificOutcomeId != null)
                .Select(a => a.SpecificOutcomeId!.Value).Distinct().ToListAsync();

            var strandRank = 0;
            foreach (var group in rows.GroupBy(x => Key(x.Strand)))
            {
                strandRank++;
                var first = group.First();
                var strand = strands.FirstOrDefault(s => Key(s.Name) == group.Key);
                if (strand == null)
                {
                    strand = new Strand
                    {
                        Name = first.Strand, SubjectId = scope.SubjectId, LearningLevelId = scope.LearningLevelId,
                        CurriculumId = curriculumId, Version = 1
                    };
                    _db.Strands.Add(strand);
                    strands.Add(strand);
                    Count("Strands added");
                }
                else Count("Strands updated");
                strand.Name = first.Strand;
                strand.Code = Blank(first.StrandNo) ?? strand.Code;
                strand.Rank = strandRank;

                var subRank = 0;
                foreach (var x in group)
                {
                    subRank++;
                    var sub = strand.SubStrands.FirstOrDefault(ss => Key(ss.Name) == Key(x.Sub));
                    if (sub == null)
                    {
                        sub = new SubStrand
                        {
                            Name = x.Sub, SubjectId = scope.SubjectId, LearningLevelId = scope.LearningLevelId,
                            CurriculumId = curriculumId, Version = 1
                        };
                        strand.SubStrands.Add(sub);
                        Count("Sub-strands added");
                    }
                    else Count("Sub-strands updated");
                    sub.Name = x.Sub;
                    sub.Code = Blank(x.SubNo) ?? sub.Code;
                    sub.LessonNo = x.Lessons;
                    sub.Rank = subRank;

                    // Specific outcomes: matched by text so assessments recorded against them stay put.
                    var linked = competencies.Where(c => x.CompetencyIds.Contains(c.Id)).ToList();
                    var keptOutcomes = new HashSet<SpecificOutcome>();
                    for (var i = 0; i < x.Outcomes.Count; i++)
                    {
                        var o = sub.SpecificOutcomes.FirstOrDefault(so => !keptOutcomes.Contains(so) && Key(so.Name) == Key(x.Outcomes[i]));
                        if (o == null)
                        {
                            o = new SpecificOutcome { Name = x.Outcomes[i] };
                            sub.SpecificOutcomes.Add(o);
                            Count("Specific outcomes added");
                        }
                        keptOutcomes.Add(o);
                        o.Name = x.Outcomes[i];
                        o.Rank = i + 1;
                        o.BroadOutcomeId = broadIds[x.SubjectOutcomeIndex];
                        o.GeneralOutcomeId = x.GeneralOutcomeId;
                        o.Competencies.Clear();
                        o.Competencies.AddRange(linked);
                    }
                    foreach (var o in sub.SpecificOutcomes.Where(o => !keptOutcomes.Contains(o) && o.Id > 0).ToList())
                    {
                        if (assessed.Contains(o.Id))
                        {
                            r.Warnings.Add(new CurriculumImportIssueDto { Sheet = CurriculumSheet, Row = x.Row,
                                Message = $"'{Cut(o.Name, 60)}' is no longer in the workbook but learners have been assessed on it - it was kept." });
                            continue;
                        }
                        sub.SpecificOutcomes.Remove(o);
                        _db.SpecificOutcomes.Remove(o);
                        Count("Specific outcomes removed");
                    }

                    // Experiences, questions and PCIs nothing else points at: replaced.
                    _db.Set<LearningExperience>().RemoveRange(sub.LearningExperiences);
                    _db.Set<KeyQuestion>().RemoveRange(sub.Questions);
                    _db.Set<PCI>().RemoveRange(sub.PCIs);
                    sub.LearningExperiences = x.Experiences.Select((e, i) => new LearningExperience { Name = e, Rank = i + 1 }).ToList();
                    sub.Questions = x.Questions.Select((q, i) => new KeyQuestion { Name = q, Rank = i + 1 }).ToList();
                    sub.PCIs = x.Pcis.Select((p, i) => new PCI { Name = p.Name, Description = p.Description, Rank = i + 1 }).ToList();
                }
            }
            await _db.SaveChangesAsync();
            await tx.CommitAsync();
        }

        // ================= Helpers =================

        private static string Letter(int i) => ((char)('a' + i)).ToString();

        private static string Text(IXLCell cell)
        {
            var v = cell.HasFormula ? cell.CachedValue.ToString() : cell.GetFormattedString();
            return (v ?? "").Replace("\r\n", "\n").Replace('\r', '\n').Trim();
        }

        // Comparison key: case, spacing and punctuation ignored.
        private static string Key(string? s) => Regex.Replace((s ?? "").ToLowerInvariant(), @"[^a-z0-9]", "");

        private static string Clean(string s) => Regex.Replace(s.Replace('\n', ' '), @"\s+", " ").Trim();

        private static string? Blank(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();

        private static string Cut(string s, int max) => s.Length <= max ? s : s[..max].TrimEnd();

        // Where an item starts: a), (a), 1., 1), i), and the bullets PDFs paste as.
        private static readonly Regex ItemStart = new(@"^\s*(?:\(?[a-z]\)|\(?[ivx]{1,4}\)|\d{1,2}[.)]|[•●▪‣⁃◦\-\*])\s*",
            RegexOptions.IgnoreCase);
        // Sub-bullets join the item above, e.g. "listen to the Bible verses: 1st John 3:1; Isaiah 64:7".
        private static readonly Regex SubItemStart = new(@"^\s*[▪◦o]\s+");

        /// <summary>
        /// Splits a cell into items. With markers, a line that starts with one
        /// begins an item and other lines continue it (text pasted from a PDF
        /// wraps mid-sentence); without markers, each line is an item. Headings
        /// such as "The learner is guided to:" are dropped.
        /// </summary>
        public static List<string> Items(string cell)
        {
            var lines = cell.Split('\n').Select(l => l.Trim()).Where(l => l.Length > 0)
                .Where(l => !(l.EndsWith(":") && Regex.IsMatch(l, @"\b(learner|able to|guided to)\b", RegexOptions.IgnoreCase)))
                .ToList();
            var marked = lines.Any(l => ItemStart.IsMatch(l));
            // With markers, text before the first one is a heading that wrapped
            // ("By the end of the sub / strand, the learner / should be able to:").
            if (marked) lines = lines.SkipWhile(l => !ItemStart.IsMatch(l)).ToList();
            var items = new List<string>();
            foreach (var line in lines)
            {
                if (SubItemStart.IsMatch(line) && items.Count > 0)
                {
                    var sub = SubItemStart.Replace(line, "").Trim();
                    items[^1] = items[^1].TrimEnd(',', ';', ' ') + (items[^1].EndsWith(':') ? " " : "; ") + sub;
                }
                else if (!marked || ItemStart.IsMatch(line) || items.Count == 0)
                    items.Add(ItemStart.Replace(line, "").Trim());
                else
                    items[^1] = items[^1] + " " + line;
            }
            return items.Select(i => Regex.Replace(i, @"\s+", " ").Trim().TrimEnd(',', ';', '.').Trim())
                .Where(i => i.Length > 0).ToList();
        }
    }
}
