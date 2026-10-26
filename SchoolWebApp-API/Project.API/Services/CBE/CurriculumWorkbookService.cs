using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.DTOs.CBE;
using SchoolWebApp.Core.Entities.CBE.Assessments;
using System.Text.RegularExpressions;

namespace SchoolWebApp.API.Services.CBE
{
    /// <summary>
    /// A subject's KICD curriculum design for one grade as an Excel workbook, so
    /// all of it can be captured, corrected and reused:
    /// - Curriculum: a row per sub-strand as in the design's strand tables -
    ///   outcomes, learning experiences, key questions, core competencies, values,
    ///   PCIs, links to other learning areas, lessons, and (upper grades) its
    ///   suggested resources, assessment methods and non-formal activities.
    /// - Rubrics: the assessment rubrics, for a strand or a sub-strand.
    /// - Subject Outcomes: the subject general learning outcomes.
    /// - Design: title, source, essence statement, lessons per week.
    /// - Appendix: the suggested resources / assessment methods / tools /
    ///   non-formal programmes lists.
    /// - Other Sections: anything else, word for word (national goals, level
    ///   learning outcomes, CSL...).
    /// Downloaded pre-filled with what is saved; uploaded back with checks and
    /// saved in place - strands, sub-strands and outcomes are matched by name, so
    /// schemes and assessments that point at them keep working.
    /// </summary>
    public class CurriculumWorkbookService
    {
        private const string Marker = "SHULENOVA-CURRICULUM";
        private const string CurriculumSheet = "Curriculum", OutcomesSheet = "Subject Outcomes", RubricsSheet = "Rubrics",
            DesignSheet = "Design", AppendixSheet = "Appendix", SectionsSheet = "Other Sections", ListsSheet = "Lists";
        private const int KeyRow = 1, TitleRow = 2, NoteRow = 3, HeaderRow = 4, FirstRow = 5;
        private const int OutcomesHeaderRow = 3, OutcomesFirstRow = 4, OutcomeSlots = 15;
        // Rubrics, Appendix and Other Sections: title, note, headers, then rows.
        private const int ListHeaderRow = 3, ListFirstRow = 4;
        private const int SpareRows = 30, MaxName = 255, MaxDescription = 500, MaxItem = 1000;

        private static readonly (string Header, double Width)[] Columns =
        {
            ("Strand No.", 8), ("Strand", 22), ("Sub-strand No.", 9), ("Sub-strand", 26), ("Lessons", 8),
            ("Specific Learning Outcomes", 45), ("Suggested Learning Experiences", 55), ("Key Inquiry Question(s)", 30),
            ("Core Competencies", 45), ("Values", 40), ("Pertinent and Contemporary Issues (PCIs)", 40),
            ("Links to Other Learning Areas", 40), ("Subject Outcome (letter)", 10), ("Level Learning Outcome", 40),
            ("Suggested Learning Resources", 35), ("Suggested Assessment Methods", 30), ("Suggested Non-Formal Activities", 35)
        };
        private const int CStrandNo = 1, CStrand = 2, CSubNo = 3, CSub = 4, CLessons = 5, COutcomes = 6, CExperiences = 7,
            CQuestions = 8, CCompetencies = 9, CValues = 10, CPcis = 11, CLinks = 12, CSubjectOutcome = 13, CLevelOutcome = 14,
            CResources = 15, CAssessment = 16, CNonFormal = 17;

        private static readonly (string Header, double Width)[] RubricColumns =
        {
            ("Strand No.", 9), ("Sub-strand No. (blank = whole strand)", 13), ("Indicator", 30),
            ("Exceeds Expectations", 40), ("Meets Expectations", 40), ("Approaches Expectations", 40), ("Below Expectations", 40)
        };

        private static readonly (string Category, string Label)[] AppendixCategories =
        {
            (CurriculumDesignItem.Resource, "Suggested Resources"),
            (CurriculumDesignItem.AssessmentMethod, "Suggested Assessment Methods"),
            (CurriculumDesignItem.AssessmentTool, "Suggested Assessment Tools"),
            (CurriculumDesignItem.NonFormalProgramme, "Non-formal Programmes")
        };

        private static readonly string[] DesignFields = { "Title", "Source (publisher, edition)", "Lessons per week", "Essence statement" };

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

        private IQueryable<Strand> StrandsWithDetail(int subjectId, int learningLevelId) => _db.Strands
            .Where(s => s.SubjectId == subjectId && s.LearningLevelId == learningLevelId)
            .Include(s => s.Rubrics)
            .Include(s => s.SubStrands).ThenInclude(ss => ss.SpecificOutcomes).ThenInclude(o => o.Competencies)
            .Include(s => s.SubStrands).ThenInclude(ss => ss.LearningExperiences)
            .Include(s => s.SubStrands).ThenInclude(ss => ss.Questions)
            .Include(s => s.SubStrands).ThenInclude(ss => ss.PCIs)
            .Include(s => s.SubStrands).ThenInclude(ss => ss.CoreCompetencies)
            .Include(s => s.SubStrands).ThenInclude(ss => ss.Values)
            .Include(s => s.SubStrands).ThenInclude(ss => ss.LearningAreaLinks)
            .AsSplitQuery();

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
            var values = await _db.Values.AsNoTracking().OrderBy(v => v.Rank).ThenBy(v => v.Id).ToListAsync();
            var strands = await StrandsWithDetail(subjectId, learningLevelId).AsNoTracking()
                .OrderBy(s => s.Rank).ThenBy(s => s.Code).ThenBy(s => s.Id).ToListAsync();
            var design = await _db.CurriculumDesigns.AsNoTracking().Include(d => d.Items).Include(d => d.Sections)
                .FirstOrDefaultAsync(d => d.SubjectId == subjectId && d.LearningLevelId == learningLevelId);

            using var wb = new XLWorkbook();
            Instructions(wb.AddWorksheet("Instructions"), scope);
            var ws = wb.AddWorksheet(CurriculumSheet);
            var rubricsWs = wb.AddWorksheet(RubricsSheet);
            var outcomesWs = wb.AddWorksheet(OutcomesSheet);
            var designWs = wb.AddWorksheet(DesignSheet);
            var appendixWs = wb.AddWorksheet(AppendixSheet);
            var sectionsWs = wb.AddWorksheet(SectionsSheet);
            var lists = wb.AddWorksheet(ListsSheet);

            // ---- Lists for the dropdowns ----
            string[] listTitles = { "Level learning outcomes", "Core competencies", "Values", "Appendix categories" };
            var listColumns = new[]
            {
                levelOutcomes.Select(o => o.Name).ToList(), competencies.Select(c => c.Name).ToList(),
                values.Select(v => v.Name).ToList(), AppendixCategories.Select(a => a.Label).ToList()
            };
            for (var c = 0; c < listColumns.Length; c++)
            {
                lists.Cell(1, 1 + c * 2).Value = listTitles[c];
                lists.Cell(1, 1 + c * 2).Style.Font.SetBold();
                for (var i = 0; i < listColumns[c].Count; i++) lists.Cell(2 + i, 1 + c * 2).Value = listColumns[c][i];
                lists.Column(1 + c * 2).Width = c == 0 ? 90 : 35;
                lists.Column(2 + c * 2).Width = 3;
            }
            lists.Protect();

            // ---- Subject general learning outcomes ----
            outcomesWs.Cell(1, 1).Value = $"Subject general learning outcomes - {scope.SubjectName}, {scope.EducationLevelName}";
            outcomesWs.Cell(1, 1).Style.Font.SetBold().Font.SetFontSize(13);
            Note(outcomesWs, 2, 2, "From the design's 'Subject General Learning Outcomes' page. Shared by every grade of "
                + $"{scope.EducationLevelName}. Each sub-strand on the Curriculum sheet names the letter it serves.");
            Headers(outcomesWs, OutcomesHeaderRow, new[] { ("Letter", 8.0), ("Outcome", 110.0) });
            for (var i = 0; i < OutcomeSlots; i++)
            {
                outcomesWs.Cell(OutcomesFirstRow + i, 1).Value = Letter(i);
                if (i < subjectOutcomes.Count) outcomesWs.Cell(OutcomesFirstRow + i, 2).Value = subjectOutcomes[i].Name;
            }
            Open(outcomesWs.Range(OutcomesFirstRow, 2, OutcomesFirstRow + OutcomeSlots - 1, 2));
            outcomesWs.Protect().AllowElement(XLSheetProtectionElements.FormatRows | XLSheetProtectionElements.FormatColumns);

            // ---- Design ----
            designWs.Cell(1, 1).Value = $"Curriculum design - {scope.SubjectName}, {scope.LearningLevelName}";
            designWs.Cell(1, 1).Style.Font.SetBold().Font.SetFontSize(13);
            Note(designWs, 2, 2, "The design as a whole. Lessons per week comes from the design's time allocation, where it gives one, "
                + "and becomes the subject's lesson allocation for the grade.");
            string?[] designValues = { design?.Title, design?.Source, design?.LessonsPerWeek?.ToString(), design?.EssenceStatement };
            for (var i = 0; i < DesignFields.Length; i++)
            {
                designWs.Cell(3 + i, 1).Value = DesignFields[i];
                designWs.Cell(3 + i, 1).Style.Font.SetBold().Alignment.SetVertical(XLAlignmentVerticalValues.Top);
                designWs.Cell(3 + i, 2).Value = designValues[i] ?? "";
            }
            designWs.Column(1).Width = 28;
            designWs.Column(2).Width = 120;
            designWs.Row(3 + DesignFields.Length - 1).Height = 150;
            Open(designWs.Range(3, 2, 2 + DesignFields.Length, 2));
            designWs.Protect().AllowElement(XLSheetProtectionElements.FormatRows | XLSheetProtectionElements.FormatColumns);

            // ---- Appendix lists ----
            appendixWs.Cell(1, 1).Value = "Appendix: suggested resources, assessment methods and tools, non-formal programmes";
            appendixWs.Cell(1, 1).Style.Font.SetBold().Font.SetFontSize(13);
            Note(appendixWs, 2, 2, "One item per row, with its list. (Upper-grade designs that give these per sub-strand: use the last "
                + "three columns of the Curriculum sheet instead.)");
            Headers(appendixWs, ListHeaderRow, new[] { ("List", 32.0), ("Item", 100.0) });
            var appendixRow = ListFirstRow;
            foreach (var item in (design?.Items ?? new()).OrderBy(i => Array.FindIndex(AppendixCategories, a => a.Category == i.Category)).ThenBy(i => i.Rank))
            {
                appendixWs.Cell(appendixRow, 1).Value = AppendixCategories.FirstOrDefault(a => a.Category == item.Category).Label ?? item.Category;
                appendixWs.Cell(appendixRow, 2).Value = item.Name;
                appendixRow++;
            }
            var appendixLast = Math.Max(appendixRow, ListFirstRow) + 60;
            Open(appendixWs.Range(ListFirstRow, 1, appendixLast, 2));
            var category = appendixWs.Range(ListFirstRow, 1, appendixLast, 1).CreateDataValidation();
            category.List(lists.Range(2, 7, 1 + AppendixCategories.Length, 7), true);
            appendixWs.Protect().AllowElement(XLSheetProtectionElements.InsertRows | XLSheetProtectionElements.DeleteRows
                | XLSheetProtectionElements.FormatRows | XLSheetProtectionElements.FormatColumns);

            // ---- Other sections ----
            sectionsWs.Cell(1, 1).Value = "Other sections of the design, word for word";
            sectionsWs.Cell(1, 1).Style.Font.SetBold().Font.SetFontSize(13);
            Note(sectionsWs, 2, 2, "Anything not captured elsewhere - national goals of education, level learning outcomes, "
                + "introduction, community service learning appendix ... One section per row: its heading, then its text.");
            Headers(sectionsWs, ListHeaderRow, new[] { ("Section", 40.0), ("Content", 120.0) });
            var sectionRow = ListFirstRow;
            foreach (var s in (design?.Sections ?? new()).OrderBy(s => s.Rank))
            {
                sectionsWs.Cell(sectionRow, 1).Value = s.Title;
                sectionsWs.Cell(sectionRow, 2).Value = s.Content ?? "";
                sectionRow++;
            }
            Open(sectionsWs.Range(ListFirstRow, 1, Math.Max(sectionRow, ListFirstRow) + 20, 2));
            sectionsWs.Protect().AllowElement(XLSheetProtectionElements.InsertRows | XLSheetProtectionElements.DeleteRows
                | XLSheetProtectionElements.FormatRows | XLSheetProtectionElements.FormatColumns);

            // ---- The curriculum: a row per sub-strand ----
            ws.Cell(KeyRow, 1).Value = Marker;
            ws.Cell(KeyRow, 2).Value = scope.SubjectId;
            ws.Cell(KeyRow, 3).Value = scope.LearningLevelId;
            ws.Row(KeyRow).Hide();
            ws.Cell(TitleRow, 1).Value = $"{scope.SubjectName} - {scope.LearningLevelName} curriculum design".ToUpperInvariant();
            ws.Range(TitleRow, 1, TitleRow, Columns.Length).Merge().Style.Font.SetBold().Font.SetFontSize(13);
            Note(ws, NoteRow, Columns.Length, "One row per sub-strand, copied from the design's strand tables. Put each outcome, experience, "
                + "question, competency, value, PCI and link on its own line (Alt+Enter), or keep the design's a) / bullets / 1. markers. See Instructions.");
            Headers(ws, HeaderRow, Columns);

            var row = FirstRow;
            var rubricRow = ListFirstRow;
            for (var si = 0; si < strands.Count; si++)
            {
                var s = strands[si];
                var subs = s.SubStrands.OrderBy(x => x.Rank).ThenBy(x => x.Code).ThenBy(x => x.Id).ToList();
                foreach (var ss in subs)
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
                    ws.Cell(row, CCompetencies).Value = ss.CoreCompetencies.Count > 0
                        ? Lines(ss.CoreCompetencies.OrderBy(x => x.Rank).Select(x => Statement(x.Name, x.Description)))
                        : Lines(outcomes.SelectMany(o => o.Competencies).Select(c => c.Name).Distinct());
                    ws.Cell(row, CValues).Value = Lines(ss.Values.OrderBy(x => x.Rank).Select(x => Statement(x.Name, x.Description)));
                    ws.Cell(row, CPcis).Value = Lines(ss.PCIs.OrderBy(x => x.Rank).ThenBy(x => x.Id).Select(x => Statement(x.Name, x.Description)));
                    ws.Cell(row, CLinks).Value = Lines(ss.LearningAreaLinks.OrderBy(x => x.Rank).Select(x => Statement(x.LearningArea, x.Description)));
                    var broadIndex = subjectOutcomes.FindIndex(o => o.Id == outcomes.Select(x => x.BroadOutcomeId).FirstOrDefault());
                    if (broadIndex >= 0) ws.Cell(row, CSubjectOutcome).Value = Letter(broadIndex);
                    var general = levelOutcomes.FirstOrDefault(o => o.Id == outcomes.Select(x => x.GeneralOutcomeId).FirstOrDefault());
                    if (general != null) ws.Cell(row, CLevelOutcome).Value = general.Name;
                    ws.Cell(row, CResources).Value = ss.SuggestedResources ?? "";
                    ws.Cell(row, CAssessment).Value = ss.SuggestedAssessmentMethods ?? "";
                    ws.Cell(row, CNonFormal).Value = ss.SuggestedNonFormalActivities ?? "";
                    row++;
                }
                foreach (var rb in s.Rubrics.OrderBy(x => x.SubStrandId.HasValue).ThenBy(x => x.Rank))
                {
                    rubricsWs.Cell(rubricRow, 1).Value = s.Code ?? "";
                    rubricsWs.Cell(rubricRow, 2).Value = rb.SubStrandId == null ? "" : subs.FirstOrDefault(x => x.Id == rb.SubStrandId)?.Code ?? "";
                    rubricsWs.Cell(rubricRow, 3).Value = rb.Indicator;
                    rubricsWs.Cell(rubricRow, 4).Value = rb.ExceedsExpectations ?? "";
                    rubricsWs.Cell(rubricRow, 5).Value = rb.MeetsExpectations ?? "";
                    rubricsWs.Cell(rubricRow, 6).Value = rb.ApproachesExpectations ?? "";
                    rubricsWs.Cell(rubricRow, 7).Value = rb.BelowExpectations ?? "";
                    rubricRow++;
                }
            }
            var lastRow = row - 1 + SpareRows;
            Open(ws.Range(FirstRow, 1, lastRow, Columns.Length));
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
            Protect(ws);

            // ---- Rubrics ----
            rubricsWs.Cell(1, 1).Value = "Assessment rubrics";
            rubricsWs.Cell(1, 1).Style.Font.SetBold().Font.SetFontSize(13);
            Note(rubricsWs, 2, RubricColumns.Length, "One row per indicator, from each 'Assessment Rubric' table. Give the strand number it "
                + "follows; give the sub-strand number too when the design gives a rubric for one sub-strand.");
            Headers(rubricsWs, ListHeaderRow, RubricColumns);
            Open(rubricsWs.Range(ListFirstRow, 1, Math.Max(rubricRow, ListFirstRow) + 40, RubricColumns.Length));
            rubricsWs.SheetView.FreezeRows(ListHeaderRow);
            Protect(rubricsWs);

            ws.SetTabActive();
            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return (ms.ToArray(), $"Curriculum - {scope.SubjectName} - {scope.LearningLevelName}.xlsx");
        }

        private static void Headers(IXLWorksheet ws, int row, IEnumerable<(string Header, double Width)> columns)
        {
            var c = 0;
            foreach (var (header, width) in columns)
            {
                c++;
                ws.Cell(row, c).Value = header;
                ws.Column(c).Width = width;
            }
            ws.Range(row, 1, row, c).Style.Font.SetBold().Fill.SetBackgroundColor(XLColor.LightGray)
                .Alignment.SetWrapText().Alignment.SetVertical(XLAlignmentVerticalValues.Top)
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin).Border.SetInsideBorder(XLBorderStyleValues.Thin);
        }

        private static void Note(IXLWorksheet ws, int row, int span, string text)
        {
            ws.Cell(row, 1).Value = text;
            ws.Range(row, 1, row, span).Merge().Style.Font.SetItalic().Font.SetFontColor(XLColor.Gray).Alignment.SetWrapText();
            ws.Row(row).Height = 30;
        }

        // Cells to fill: unlocked, wrapped, bordered.
        private static void Open(IXLRange range) =>
            range.Style.Alignment.SetWrapText().Alignment.SetVertical(XLAlignmentVerticalValues.Top)
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin).Border.SetInsideBorder(XLBorderStyleValues.Hair)
                .Protection.SetLocked(false);

        private static void Protect(IXLWorksheet ws) =>
            ws.Protect().AllowElement(XLSheetProtectionElements.InsertRows | XLSheetProtectionElements.DeleteRows
                | XLSheetProtectionElements.FormatRows | XLSheetProtectionElements.FormatColumns
                | XLSheetProtectionElements.SelectLockedCells | XLSheetProtectionElements.SelectUnlockedCells);

        private static string Statement(string name, string? description) =>
            string.IsNullOrWhiteSpace(description) ? name : $"{name}: {description}";

        private static string Lines(IEnumerable<string> items) => string.Join("\n", items);

        private static void Instructions(IXLWorksheet ws, Scope scope)
        {
            var lines = new[]
            {
                $"Curriculum design - {scope.SubjectName}, {scope.LearningLevelName}",
                "",
                "Capture the whole design. What goes where:",
                $"1. '{CurriculumSheet}': one row per sub-strand from the strand tables (STRAND 1.0 ..., sub-strands 1.1, 1.2 ...):",
                "   - Strand No. and Strand (e.g. 1.0, Creation); Sub-strand No. and Sub-strand (e.g. 1.1, Our God).",
                "     The strand may be left blank on its later sub-strands - it carries down.",
                "   - Lessons: the number in '(7 lessons)'.",
                "   - Specific Learning Outcomes, Suggested Learning Experiences, Key Inquiry Question(s): one item per line.",
                "     Pasting straight from the PDF is fine - a), b) / bullets / 1., 2. mark where each item starts, and",
                "     headings such as 'By the end of the sub strand, the learner should be able to:' are dropped.",
                "   - Core Competencies, Values, PCIs, Links to Other Learning Areas: the boxes under the sub-strand, one entry per",
                "     line, word for word, e.g. 'Communication and Collaboration: Speaking engagingly: The learner ...'.",
                $"     The name before the first colon is linked to the core competency / value / subject when it matches ('{ListsSheet}').",
                "   - Subject Outcome (letter): the subject general learning outcome the sub-strand serves (strand 1 usually a), ...).",
                "   - Level Learning Outcome: pick from the list (the level learning outcomes set up in ShuleNova).",
                "   - Suggested Learning Resources / Assessment Methods / Non-Formal Activities: only where the design gives",
                "     these per sub-strand (upper grades' appendix table). Otherwise use the Appendix sheet.",
                $"2. '{RubricsSheet}': each 'Assessment Rubric' table, one indicator per row with its four levels.",
                $"3. '{OutcomesSheet}': the SUBJECT GENERAL LEARNING OUTCOMES, one per row next to its letter.",
                $"4. '{DesignSheet}': title, source (e.g. KICD, revised 2024), lessons per week (time allocation), essence statement.",
                $"5. '{AppendixSheet}': the appendix lists - suggested resources, assessment methods, assessment tools, non-formal programmes.",
                $"6. '{SectionsSheet}': every other part of the document, word for word - national goals of education, level learning",
                "   outcomes, introduction, community service learning appendix ...",
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
            ws.Cell(25, 1).Style.Font.SetBold();
            ws.Column(1).Width = 125;
        }

        // ================= Upload =================

        private class Statement2
        {
            public string Name { get; set; } = "";
            public string? Description { get; set; }
            public string Key { get; set; } = "";
        }

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
            public List<Statement2> Competencies { get; set; } = new();
            public List<Statement2> Values { get; set; } = new();
            public List<Statement2> Pcis { get; set; } = new();
            public List<Statement2> Links { get; set; } = new();
            public int SubjectOutcomeIndex { get; set; } = -1;
            public int GeneralOutcomeId { get; set; }
            public string? Resources { get; set; }
            public string? Assessment { get; set; }
            public string? NonFormal { get; set; }
        }

        private class RubricRow
        {
            public int Row { get; set; }
            public string StrandNo { get; set; } = "";
            public string SubNo { get; set; } = "";
            public string Indicator { get; set; } = "";
            public string? Exceeds { get; set; }
            public string? Meets { get; set; }
            public string? Approaches { get; set; }
            public string? Below { get; set; }
        }

        private class DesignData
        {
            public string? Title { get; set; }
            public string? Source { get; set; }
            public int? LessonsPerWeek { get; set; }
            public string? Essence { get; set; }
            public List<(string Category, string Name)> Items { get; set; } = new();
            public List<(string Title, string? Content)> Sections { get; set; } = new();
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
                    || !Text(ws.Cell(HeaderRow, COutcomes)).StartsWith("Specific", StringComparison.OrdinalIgnoreCase)
                    || !Text(ws.Cell(HeaderRow, CValues)).StartsWith("Values", StringComparison.OrdinalIgnoreCase))
                {
                    Error(ws.Name, HeaderRow, "The column headings have been moved or changed, or this is an older template. Download a fresh template.");
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
                var values = await _db.Values.AsNoTracking().ToListAsync();
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

                    x.Competencies = Statements(Cell(CCompetencies));
                    foreach (var c in x.Competencies.Where(c => !competencies.Any(k => Key(k.Name) == c.Key)))
                        Warn(ws.Name, rowNo, $"Core competency '{Cut(c.Key.Length == 0 ? c.Name : c.Name.Split(':')[0], 60)}' is not one of the core competencies set up - kept as text, not linked.");
                    x.Values = Statements(Cell(CValues));
                    foreach (var v in x.Values.Where(v => !values.Any(k => Key(k.Name) == v.Key)))
                        Warn(ws.Name, rowNo, $"Value '{Cut(v.Name.Split(':')[0], 60)}' is not one of the values set up - kept as text, not linked.");
                    x.Pcis = Statements(Cell(CPcis));
                    x.Links = Statements(Cell(CLinks));
                    foreach (var s in x.Pcis.Concat(x.Competencies).Concat(x.Values).Concat(x.Links).Where(s => s.Name.Length > MaxName))
                        Error(ws.Name, rowNo, $"'{Cut(s.Name, 60)}...' is longer than {MaxName} characters before its statement - check the colons.");
                    foreach (var p in x.Pcis.Where(p => p.Description?.Length > MaxDescription))
                        Error(ws.Name, rowNo, $"The PCI statement '{Cut(p.Description!, 50)}...' is longer than {MaxDescription} characters.");

                    var letter = Cell(CSubjectOutcome).Trim().TrimEnd(')').ToLowerInvariant();
                    if (letter.Length == 0) Error(ws.Name, rowNo, "Pick the subject outcome (letter) this sub-strand serves.");
                    else if (!outcomeLetters.TryGetValue(letter, out var index)) Error(ws.Name, rowNo, $"Subject outcome '{letter}' is not on the '{OutcomesSheet}' sheet.");
                    else x.SubjectOutcomeIndex = index;

                    var levelText = Cell(CLevelOutcome);
                    var general = generalOutcomes.FirstOrDefault(o => Key(o.Name) == Key(levelText));
                    if (levelText.Length == 0) Error(ws.Name, rowNo, "Pick the level learning outcome this sub-strand serves.");
                    else if (general == null) Error(ws.Name, rowNo, $"Level learning outcome '{Cut(levelText, 60)}' is not one of those set up for {scope.EducationLevelName}.");
                    else x.GeneralOutcomeId = general.Id;

                    x.Resources = ListText(Cell(CResources));
                    x.Assessment = ListText(Cell(CAssessment));
                    x.NonFormal = ListText(Cell(CNonFormal));

                    if (rows.Any(p => Key(p.Strand) == Key(x.Strand) && Key(p.Sub) == Key(x.Sub)))
                        Error(ws.Name, rowNo, $"'{x.Sub}' appears twice under '{x.Strand}'.");
                    rows.Add(x);
                }
                if (rows.Count == 0) Error(ws.Name, null, "No sub-strands are filled in.");

                // ---- Rubrics ----
                var rubrics = new List<RubricRow>();
                if (wb.TryGetWorksheet(RubricsSheet, out var rubricsWs))
                {
                    var last = rubricsWs.LastRowUsed()?.RowNumber() ?? 0;
                    for (var rowNo = ListFirstRow; rowNo <= last; rowNo++)
                    {
                        string Cell(int c) => Text(rubricsWs.Cell(rowNo, c));
                        if (Enumerable.Range(1, RubricColumns.Length).All(c => Cell(c).Length == 0)) continue;
                        var rb = new RubricRow
                        {
                            Row = rowNo, StrandNo = Cell(1), SubNo = Cell(2), Indicator = Cell(3),
                            Exceeds = Blank(Cell(4)), Meets = Blank(Cell(5)), Approaches = Blank(Cell(6)), Below = Blank(Cell(7))
                        };
                        if (rb.Indicator.Length == 0) Error(RubricsSheet, rowNo, "The indicator is missing.");
                        if (!rows.Any(x => NumKey(x.StrandNo) == NumKey(rb.StrandNo)))
                            Error(RubricsSheet, rowNo, $"Strand '{rb.StrandNo}' is not on the Curriculum sheet - give the strand number, e.g. 1.0.");
                        else if (rb.SubNo.Length > 0 && !rows.Any(x => NumKey(x.StrandNo) == NumKey(rb.StrandNo) && NumKey(x.SubNo) == NumKey(rb.SubNo)))
                            Error(RubricsSheet, rowNo, $"Sub-strand '{rb.SubNo}' is not under strand '{rb.StrandNo}' on the Curriculum sheet.");
                        rubrics.Add(rb);
                    }
                }

                // ---- Design, appendix, other sections ----
                var design = new DesignData();
                if (wb.TryGetWorksheet(DesignSheet, out var designWs))
                {
                    design.Title = Blank(Text(designWs.Cell(3, 2)));
                    design.Source = Blank(Text(designWs.Cell(4, 2)));
                    var lpw = Text(designWs.Cell(5, 2));
                    if (lpw.Length > 0)
                    {
                        if (int.TryParse(Regex.Match(lpw, @"\d+").Value, out var n) && n is > 0 and <= 40) design.LessonsPerWeek = n;
                        else Error(DesignSheet, 5, $"Lessons per week '{lpw}' is not a number.");
                    }
                    design.Essence = Blank(Text(designWs.Cell(6, 2)));
                    if (design.Title?.Length > MaxName) Error(DesignSheet, 3, $"The title is longer than {MaxName} characters.");
                    if (design.Source?.Length > MaxName) Error(DesignSheet, 4, $"The source is longer than {MaxName} characters.");
                }
                if (wb.TryGetWorksheet(AppendixSheet, out var appendixWs))
                {
                    var last = appendixWs.LastRowUsed()?.RowNumber() ?? 0;
                    for (var rowNo = ListFirstRow; rowNo <= last; rowNo++)
                    {
                        var label = Text(appendixWs.Cell(rowNo, 1));
                        var itemText = Text(appendixWs.Cell(rowNo, 2));
                        if (label.Length == 0 && itemText.Length == 0) continue;
                        var cat = AppendixCategories.FirstOrDefault(a => Key(a.Label) == Key(label) || Key(a.Category) == Key(label));
                        if (cat.Category == null) { Error(AppendixSheet, rowNo, $"Pick the list for '{Cut(itemText, 40)}' (resources, assessment methods, tools or non-formal programmes)."); continue; }
                        // A cell may hold several items pasted together.
                        foreach (var item in Items(itemText))
                        {
                            if (item.Length > MaxItem) Error(AppendixSheet, rowNo, $"An item is longer than {MaxItem} characters.");
                            design.Items.Add((cat.Category, item));
                        }
                    }
                }
                if (wb.TryGetWorksheet(SectionsSheet, out var sectionsWs))
                {
                    var last = sectionsWs.LastRowUsed()?.RowNumber() ?? 0;
                    for (var rowNo = ListFirstRow; rowNo <= last; rowNo++)
                    {
                        var title = Clean(Text(sectionsWs.Cell(rowNo, 1)));
                        var content = Text(sectionsWs.Cell(rowNo, 2));
                        if (title.Length == 0 && content.Length == 0) continue;
                        if (title.Length == 0) { Error(SectionsSheet, rowNo, "Give the section its heading."); continue; }
                        if (title.Length > MaxName) Error(SectionsSheet, rowNo, $"The heading is longer than {MaxName} characters.");
                        design.Sections.Add((title, Blank(content)));
                    }
                }

                r.Strands = rows.Select(x => Key(x.Strand)).Distinct().Count();
                r.SubStrands = rows.Count;
                r.Lessons = rows.Sum(x => x.Lessons);
                r.SpecificOutcomes = rows.Sum(x => x.Outcomes.Count);
                r.LearningExperiences = rows.Sum(x => x.Experiences.Count);
                r.KeyQuestions = rows.Sum(x => x.Questions.Count);
                r.Pcis = rows.Sum(x => x.Pcis.Count);
                r.CoreCompetencies = rows.Sum(x => x.Competencies.Count);
                r.Values = rows.Sum(x => x.Values.Count);
                r.LearningAreaLinks = rows.Sum(x => x.Links.Count);
                r.Rubrics = rubrics.Count;
                r.AppendixItems = design.Items.Count;
                r.Sections = design.Sections.Count;
                if (rubrics.Count == 0) Warn(RubricsSheet, null, "No assessment rubrics are filled in.");
                if (design.Essence == null) Warn(DesignSheet, null, "No essence statement is filled in.");

                // What is in ShuleNova but not in the workbook stays as it is.
                var keep = rows.Select(x => (Key(x.Strand), Key(x.Sub))).ToHashSet();
                var existing = await _db.SubStrands.AsNoTracking().Include(ss => ss.Strand)
                    .Where(ss => ss.Strand!.SubjectId == subjectId && ss.Strand.LearningLevelId == levelId).ToListAsync();
                foreach (var ss in existing.Where(ss => !keep.Contains((Key(ss.Strand!.Name), Key(ss.Name)))))
                    Warn(null, null, $"'{ss.Strand!.Name} / {ss.Name}' is in ShuleNova but not in this workbook - it is left as it is.");

                if (commit && r.Errors.Count == 0)
                {
                    await SaveAsync(scope, subjectOutcomes, rows, rubrics, design, competencies.Select(c => (c.Id, c.Name)).ToList(),
                        values.Select(v => (v.Id, v.Name)).ToList(), r);
                    r.Saved = true;
                }
            }
            return r;
        }

        private async Task SaveAsync(Scope scope, List<string> subjectOutcomeTexts, List<SubStrandRow> rows, List<RubricRow> rubrics,
            DesignData designData, List<(int Id, string Name)> competencyList, List<(int Id, string Name)> valueList, CurriculumImportResultDto r)
        {
            void Count(string what, int n = 1) { if (n != 0) r.Changes[what] = r.Changes.GetValueOrDefault(what) + n; }
            var curriculumId = await _db.Curricula.OrderBy(c => c.Id).Select(c => (int?)c.Id).FirstOrDefaultAsync();
            var subjects = await _db.Subjects.AsNoTracking().Select(s => new { s.Id, s.Name }).ToListAsync();
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

            var strands = await StrandsWithDetail(scope.SubjectId, scope.LearningLevelId).ToListAsync();
            var competencies = await _db.Competencies.ToListAsync();
            var assessed = await _db.StudentAssessments.Where(a => a.SpecificOutcomeId != null)
                .Select(a => a.SpecificOutcomeId!.Value).Distinct().ToListAsync();
            var savedSubs = new List<(SubStrandRow Row, Strand Strand, SubStrand Sub)>();

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
                    sub.SuggestedResources = x.Resources;
                    sub.SuggestedAssessmentMethods = x.Assessment;
                    sub.SuggestedNonFormalActivities = x.NonFormal;
                    savedSubs.Add((x, strand, sub));

                    // Specific outcomes: matched by text so assessments recorded against them stay put.
                    var linkedIds = x.Competencies.Select(c => competencyList.FirstOrDefault(k => Key(k.Name) == c.Key).Id).Where(id => id > 0).ToHashSet();
                    var linked = competencies.Where(c => linkedIds.Contains(c.Id)).ToList();
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

                    // The rest nothing else points at: replaced.
                    _db.Set<LearningExperience>().RemoveRange(sub.LearningExperiences);
                    _db.Set<KeyQuestion>().RemoveRange(sub.Questions);
                    _db.Set<PCI>().RemoveRange(sub.PCIs);
                    _db.SubStrandCompetencies.RemoveRange(sub.CoreCompetencies);
                    _db.SubStrandValues.RemoveRange(sub.Values);
                    _db.SubStrandLearningAreaLinks.RemoveRange(sub.LearningAreaLinks);
                    sub.LearningExperiences = x.Experiences.Select((e, i) => new LearningExperience { Name = e, Rank = i + 1 }).ToList();
                    sub.Questions = x.Questions.Select((q, i) => new KeyQuestion { Name = q, Rank = i + 1 }).ToList();
                    sub.PCIs = x.Pcis.Select((p, i) => new PCI { Name = p.Name, Description = p.Description, Rank = i + 1 }).ToList();
                    sub.CoreCompetencies = x.Competencies.Select((c, i) => new SubStrandCompetency
                    {
                        Name = c.Name, Description = c.Description, Rank = i + 1,
                        CompetencyId = competencyList.FirstOrDefault(k => Key(k.Name) == c.Key).Id is var cid && cid > 0 ? (int?)cid : null
                    }).ToList();
                    sub.Values = x.Values.Select((v, i) => new SubStrandValue
                    {
                        Name = v.Name, Description = v.Description, Rank = i + 1,
                        ValueId = valueList.FirstOrDefault(k => Key(k.Name) == v.Key).Id is var vid && vid > 0 ? (int?)vid : null
                    }).ToList();
                    sub.LearningAreaLinks = x.Links.Select((l, i) => new SubStrandLearningAreaLink
                    {
                        LearningArea = l.Name, Description = l.Description, Rank = i + 1,
                        SubjectId = SubjectFor(l.Name, subjects.Select(s => (s.Id, s.Name)))
                    }).ToList();
                }
            }
            await _db.SaveChangesAsync();

            // Rubrics for the strands in the workbook: replaced.
            var fileStrands = savedSubs.Select(s => s.Strand).Distinct().ToList();
            var oldRubrics = fileStrands.SelectMany(s => s.Rubrics).ToList();
            _db.AssessmentRubrics.RemoveRange(oldRubrics);
            var rubricRank = 0;
            foreach (var rb in rubrics)
            {
                var strand = savedSubs.First(s => NumKey(s.Row.StrandNo) == NumKey(rb.StrandNo)).Strand;
                var sub = rb.SubNo.Length == 0 ? null
                    : savedSubs.First(s => NumKey(s.Row.StrandNo) == NumKey(rb.StrandNo) && NumKey(s.Row.SubNo) == NumKey(rb.SubNo)).Sub;
                _db.AssessmentRubrics.Add(new AssessmentRubric
                {
                    StrandId = strand.Id, SubStrandId = sub?.Id, Rank = ++rubricRank, Indicator = rb.Indicator,
                    ExceedsExpectations = rb.Exceeds, MeetsExpectations = rb.Meets,
                    ApproachesExpectations = rb.Approaches, BelowExpectations = rb.Below
                });
            }
            Count("Rubric indicators saved", rubrics.Count);

            // The design: one per subject and grade; its lists and sections replaced.
            var design = await _db.CurriculumDesigns.Include(d => d.Items).Include(d => d.Sections)
                .FirstOrDefaultAsync(d => d.SubjectId == scope.SubjectId && d.LearningLevelId == scope.LearningLevelId);
            if (design == null)
            {
                design = new CurriculumDesign { SubjectId = scope.SubjectId, LearningLevelId = scope.LearningLevelId };
                _db.CurriculumDesigns.Add(design);
            }
            design.Title = designData.Title;
            design.Source = designData.Source;
            design.EssenceStatement = designData.Essence;
            design.LessonsPerWeek = designData.LessonsPerWeek;
            _db.CurriculumDesignItems.RemoveRange(design.Items);
            _db.CurriculumDesignSections.RemoveRange(design.Sections);
            design.Items = designData.Items.Select((it, i) => new CurriculumDesignItem { Category = it.Category, Name = it.Name, Rank = i + 1 }).ToList();
            design.Sections = designData.Sections.Select((s, i) => new CurriculumDesignSection { Title = s.Title, Content = s.Content, Rank = i + 1 }).ToList();
            Count("Appendix items saved", design.Items.Count);
            Count("Other sections saved", design.Sections.Count);

            // The design's time allocation is the subject's lesson allocation for the grade.
            if (designData.LessonsPerWeek is int perWeek)
            {
                var allocation = await _db.Set<LessonAllocation>()
                    .FirstOrDefaultAsync(a => a.SubjectId == scope.SubjectId && a.LearningLevelId == scope.LearningLevelId);
                if (allocation == null)
                {
                    allocation = new LessonAllocation { SubjectId = scope.SubjectId, LearningLevelId = scope.LearningLevelId };
                    _db.Set<LessonAllocation>().Add(allocation);
                }
                allocation.LessonsPerWeek = perWeek;
                allocation.Description ??= "From the curriculum design";
            }

            await _db.SaveChangesAsync();
            await tx.CommitAsync();
        }

        // The subject a link names: "Environmental activities" -> "Environmental Activities (including ...)".
        public static int? SubjectFor(string area, IEnumerable<(int Id, string Name)> subjects)
        {
            var key = Key(area);
            if (key.Length < 4) return null;
            var hit = subjects
                .Select(s => (s.Id, Key: Key(Regex.Replace(s.Name, @"\(.*?\)", ""))))
                .Where(s => s.Key.Length >= 4 && (s.Key == key || s.Key.StartsWith(key) || key.StartsWith(s.Key)))
                .OrderBy(s => Math.Abs(s.Key.Length - key.Length))
                .FirstOrDefault();
            return hit.Id > 0 ? hit.Id : null;
        }

        // ================= Helpers =================

        private static string Letter(int i) => ((char)('a' + i)).ToString();

        private static string Text(IXLCell cell)
        {
            var v = cell.HasFormula ? cell.CachedValue.ToString() : cell.GetFormattedString();
            return (v ?? "").Replace("\r\n", "\n").Replace('\r', '\n').Trim();
        }

        // Comparison key: case, spacing and punctuation ignored.
        public static string Key(string? s) => Regex.Replace((s ?? "").ToLowerInvariant(), @"[^a-z0-9]", "");

        // "1.0" / "1" / "1.0." compare equal; "1.1" stays distinct from "11".
        private static string NumKey(string? s)
        {
            var parts = Regex.Matches(s ?? "", @"\d+").Select(m => int.Parse(m.Value).ToString()).ToList();
            while (parts.Count > 1 && parts[^1] == "0") parts.RemoveAt(parts.Count - 1);
            return string.Join(".", parts);
        }

        private static string Clean(string s) => Regex.Replace(s.Replace('\n', ' '), @"\s+", " ").Trim();

        private static string? Blank(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();

        private static string Cut(string s, int max) => s.Length <= max ? s : s[..max].TrimEnd();

        // A list cell kept as text, one item per line.
        private static string? ListText(string cell)
        {
            var items = Items(cell);
            return items.Count == 0 ? null : string.Join("\n", items);
        }

        /// <summary>
        /// "Name: sub-name: statement" entries, one per item - the core competency,
        /// value, PCI and link boxes. The statement starts at "The learner ...";
        /// failing that, after the second colon (or the first when there is one).
        /// The key is the first part, for linking to competencies / values.
        /// </summary>
        private static List<Statement2> Statements(string cell)
        {
            var list = new List<Statement2>();
            foreach (var item in Items(cell, keepFullStop: true))
            {
                string name, description;
                var at = item.IndexOf("The learner", StringComparison.OrdinalIgnoreCase);
                if (at > 0)
                {
                    name = item[..at].Trim().TrimEnd(':').Trim();
                    description = item[at..].Trim();
                }
                else
                {
                    var parts = item.Split(':', 3);
                    (name, description) = parts.Length switch
                    {
                        1 => (item, ""),
                        2 => (parts[0].Trim(), parts[1].Trim()),
                        _ => ($"{parts[0].Trim()}: {parts[1].Trim()}", parts[2].Trim())
                    };
                }
                list.Add(new Statement2 { Name = name, Description = Blank(description), Key = Key(name.Split(':')[0]) });
            }
            return list;
        }

        // Where an item starts: a), (a), 1., 1), i), and the bullets PDFs paste as.
        private static readonly Regex ItemStart = new(@"^\s*(?:\(?[a-z]\)|\(?[ivx]{1,4}\)|\d{1,2}[.)]|[\u2022\u25CF\u25AA\u2023\u2043\u25E6\uF0B7\uF0A7\uF076\uF0D8\-\*])\s*",
            RegexOptions.IgnoreCase);
        // Sub-bullets join the item above, e.g. "listen to the Bible verses; 1st John 3:1; Isaiah 64:7".
        private static readonly Regex SubItemStart = new(@"^\s*[\u25AA\u25E6\uF0A7o]\s+");

        /// <summary>
        /// Splits a cell into items. With markers, a line that starts with one
        /// begins an item and other lines continue it (text pasted from a PDF
        /// wraps mid-sentence), and text before the first marker is a heading;
        /// without markers, each line is an item. Headings such as "The learner
        /// is guided to:" are dropped.
        /// </summary>
        public static List<string> Items(string cell, bool keepFullStop = false)
        {
            var lines = cell.Split('\n').Select(l => l.Trim()).Where(l => l.Length > 0)
                .Where(l => !(l.EndsWith(":") && Regex.IsMatch(l, @"\b(learner|able to|guided to)\b", RegexOptions.IgnoreCase)))
                .ToList();
            var marked = lines.Any(l => ItemStart.IsMatch(l));
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
            return items.Select(i => Regex.Replace(i, @"\s+", " ").Trim().TrimEnd(',', ';').Trim())
                .Select(i => keepFullStop ? i : i.TrimEnd('.').Trim())
                .Where(i => i.Length > 0).ToList();
        }
    }
}
