using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.DTOs.Elections;
using SchoolWebApp.Core.Entities.Elections;

namespace SchoolWebApp.API.Services.Elections
{
    /// <summary>
    /// Counts an election: votes per candidate, turnout per race and station, and
    /// who is winning. Each race's ballots are checked against the voters marked as
    /// having voted in it. A tie at the last seat stays open until lots are drawn.
    /// </summary>
    public class ElectionResultsService
    {
        private readonly ApplicationDbContext _db;
        private readonly ElectionService _elections;

        public ElectionResultsService(ApplicationDbContext db, ElectionService elections)
        {
            _db = db;
            _elections = elections;
        }

        public async Task<ResultsDto> CountAsync(Election e)
        {
            var contests = await _db.ElectionContests.AsNoTracking()
                .Include(c => c.ElectionPosition).ThenInclude(p => p!.Levels)
                .Include(c => c.Candidates.Where(x => x.Status == CandidateStatus.Approved)).ThenInclude(x => x.Student)
                .Include(c => c.Candidates.Where(x => x.Status == CandidateStatus.Approved)).ThenInclude(x => x.RunningMateStudent)
                .Where(c => c.ElectionPosition!.ElectionId == e.Id)
                .AsSplitQuery()
                .OrderBy(c => c.ElectionPosition!.Rank).ThenBy(c => c.ElectionPosition!.Name).ThenBy(c => c.Rank)
                .ToListAsync();

            var tallies = await _db.ElectionBallotChoices.AsNoTracking().Where(c => c.ElectionId == e.Id)
                .GroupBy(c => new { c.ElectionContestId, c.ElectionCandidateId })
                .Select(g => new { g.Key.ElectionContestId, g.Key.ElectionCandidateId, Count = g.Count() }).ToListAsync();
            var ballots = await _db.ElectionBallotChoices.AsNoTracking().Where(c => c.ElectionId == e.Id)
                .GroupBy(c => c.ElectionContestId)
                .Select(g => new { ContestId = g.Key, Count = g.Select(x => x.BallotId).Distinct().Count() }).ToListAsync();
            var casts = await _db.ElectionVoteCasts.AsNoTracking().Where(v => v.ElectionId == e.Id)
                .Select(v => new { v.StudentId, v.ElectionStationId }).ToListAsync();
            var voted = casts.Select(c => c.StudentId).ToHashSet();

            var places = await _elections.VoterPlacesAsync(e);
            var genders = await _db.Genders.AsNoTracking().ToDictionaryAsync(g => g.Id, g => g.Name);
            var ids = contests.SelectMany(c => c.Candidates).Select(x => x.StudentId).ToList();
            var classes = await _db.StudentClasses.AsNoTracking()
                .Where(sc => ids.Contains(sc.StudentId) && sc.SchoolClass!.AcademicYearId == e.AcademicYearId)
                .Select(sc => new { sc.Id, sc.StudentId, sc.SchoolClass!.Name }).ToListAsync();
            var className = classes.GroupBy(x => x.StudentId).ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.Id).First().Name);
            var declared = e.Stage == ElectionStage.Declared;

            var races = new List<ResultRaceDto>();
            foreach (var c in contests)
            {
                var p = c.ElectionPosition!;
                var inRace = places.Where(s => ElectionService.InContest(c, p, s, ContestRole.Voter)).ToList();
                var perGender = p.GenderRule == PositionGenderRule.SeatsPerGender;
                var cands = c.Candidates.Select(x => new ResultCandidateDto
                {
                    CandidateId = x.Id,
                    Name = x.Student!.FullName,
                    ClassName = className.GetValueOrDefault(x.StudentId, ""),
                    GenderName = genders.GetValueOrDefault(x.Student.GenderId, ""),
                    RunningMateName = x.RunningMateStudent?.FullName,
                    PhotoUrl = x.PhotoFile == null ? null : $"/elections/photos/{x.PhotoFile}",
                    Votes = declared ? x.Votes : tallies.FirstOrDefault(t => t.ElectionContestId == c.Id && t.ElectionCandidateId == x.Id)?.Count ?? 0,
                    TieBreak = x.TieBreak,
                    Winning = declared && x.IsElected,
                    LeadershipRecordedAt = x.LeadershipRecordedAt
                }).ToList();

                var race = new ResultRaceDto
                {
                    ContestId = c.Id,
                    PositionId = p.Id,
                    PositionName = p.Name,
                    Name = c.Name,
                    RunningMateTitle = p.RunningMateTitle,
                    Seats = p.Seats,
                    SeatsPerGender = perGender,
                    Voters = inRace.Count,
                    Voted = inRace.Count(s => voted.Contains(s.StudentId)),
                    Ballots = ballots.FirstOrDefault(b => b.ContestId == c.Id)?.Count ?? 0,
                    Blank = tallies.FirstOrDefault(t => t.ElectionContestId == c.Id && t.ElectionCandidateId == null)?.Count ?? 0
                };
                var counted = cands.Sum(x => x.Votes);
                foreach (var x in cands) x.Percent = counted == 0 ? 0 : Math.Round(100.0 * x.Votes / counted, 1);

                // Who leads: the top Seats (of each gender when seats are per gender).
                if (!declared)
                    foreach (var group in perGender ? cands.GroupBy(x => x.GenderName).Select(g => g.ToList()) : new[] { cands })
                        race.Tied |= MarkLeaders(group, p.Seats);
                race.Candidates = cands.OrderByDescending(x => x.Votes).ThenByDescending(x => x.TieBreak).ThenBy(x => x.Name).ToList();
                races.Add(race);
            }

            var stations = await _db.ElectionStations.AsNoTracking().Include(s => s.Classes)
                .Where(s => s.ElectionId == e.Id).OrderBy(s => s.Name).ToListAsync();
            return new ResultsDto
            {
                ElectionId = e.Id,
                ElectionName = e.Name,
                Stage = e.Stage,
                RefreshSeconds = e.DashboardRefreshSeconds,
                DeclaredAt = e.DeclaredAt,
                DeclaredBy = e.DeclaredBy,
                Voters = places.Count,
                Voted = places.Count(s => voted.Contains(s.StudentId)),
                AsAt = DateTime.UtcNow,
                Races = races,
                Stations = stations.Select(s =>
                {
                    var classIds = s.Classes.Select(c => c.SchoolClassId).ToHashSet();
                    var here = places.Where(p => classIds.Contains(p.SchoolClassId)).ToList();
                    return new StationTurnoutDto
                    {
                        StationId = s.Id,
                        Name = s.Name,
                        Voters = here.Count,
                        Voted = casts.Count(c => c.ElectionStationId == s.Id)
                    };
                }).ToList()
            };
        }

        /// <summary>
        /// Marks the top seats as winning, drawn lots (TieBreak) settling equal votes.
        /// Returns true when candidates tie across the last seat and the draw has not
        /// separated them; they are marked InTie and none of them as winning.
        /// </summary>
        private static bool MarkLeaders(List<ResultCandidateDto> group, int seats)
        {
            var order = group.OrderByDescending(x => x.Votes).ThenByDescending(x => x.TieBreak).ToList();
            if (order.Count <= seats)
            {
                foreach (var x in order) x.Winning = true;
                return false;
            }
            var last = order[seats - 1];
            var next = order[seats];
            if (last.Votes == next.Votes && last.TieBreak == next.TieBreak)
            {
                foreach (var x in order.Where(x => x.Votes > last.Votes)) x.Winning = true;
                foreach (var x in order.Where(x => x.Votes == last.Votes)) x.InTie = true;
                return true;
            }
            foreach (var x in order.Take(seats)) x.Winning = true;
            return false;
        }

        /// <summary>Writes the count onto the candidates: their votes and who is elected.</summary>
        public async Task DeclareAsync(Election e, ResultsDto results, string? by)
        {
            var winners = results.Races.SelectMany(r => r.Candidates).ToDictionary(c => c.CandidateId);
            var candidates = await _db.ElectionCandidates
                .Where(c => c.ElectionContest!.ElectionPosition!.ElectionId == e.Id && c.Status == CandidateStatus.Approved)
                .ToListAsync();
            foreach (var c in candidates)
            {
                var r = winners.GetValueOrDefault(c.Id);
                c.Votes = r?.Votes ?? 0;
                c.IsElected = r?.Winning ?? false;
            }
            e.DeclaredAt = DateTime.UtcNow;
            e.DeclaredBy = by;
        }
    }
}
