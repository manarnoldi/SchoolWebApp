using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.Identity;
using SchoolWebApp.Core.Entities.Shared;
using SchoolWebApp.Core.Entities.Students;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolWebApp.Core.Entities.Elections
{
    /// <summary>
    /// A polling station: the classes whose students vote there and the returning
    /// officer who issues their ballots. A class votes at one station per election.
    /// </summary>
    public class ElectionStation : Base
    {
        public int ElectionId { get; set; }
        public Election? Election { get; set; }
        [StringLength(255)]
        public required string Name { get; set; }
        public int? OfficerUserId { get; set; }
        public AppUser? OfficerUser { get; set; }
        public List<ElectionStationClass> Classes { get; set; } = new();
    }

    public class ElectionStationClass : Base
    {
        public int ElectionStationId { get; set; }
        public ElectionStation? ElectionStation { get; set; }
        public int SchoolClassId { get; set; }
        public SchoolClass? SchoolClass { get; set; }
    }

    /// <summary>
    /// Who voted: the student, the station and (Created/CreatedBy) when and which
    /// officer issued the ballot. Says nothing about how they voted.
    /// </summary>
    public class ElectionVoteCast : Base
    {
        public int ElectionId { get; set; }
        public Election? Election { get; set; }
        public int StudentId { get; set; }
        public Student? Student { get; set; }
        public int ElectionStationId { get; set; }
        public ElectionStation? ElectionStation { get; set; }
    }

    /// <summary>
    /// A choice on a ballot, kept apart from who cast it: random ids, no times and
    /// no user, so choices cannot be traced to a voter. All of one ballot's choices
    /// share a BallotId. A race the voter left blank has one row with no candidate.
    /// Not audited.
    /// </summary>
    public class ElectionBallotChoice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        public int ElectionId { get; set; }
        public Guid BallotId { get; set; }
        public int ElectionContestId { get; set; }
        public int? ElectionCandidateId { get; set; }
    }
}
