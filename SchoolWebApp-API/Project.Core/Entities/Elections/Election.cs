using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.School;
using SchoolWebApp.Core.Entities.Settings;
using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Elections
{
    /// <summary>Who votes. Only student elections are run so far; the others are reserved.</summary>
    public enum ElectionType
    {
        StudentLeadership = 1,
        ParentRepresentatives = 2,
        Staff = 3
    }

    /// <summary>
    /// Where an election is. It moves forward one stage at a time (some steps can
    /// be taken back while nothing depends on them); each move is recorded.
    /// </summary>
    public enum ElectionStage
    {
        Draft = 0,
        Nominations = 1,
        CandidatesFinal = 2,
        Polling = 3,
        PollingClosed = 4,
        Declared = 5
    }

    /// <summary>
    /// Frozen: the voter register is a snapshot taken when nominations close, so a
    /// student who changes class afterwards votes where they were registered.
    /// Live: eligibility follows the student's current class up to the moment they vote.
    /// </summary>
    public enum VoterRegisterMode
    {
        Frozen = 0,
        Live = 1
    }

    public class Election : Base
    {
        [StringLength(255)]
        public required string Name { get; set; }
        [StringLength(1000)]
        public string? Description { get; set; }
        public ElectionType ElectionType { get; set; } = ElectionType.StudentLeadership;
        public int AcademicYearId { get; set; }
        public AcademicYear? AcademicYear { get; set; }
        public ElectionStage Stage { get; set; } = ElectionStage.Draft;
        public VoterRegisterMode RegisterMode { get; set; } = VoterRegisterMode.Frozen;
        public DateTime? RegisterFrozenAt { get; set; }
        // Planned polling window, shown to officers; polling opens and closes by stage.
        public DateTime? PollingStart { get; set; }
        public DateTime? PollingEnd { get; set; }
        // How often the live results screen refreshes.
        public int DashboardRefreshSeconds { get; set; } = 10;
        public DateTime? DeclaredAt { get; set; }
        [StringLength(255)]
        public string? DeclaredBy { get; set; }

        public List<ElectionPosition> Positions { get; set; } = new();
        public List<ElectionStageChange> StageChanges { get; set; } = new();
    }

    /// <summary>A stage move, with who made it (CreatedBy) and when (Created).</summary>
    public class ElectionStageChange : Base
    {
        public int ElectionId { get; set; }
        public Election? Election { get; set; }
        public ElectionStage FromStage { get; set; }
        public ElectionStage ToStage { get; set; }
        [StringLength(500)]
        public string? Note { get; set; }
    }

    /// <summary>
    /// How a position is contested. Single: one contest across all its voter grades
    /// (e.g. School President for Grades 4-9). PerEducationLevel: one per school section
    /// (e.g. a Senator for Upper primary and one for Junior School). PerGrade: one per
    /// grade. PerClass: one per class (e.g. Class Representative).
    /// </summary>
    public enum PositionScope
    {
        Single = 0,
        PerEducationLevel = 1,
        PerGrade = 2,
        PerClass = 3
    }

    /// <summary>
    /// None. SingleGender: only candidates of one gender (e.g. Girls' Representative),
    /// optionally voted for by that gender only. SeatsPerGender: Seats winners of
    /// each gender (e.g. one boy and one girl).
    /// </summary>
    public enum PositionGenderRule
    {
        None = 0,
        SingleGender = 1,
        SeatsPerGender = 2
    }

    public class ElectionPosition : Base
    {
        public int ElectionId { get; set; }
        public Election? Election { get; set; }
        [StringLength(255)]
        public required string Name { get; set; }
        // Set for a joint ticket: one vote elects the candidate and their running mate
        // (e.g. "Deputy President" on a President ticket).
        [StringLength(255)]
        public string? RunningMateTitle { get; set; }
        public PositionScope Scope { get; set; }
        public int Seats { get; set; } = 1;
        public PositionGenderRule GenderRule { get; set; }
        public int? GenderId { get; set; }
        public Gender? Gender { get; set; }
        public bool VotersSameGenderOnly { get; set; }
        public int Rank { get; set; }

        // Voter grades define the position's reach; candidate grades who may stand
        // (none = any voter grade).
        public List<ElectionPositionLevel> Levels { get; set; } = new();
        public List<ElectionContest> Contests { get; set; } = new();
    }

    public enum PositionLevelRole
    {
        Voter = 0,
        Candidate = 1
    }

    public class ElectionPositionLevel : Base
    {
        public int ElectionPositionId { get; set; }
        public ElectionPosition? ElectionPosition { get; set; }
        public int LearningLevelId { get; set; }
        public LearningLevel? LearningLevel { get; set; }
        public PositionLevelRole Role { get; set; }
    }

    /// <summary>
    /// One race for a position: the whole position (Single), or one section, grade or
    /// class of it. Candidates, the voter register and results are per contest.
    /// </summary>
    public class ElectionContest : Base
    {
        public int ElectionPositionId { get; set; }
        public ElectionPosition? ElectionPosition { get; set; }
        [StringLength(255)]
        public required string Name { get; set; }
        public int? EducationLevelId { get; set; }
        public EducationLevel? EducationLevel { get; set; }
        public int? LearningLevelId { get; set; }
        public LearningLevel? LearningLevel { get; set; }
        public int? SchoolClassId { get; set; }
        public SchoolClass? SchoolClass { get; set; }
        public int Rank { get; set; }

        public List<ElectionCandidate> Candidates { get; set; } = new();
    }
}
