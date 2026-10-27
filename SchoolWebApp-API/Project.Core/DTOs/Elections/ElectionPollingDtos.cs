using SchoolWebApp.Core.Entities.Elections;

namespace SchoolWebApp.Core.DTOs.Elections
{
    // Polling stations

    public class StationDto
    {
        public int Id { get; set; }
        public int ElectionId { get; set; }
        public string Name { get; set; } = "";
        public int? OfficerUserId { get; set; }
        public string? OfficerName { get; set; }
        public List<IdNameDto> Classes { get; set; } = new();
        public int Voters { get; set; }
        public int Voted { get; set; }
    }

    public class StationsDto
    {
        public int ElectionId { get; set; }
        public string ElectionName { get; set; } = "";
        public ElectionStage Stage { get; set; }
        public bool CanManage { get; set; }
        public List<StationDto> Stations { get; set; } = new();
        // Classes with voters that no station covers.
        public List<IdNameDto> UncoveredClasses { get; set; } = new();
        // Every class with voters, for the station editor.
        public List<IdNameDto> Classes { get; set; } = new();
        public List<IdNameDto> Officers { get; set; } = new();
    }

    public class SaveStationDto
    {
        public string Name { get; set; } = "";
        public int? OfficerUserId { get; set; }
        public List<int> SchoolClassIds { get; set; } = new();
    }

    /// <summary>Stations the caller can run: their own, or every station for elections officers.</summary>
    public class MyStationDto
    {
        public int StationId { get; set; }
        public string StationName { get; set; } = "";
        public int ElectionId { get; set; }
        public string ElectionName { get; set; } = "";
        public ElectionStage Stage { get; set; }
        public string? OfficerName { get; set; }
        public int Voters { get; set; }
        public int Voted { get; set; }
    }

    // The station screen

    public class StationRegisterDto
    {
        public int StationId { get; set; }
        public string StationName { get; set; } = "";
        public int ElectionId { get; set; }
        public string ElectionName { get; set; } = "";
        public ElectionStage Stage { get; set; }
        public string? OfficerName { get; set; }
        public List<StationVoterDto> Voters { get; set; } = new();
    }

    public class StationVoterDto
    {
        public int StudentId { get; set; }
        public string Name { get; set; } = "";
        public string Upi { get; set; } = "";
        public string ClassName { get; set; } = "";
        public string GenderName { get; set; } = "";
        public bool Voted { get; set; }
        public DateTime? VotedAt { get; set; }
    }

    public class BallotDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = "";
        public string ClassName { get; set; } = "";
        public List<BallotRaceDto> Races { get; set; } = new();
    }

    public class BallotRaceDto
    {
        public int ContestId { get; set; }
        public string Name { get; set; } = "";
        public string PositionName { get; set; } = "";
        public string? RunningMateTitle { get; set; }
        public int Seats { get; set; }
        // Seats is per gender: the voter picks up to Seats of each.
        public bool SeatsPerGender { get; set; }
        public List<BallotCandidateDto> Candidates { get; set; } = new();
    }

    public class BallotCandidateDto
    {
        public int CandidateId { get; set; }
        public int BallotOrder { get; set; }
        public string Name { get; set; } = "";
        public string ClassName { get; set; } = "";
        public string GenderName { get; set; } = "";
        public string? RunningMateName { get; set; }
        public string? Slogan { get; set; }
        public string? PhotoUrl { get; set; }
        public string? RunningMatePhotoUrl { get; set; }
    }

    public class CastBallotDto
    {
        public int StudentId { get; set; }
        public List<CastChoiceDto> Choices { get; set; } = new();
    }

    public class CastChoiceDto
    {
        public int ContestId { get; set; }
        public List<int> CandidateIds { get; set; } = new();
    }

    // Results

    public class ResultsDto
    {
        public int ElectionId { get; set; }
        public string ElectionName { get; set; } = "";
        public ElectionStage Stage { get; set; }
        public int RefreshSeconds { get; set; }
        public DateTime? DeclaredAt { get; set; }
        public string? DeclaredBy { get; set; }
        public int Voters { get; set; }
        public int Voted { get; set; }
        public DateTime AsAt { get; set; }
        public List<ResultRaceDto> Races { get; set; } = new();
        public List<StationTurnoutDto> Stations { get; set; } = new();
    }

    public class ResultRaceDto
    {
        public int ContestId { get; set; }
        public int PositionId { get; set; }
        public string PositionName { get; set; } = "";
        public string Name { get; set; } = "";
        public string? RunningMateTitle { get; set; }
        public int Seats { get; set; }
        public bool SeatsPerGender { get; set; }
        // Voters on the register for this race, and how many have voted.
        public int Voters { get; set; }
        public int Voted { get; set; }
        // Ballots counted for the race; equals Voted when the count checks out.
        public int Ballots { get; set; }
        public int Blank { get; set; }
        // A tie at the last seat that the draw has not settled.
        public bool Tied { get; set; }
        public List<ResultCandidateDto> Candidates { get; set; } = new();
    }

    public class ResultCandidateDto
    {
        public int CandidateId { get; set; }
        public string Name { get; set; } = "";
        public string ClassName { get; set; } = "";
        public string GenderName { get; set; } = "";
        public string? RunningMateName { get; set; }
        public string? PhotoUrl { get; set; }
        public int Votes { get; set; }
        public double Percent { get; set; }
        // Leading (before declaration) or elected (after).
        public bool Winning { get; set; }
        public bool InTie { get; set; }
        public int TieBreak { get; set; }
        public DateTime? LeadershipRecordedAt { get; set; }
    }

    public class StationTurnoutDto
    {
        public int StationId { get; set; }
        public string Name { get; set; } = "";
        public int Voters { get; set; }
        public int Voted { get; set; }
    }

    public class TieBreakDto
    {
        // Candidates in the order the lots were drawn, first = wins the seat.
        public List<int> CandidateIds { get; set; } = new();
    }

    // Leadership records

    public class LeadershipDto
    {
        public int ElectionId { get; set; }
        public string ElectionName { get; set; } = "";
        public ElectionStage Stage { get; set; }
        public List<IdNameDto> ClassRoles { get; set; } = new();
        public List<IdNameDto> Responsibilities { get; set; } = new();
        public List<LeadershipPositionDto> Positions { get; set; } = new();
    }

    public class LeadershipPositionDto
    {
        public int PositionId { get; set; }
        public string Name { get; set; } = "";
        public string? RunningMateTitle { get; set; }
        public bool ClassRaces { get; set; }
        public int? ClassLeadershipRoleId { get; set; }
        public int? ResponsibilityId { get; set; }
        public List<LeadershipWinnerDto> Winners { get; set; } = new();
    }

    public class LeadershipWinnerDto
    {
        public int CandidateId { get; set; }
        public string Race { get; set; } = "";
        public string Name { get; set; } = "";
        public string? RunningMateName { get; set; }
        public string ClassName { get; set; } = "";
        public DateTime? RecordedAt { get; set; }
    }

    public class LeadershipMappingDto
    {
        public int PositionId { get; set; }
        public int? ClassLeadershipRoleId { get; set; }
        public int? ResponsibilityId { get; set; }
    }

    public class RecordLeadershipDto
    {
        public List<LeadershipMappingDto> Positions { get; set; } = new();
    }
}
