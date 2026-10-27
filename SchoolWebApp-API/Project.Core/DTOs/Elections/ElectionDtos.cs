using SchoolWebApp.Core.Entities.Elections;

namespace SchoolWebApp.Core.DTOs.Elections
{
    public class ElectionOptionsDto
    {
        public bool CanManage { get; set; }
        public List<IdNameDto> AcademicYears { get; set; } = new();
        public int? CurrentAcademicYearId { get; set; }
        public List<ElectionSectionDto> Sections { get; set; } = new();
        public List<IdNameDto> Genders { get; set; } = new();
        public VoterRegisterMode DefaultRegisterMode { get; set; }
        public int DefaultRefreshSeconds { get; set; }
    }

    public class IdNameDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }

    /// <summary>An education level (school section) with its grades in order.</summary>
    public class ElectionSectionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public List<IdNameDto> Grades { get; set; } = new();
    }

    public class ElectionSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public ElectionType ElectionType { get; set; }
        public int AcademicYearId { get; set; }
        public string AcademicYearName { get; set; } = "";
        public ElectionStage Stage { get; set; }
        public int Positions { get; set; }
        public int Contests { get; set; }
        public DateTime? PollingStart { get; set; }
        public DateTime? PollingEnd { get; set; }
    }

    public class ElectionDetailDto : ElectionSummaryDto
    {
        public string? Description { get; set; }
        public VoterRegisterMode RegisterMode { get; set; }
        public DateTime? RegisterFrozenAt { get; set; }
        public int DashboardRefreshSeconds { get; set; }
        public DateTime? DeclaredAt { get; set; }
        public string? DeclaredBy { get; set; }
        public bool PositionsEditable { get; set; }
        // Approved, and waiting for approval.
        public int Candidates { get; set; }
        public int PendingNominations { get; set; }
        public List<ElectionStage> NextStages { get; set; } = new();
        public List<ElectionPositionDto> PositionList { get; set; } = new();
        public List<ElectionStageChangeDto> History { get; set; } = new();
    }

    public class ElectionStageChangeDto
    {
        public ElectionStage FromStage { get; set; }
        public ElectionStage ToStage { get; set; }
        public string? Note { get; set; }
        public DateTime? At { get; set; }
        public string? By { get; set; }
    }

    public class ElectionPositionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? RunningMateTitle { get; set; }
        public PositionScope Scope { get; set; }
        public int Seats { get; set; }
        public PositionGenderRule GenderRule { get; set; }
        public int? GenderId { get; set; }
        public string? GenderName { get; set; }
        public bool VotersSameGenderOnly { get; set; }
        public int Rank { get; set; }
        public List<int> VoterLevelIds { get; set; } = new();
        public List<int> CandidateLevelIds { get; set; } = new();
        public List<ElectionContestDto> Contests { get; set; } = new();
    }

    public class ElectionContestDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int? EducationLevelId { get; set; }
        public int? LearningLevelId { get; set; }
        public int? SchoolClassId { get; set; }
    }

    public class SaveElectionDto
    {
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public int AcademicYearId { get; set; }
        public VoterRegisterMode RegisterMode { get; set; }
        public DateTime? PollingStart { get; set; }
        public DateTime? PollingEnd { get; set; }
        public int DashboardRefreshSeconds { get; set; } = 10;
    }

    public class SavePositionDto
    {
        public string Name { get; set; } = "";
        public string? RunningMateTitle { get; set; }
        public PositionScope Scope { get; set; }
        public int Seats { get; set; } = 1;
        public PositionGenderRule GenderRule { get; set; }
        public int? GenderId { get; set; }
        public bool VotersSameGenderOnly { get; set; }
        public int Rank { get; set; }
        public List<int> VoterLevelIds { get; set; } = new();
        public List<int> CandidateLevelIds { get; set; } = new();
    }

    public class ChangeStageDto
    {
        public ElectionStage ToStage { get; set; }
        public string? Note { get; set; }
    }

    // Nominations

    public class NominationsDto
    {
        public int ElectionId { get; set; }
        public string ElectionName { get; set; } = "";
        public ElectionStage Stage { get; set; }
        public bool CanManage { get; set; }
        // Nominations can be added, approved and removed.
        public bool CanNominate { get; set; }
        // Photos can still be changed (until polling opens).
        public bool CanChangePhotos { get; set; }
        public List<NominationPositionDto> Positions { get; set; } = new();
    }

    public class NominationPositionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? RunningMateTitle { get; set; }
        public int Seats { get; set; }
        public PositionGenderRule GenderRule { get; set; }
        public string? GenderName { get; set; }
        public List<NominationContestDto> Contests { get; set; } = new();
    }

    public class NominationContestDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public List<CandidateDto> Candidates { get; set; } = new();
    }

    public class CandidateDto
    {
        public int Id { get; set; }
        public int ContestId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = "";
        public string Upi { get; set; } = "";
        public string ClassName { get; set; } = "";
        public string GenderName { get; set; } = "";
        public int? RunningMateStudentId { get; set; }
        public string? RunningMateName { get; set; }
        public string? RunningMateClassName { get; set; }
        public string? Slogan { get; set; }
        public string? PhotoUrl { get; set; }
        public string? RunningMatePhotoUrl { get; set; }
        public CandidateStatus Status { get; set; }
        public string? StatusNote { get; set; }
        public int BallotOrder { get; set; }
        public string? NominatedBy { get; set; }
        public DateTime? NominatedAt { get; set; }
    }

    /// <summary>A student who can be nominated in a race; Taken says why not, when they already stand.</summary>
    public class EligibleStudentDto
    {
        public int StudentId { get; set; }
        public string Name { get; set; } = "";
        public string Upi { get; set; } = "";
        public string ClassName { get; set; } = "";
        public string GenderName { get; set; } = "";
        public string? Taken { get; set; }
    }

    public class SaveCandidateDto
    {
        public int StudentId { get; set; }
        public int? RunningMateStudentId { get; set; }
        public string? Slogan { get; set; }
    }

    public class CandidateStatusDto
    {
        public CandidateStatus Status { get; set; }
        public string? Note { get; set; }
    }

    // Voter register

    public class RegisterSummaryDto
    {
        public VoterRegisterMode Mode { get; set; }
        public DateTime? FrozenAt { get; set; }
        public int Students { get; set; }
        public List<RegisterContestDto> Contests { get; set; } = new();
    }

    public class RegisterContestDto
    {
        public int ContestId { get; set; }
        public string PositionName { get; set; } = "";
        public string Name { get; set; } = "";
        public int Voters { get; set; }
    }

    public class RegisterVoterDto
    {
        public int StudentId { get; set; }
        public string Name { get; set; } = "";
        public string Upi { get; set; } = "";
        public string ClassName { get; set; } = "";
        public string GenderName { get; set; } = "";
    }
}
