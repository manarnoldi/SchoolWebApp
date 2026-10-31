using SchoolWebApp.Core.DTOs.TeachingDocuments;

namespace SchoolWebApp.Core.DTOs.CBE.Assessments
{
    // Where a subject-class stands with its formative assessments this term.
    public static class AssessmentTrackStatus
    {
        // Every outcome due has been graded for (nearly) every learner.
        public const string UpToDate = "UpToDate";
        // Every outcome due has grades, but some only for part of the class.
        public const string Incomplete = "Incomplete";
        // Some outcomes due have no grades at all.
        public const string Behind = "Behind";
        // Outcomes are due and none has a grade.
        public const string NotStarted = "NotStarted";
        // The scheme has nothing due for assessment yet.
        public const string NothingDue = "NothingDue";
        // No scheme of work, so nothing says what is due.
        public const string NoScheme = "NoScheme";
    }

    public class AssessmentTrackerDto
    {
        public TeachingSessionDto Session { get; set; } = new();
        public int CurriculumId { get; set; }
        public string Scope { get; set; } = "own";
        public int GraceDays { get; set; }
        public List<AssessmentTrackerRowDto> Rows { get; set; } = new();
    }

    public class AssessmentTrackerRowDto
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
        public int LearningLevelRank { get; set; }
        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public int? SchemeOfWorkId { get; set; }

        public int Learners { get; set; }
        // Sub-strands in the scheme, and those whose outcomes are due by now.
        public int SubStrandsPlanned { get; set; }
        public int SubStrandsDue { get; set; }
        // Outcomes due; of those, graded for the class, graded for only part of it, and not graded at all.
        public int OutcomesDue { get; set; }
        public int OutcomesGraded { get; set; }
        public int OutcomesPartlyGraded { get; set; }
        public int OutcomesOverdue { get; set; }
        // Grades on the outcomes due, against one per learner per outcome.
        public int GradesEntered { get; set; }
        public int GradesExpected { get; set; }
        // Every outcome with a grade this term, due or not (and the only count without a scheme).
        public int OutcomesGradedThisTerm { get; set; }
        public string? NextSubStrand { get; set; }
        public DateTime? NextDueAt { get; set; }
        public DateTime? LastEntry { get; set; }
        public string Status { get; set; } = AssessmentTrackStatus.NoScheme;
    }

    public class AssessmentTrackerDetailDto
    {
        public int Learners { get; set; }
        public List<AssessmentTrackerSubStrandDto> SubStrands { get; set; } = new();
        public List<AssessmentTrackerLearnerDto> LearnersMissing { get; set; } = new();
    }

    public class AssessmentTrackerSubStrandDto
    {
        public int StrandId { get; set; }
        public string StrandName { get; set; } = "";
        public int SubStrandId { get; set; }
        public string SubStrandName { get; set; } = "";
        // The last scheme week teaching it, and when its outcomes fall due; none when not in the scheme.
        public int? LastWeek { get; set; }
        public DateTime? DueAt { get; set; }
        public bool Due { get; set; }
        public bool InScheme { get; set; }
        public List<AssessmentTrackerOutcomeDto> Outcomes { get; set; } = new();
    }

    public class AssessmentTrackerOutcomeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int Graded { get; set; }
    }

    public class AssessmentTrackerLearnerDto
    {
        public int StudentId { get; set; }
        public string Name { get; set; } = "";
        public string? Upi { get; set; }
        // Outcomes due without a grade for this learner.
        public int Missing { get; set; }
    }
}
