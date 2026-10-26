using SchoolWebApp.Core.Entities.TeachingDocuments;

namespace SchoolWebApp.Core.DTOs.TeachingDocuments
{
    // Where a document stands, as the tracker colours it. Derived from its
    // approval request - the documents carry no status of their own.
    public static class DocumentStatus
    {
        public const string NotStarted = "NotStarted";
        public const string Draft = "Draft";
        public const string Submitted = "Submitted";
        public const string Returned = "Returned";
        public const string Rejected = "Rejected";
        public const string Approved = "Approved";
        // Tracker only: a week beyond the scheme's number of weeks.
        public const string NotApplicable = "NotApplicable";
    }

    public class TeachingSessionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int AcademicYearId { get; set; }
        public string AcademicYearName { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Weeks { get; set; }
        public bool IsCurrent { get; set; }
    }

    public class TeachingAssignmentDto
    {
        public int StaffDetailsId { get; set; }
        public string StaffName { get; set; } = "";
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = "";
        public int SubjectRank { get; set; }
        public int SchoolClassId { get; set; }
        public string ClassName { get; set; } = "";
        public int LearningLevelId { get; set; }
        public string LearningLevelName { get; set; } = "";
        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public int? SchemeOfWorkId { get; set; }
        public string SchemeStatus { get; set; } = DocumentStatus.NotStarted;
        public int SchemeLessons { get; set; }
    }

    public class TeachingContextDto
    {
        public int UserId { get; set; }
        public int? StaffDetailsId { get; set; }
        public string? StaffName { get; set; }
        public bool IsAdmin { get; set; }
        public bool WholeSchool { get; set; }
        public List<int> HodDepartmentIds { get; set; } = new();
        public List<TeachingSessionDto> Sessions { get; set; } = new();
        public int? CurrentSessionId { get; set; }
    }

    public class WeekDto
    {
        public int WeekNo { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime DueAt { get; set; }
    }

    // ---------- Curriculum for the pickers ----------

    public class CurriculumStrandDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Code { get; set; }
        public List<CurriculumSubStrandDto> SubStrands { get; set; } = new();
    }

    public class CurriculumSubStrandDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Code { get; set; }
        public int Lessons { get; set; }
        public List<string> SpecificOutcomes { get; set; } = new();
        public List<string> LearningExperiences { get; set; } = new();
        public List<string> KeyQuestions { get; set; } = new();
        // Terms of this academic year whose schemes (for this class level) already use it.
        public List<string> ScheduledIn { get; set; } = new();
    }

    // ---------- Schemes of work ----------

    public class SchemeOfWorkLessonDto
    {
        public int Id { get; set; }
        public int WeekNo { get; set; }
        public int LessonNo { get; set; }
        public int? StrandId { get; set; }
        public int? SubStrandId { get; set; }
        public string? StrandName { get; set; }
        public string? SubStrandName { get; set; }
        public string? LearningOutcomes { get; set; }
        public string? LearningExperiences { get; set; }
        public string? KeyInquiryQuestions { get; set; }
        public string? LearningResources { get; set; }
        public string? AssessmentMethods { get; set; }
    }

    public class SchemeOfWorkDto
    {
        public int Id { get; set; }
        public int StaffDetailsId { get; set; }
        public string StaffName { get; set; } = "";
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = "";
        public int SchoolClassId { get; set; }
        public string ClassName { get; set; } = "";
        public int LearningLevelId { get; set; }
        public string LearningLevelName { get; set; } = "";
        public int SessionId { get; set; }
        public string SessionName { get; set; } = "";
        public string AcademicYearName { get; set; } = "";
        public int DepartmentId { get; set; }
        public string? References { get; set; }
        public int NumberOfWeeks { get; set; }
        public int LessonsPerWeek { get; set; }
        public string Status { get; set; } = DocumentStatus.Draft;
        public bool IsLocked { get; set; }
        public bool CanEdit { get; set; }
        public DateTime DueAt { get; set; }
        public List<WeekDto> Weeks { get; set; } = new();
        public List<SchemeOfWorkLessonDto> Lessons { get; set; } = new();
        public DateTime? Modified { get; set; }
        public string? ModifiedBy { get; set; }
    }

    public class CreateSchemeOfWorkDto
    {
        public int SubjectId { get; set; }
        public int SchoolClassId { get; set; }
        public int SessionId { get; set; }
        // Administrators may create a scheme for another teacher.
        public int? StaffDetailsId { get; set; }
    }

    public class SaveSchemeOfWorkDto
    {
        public int Id { get; set; }
        public string? References { get; set; }
        public int NumberOfWeeks { get; set; }
        public int LessonsPerWeek { get; set; }
        public List<SchemeOfWorkLessonDto> Lessons { get; set; } = new();
    }

    public class GenerateSchemeDto
    {
        // Replace the lessons already in the scheme; otherwise only an empty scheme is filled.
        public bool Replace { get; set; }
    }

    public class SchemeCopySourceDto
    {
        public int Id { get; set; }
        public string StaffName { get; set; } = "";
        public string ClassName { get; set; } = "";
        public string SessionName { get; set; } = "";
        public string AcademicYearName { get; set; } = "";
        public int Lessons { get; set; }
        public string Status { get; set; } = DocumentStatus.Draft;
    }

    // ---------- Scheme workbook upload ----------

    public class ImportIssueDto
    {
        // Excel row number, or null for the sheet as a whole.
        public int? Row { get; set; }
        public string Message { get; set; } = "";
    }

    public class SchemeImportSheetDto
    {
        public string SheetName { get; set; } = "";
        public string? StaffName { get; set; }
        public string? SubjectName { get; set; }
        public string? ClassName { get; set; }
        public int? SchemeOfWorkId { get; set; }
        // "Create" a new scheme, "Update" an existing one, "Skip" (errors), or
        // "Empty" (left as downloaded - passed over).
        public string Action { get; set; } = "Skip";
        public int Rows { get; set; }
        public int LessonsFilled { get; set; }
        public int CurriculumMatched { get; set; }
        public List<ImportIssueDto> Errors { get; set; } = new();
        public List<ImportIssueDto> Warnings { get; set; } = new();
        public bool Saved { get; set; }
    }

    public class SchemeImportResultDto
    {
        public bool Committed { get; set; }
        public List<SchemeImportSheetDto> Sheets { get; set; } = new();
        // Problems with the file itself (not a scheme workbook, unreadable...).
        public List<string> FileErrors { get; set; } = new();
    }

    // ---------- Lesson plans ----------

    public class LessonPlanWeekSummaryDto
    {
        public int WeekNo { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime DueAt { get; set; }
        public int? LessonPlanWeekId { get; set; }
        public int Plans { get; set; }
        public int SchemeLessons { get; set; }
        public string Status { get; set; } = DocumentStatus.NotStarted;
        public bool Overdue { get; set; }
    }

    public class LessonPlanStepDto
    {
        public int Id { get; set; }
        public int StepNo { get; set; }
        public string? Activity { get; set; }
    }

    public class LessonPlanDto
    {
        public int Id { get; set; }
        public int LessonPlanWeekId { get; set; }
        public int? SchemeOfWorkLessonId { get; set; }
        public int LessonNo { get; set; }
        public DateTime? LessonDate { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public int? Roll { get; set; }
        public int? StrandId { get; set; }
        public int? SubStrandId { get; set; }
        public string? StrandName { get; set; }
        public string? SubStrandName { get; set; }
        public string? LearningOutcomes { get; set; }
        public string? KeyInquiryQuestions { get; set; }
        public string? LearningResources { get; set; }
        public string? OrganisationOfLearning { get; set; }
        public string? Introduction { get; set; }
        public int? IntroductionMinutes { get; set; }
        public int? DevelopmentMinutes { get; set; }
        public string? Conclusion { get; set; }
        public int? ConclusionMinutes { get; set; }
        public string? ExtendedActivities { get; set; }
        public string? Summary { get; set; }
        public string? Reflection { get; set; }
        public List<LessonPlanStepDto> Steps { get; set; } = new();
        // The record of work entry of the scheme lesson this plan was prepared from.
        public LessonTaughtStatus TaughtStatus { get; set; }
        public DateTime? TaughtOn { get; set; }
    }

    public class LessonPlanWeekDto
    {
        public int Id { get; set; }
        public int WeekNo { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime DueAt { get; set; }
        public string Status { get; set; } = DocumentStatus.Draft;
        public bool IsLocked { get; set; }
        public bool CanEdit { get; set; }
        public SchemeOfWorkDto Scheme { get; set; } = new();
        public List<LessonPlanDto> Plans { get; set; } = new();
    }

    public class CreateLessonPlanWeekDto
    {
        public int SchemeOfWorkId { get; set; }
        public int WeekNo { get; set; }
    }

    // Written after the lesson; allowed once the week is approved.
    public class LessonPlanAfterLessonDto
    {
        public string? Summary { get; set; }
        public string? Reflection { get; set; }
        public LessonTaughtStatus TaughtStatus { get; set; }
        public DateTime? TaughtOn { get; set; }
    }

    // ---------- Record of work ----------

    public class RecordOfWorkRowDto
    {
        public int SchemeOfWorkLessonId { get; set; }
        public int WeekNo { get; set; }
        public int LessonNo { get; set; }
        public string? StrandName { get; set; }
        public string? SubStrandName { get; set; }
        public string? LearningOutcomes { get; set; }
        public LessonTaughtStatus Status { get; set; }
        public DateTime? TaughtOn { get; set; }
        public string? WorkCovered { get; set; }
        public string? Remarks { get; set; }
        public DateTime? Modified { get; set; }
    }

    public class RecordOfWorkCheckDto
    {
        public int Id { get; set; }
        public int WeekNo { get; set; }
        public string CheckedByName { get; set; } = "";
        public int CheckedByUserId { get; set; }
        public DateTime CheckedAt { get; set; }
        public string? Comment { get; set; }
    }

    public class RecordOfWorkDto
    {
        public SchemeOfWorkDto Scheme { get; set; } = new();
        public bool CanCheck { get; set; }
        public List<RecordOfWorkRowDto> Rows { get; set; } = new();
        public List<RecordOfWorkCheckDto> Checks { get; set; } = new();
        public int Planned { get; set; }
        public int Taught { get; set; }
        public int PartlyTaught { get; set; }
        public int NotTaught { get; set; }
    }

    public class SaveRecordOfWorkEntryDto
    {
        public int SchemeOfWorkLessonId { get; set; }
        public LessonTaughtStatus Status { get; set; }
        public DateTime? TaughtOn { get; set; }
        public string? WorkCovered { get; set; }
        public string? Remarks { get; set; }
    }

    public class CheckRecordOfWorkDto
    {
        public int SchemeOfWorkId { get; set; }
        public int WeekNo { get; set; }
        public string? Comment { get; set; }
    }

    // ---------- Tracker ----------

    public class TrackerCellDto
    {
        public int? Id { get; set; }
        public string Status { get; set; } = DocumentStatus.NotStarted;
        public bool Overdue { get; set; }
        public string? AwaitingName { get; set; }
    }

    public class TrackerRowDto
    {
        public int StaffDetailsId { get; set; }
        public string StaffName { get; set; } = "";
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = "";
        public int SubjectRank { get; set; }
        public int SchoolClassId { get; set; }
        public string ClassName { get; set; } = "";
        public int LearningLevelId { get; set; }
        public string LearningLevelName { get; set; } = "";
        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public TrackerCellDto Scheme { get; set; } = new();
        public List<TrackerCellDto> Weeks { get; set; } = new();
        public int PlannedLessons { get; set; }
        public int TaughtLessons { get; set; }
        public int CheckedWeeks { get; set; }
    }

    public class TrackerDto
    {
        public TeachingSessionDto Session { get; set; } = new();
        public List<WeekDto> Weeks { get; set; } = new();
        public DateTime SchemeDueAt { get; set; }
        public string Scope { get; set; } = "own";
        public List<TrackerRowDto> Rows { get; set; } = new();
    }
}
