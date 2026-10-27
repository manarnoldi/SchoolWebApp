using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.Shared;
using SchoolWebApp.Core.Entities.Students;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Elections
{
    /// <summary>
    /// Nominated: waiting for the elections officer. Approved: on the ballot once
    /// nominations close. Rejected / Withdrawn: kept for the record, never on the ballot.
    /// </summary>
    public enum CandidateStatus
    {
        Nominated = 0,
        Approved = 1,
        Rejected = 2,
        Withdrawn = 3
    }

    /// <summary>
    /// A student standing in one race - with their running mate when the position
    /// is a joint ticket. Photos are files on disk (see ElectionPhotoStore); only
    /// their file names are kept here.
    /// </summary>
    public class ElectionCandidate : Base
    {
        public int ElectionContestId { get; set; }
        public ElectionContest? ElectionContest { get; set; }
        public int StudentId { get; set; }
        public Student? Student { get; set; }
        public int? RunningMateStudentId { get; set; }
        public Student? RunningMateStudent { get; set; }
        [StringLength(255)]
        public string? Slogan { get; set; }
        [StringLength(100)]
        public string? PhotoFile { get; set; }
        [StringLength(100)]
        public string? RunningMatePhotoFile { get; set; }
        public CandidateStatus Status { get; set; } = CandidateStatus.Nominated;
        [StringLength(500)]
        public string? StatusNote { get; set; }
        // Place on the ballot, set when nominations close (alphabetical).
        public int BallotOrder { get; set; }
    }

    /// <summary>
    /// The frozen voter register: one row per student, with the class, grade and
    /// gender they had when nominations closed. Which races they vote in follows
    /// from these and the positions. Empty for a Live register.
    /// </summary>
    public class ElectionVoter : Base
    {
        public int ElectionId { get; set; }
        public Election? Election { get; set; }
        public int StudentId { get; set; }
        public Student? Student { get; set; }
        public int SchoolClassId { get; set; }
        public SchoolClass? SchoolClass { get; set; }
        public int LearningLevelId { get; set; }
        public int GenderId { get; set; }
    }
}
