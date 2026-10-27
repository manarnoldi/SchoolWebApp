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
}
