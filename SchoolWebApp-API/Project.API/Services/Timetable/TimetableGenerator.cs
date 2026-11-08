using System.Diagnostics;
using SchoolWebApp.Core.Entities.Timetable;

namespace SchoolWebApp.API.Services.Timetable
{
    /// <summary>
    /// Builds a week's timetable from <see cref="TimetableInput"/>.
    ///
    /// Each class's lessons become cards ("units"): a single or a double lesson
    /// of one subject, or one meeting of an option group (CRE and IRE together,
    /// each with its own teacher). Cards are placed hardest first, each in the
    /// free slot that scores best. A card with no free slot may bump one card
    /// that is in its way, if that card can go somewhere else. The whole run is
    /// repeated with shuffled orders until the time is up, keeping the best.
    ///
    /// Always kept: a class has one lesson at a time; a teacher is never in two
    /// places (compared by clock time, so it holds across bell schedules) or in
    /// a time off; morning-only / afternoon-only; most lessons of a subject a
    /// day; a teacher's most lessons a day; doubles in two lessons with no break
    /// between. Tried for: preferred mornings / afternoons, a subject spread over
    /// the week, not last lesson, a teacher's most lessons in a row.
    /// </summary>
    public class TimetableGenerator
    {
        private readonly TimetableInput _input;
        private readonly List<ClassGrid> _classes = new();
        private readonly List<Unit> _units = new();
        private readonly Dictionary<int, string> _teacherNames = new();
        private Random _rng = new(1);

        public TimetableGenerator(TimetableInput input) => _input = input;

        public List<string> Warnings { get; } = new();

        // ------------------------------------------------------------------ model

        private sealed class Cell
        {
            public int Day;
            public int Index;            // position among the schedule's lesson slots
            public BellSlot Bell = null!;
            public bool Afternoon;
            public bool LastOfDay;
            public bool LinkedToNext;    // the next lesson follows with no break between
            public bool Blocked;         // a fixed activity
            public Unit? Unit;
        }

        private sealed class ClassGrid
        {
            public TimetableClass Class = null!;
            public List<int> Days = new();
            public int Lessons;
            public Cell[,] Cells = null!;   // [day position, lesson index]
            public string Name => Class.SchoolClass.Name;
        }

        private sealed class Member
        {
            public int SubjectId;
            public int? StaffId;
        }

        private sealed class Unit
        {
            public int Id;
            public ClassGrid Grid = null!;
            public List<Member> Members = new();
            public int Length = 1;
            public SubjectPlacement Placement;
            public int MaxPerDay = 1;
            public bool AvoidLast;
            public string Key = "";         // what "a day" is counted by: the subject, or the option group
            public string Label = "";
            public bool Fixed;              // placed before generating (fixed slot or locked card)
            public int DayPos = -1;
            public int Index = -1;
            public double Difficulty;
            public bool Placed => DayPos >= 0;
            public IEnumerable<int> Teachers => Members.Where(m => m.StaffId != null).Select(m => m.StaffId!.Value).Distinct();
        }

        private sealed record Busy(int Day, TimeOnly Start, TimeOnly End, Unit Unit);

        private readonly Dictionary<int, List<Busy>> _busy = new();

        /// <summary>A card already on the timetable that must stay where it is (locked).</summary>
        public sealed record LockedLesson(int SchoolClassId, int SubjectId, int Day, int BellSlotId, int Length, int? LinkNo);

        public sealed record Placement(int SchoolClassId, int SubjectId, int? StaffDetailsId, int? Day, int? BellSlotId,
            int Length, int? LinkNo, bool IsLocked);

        public sealed record Unplaced(string ClassName, string Subjects, string Teachers, int Length, string Reason);

        public sealed class Result
        {
            public List<Placement> Lessons { get; set; } = new();
            public List<Unplaced> Unplaced { get; set; } = new();
            public int Placed { get; set; }
            public int Total { get; set; }
            public int Attempts { get; set; }
            public double Seconds { get; set; }
            public double Score { get; set; }
        }

        // ------------------------------------------------------------------ build

        private void Build()
        {
            foreach (var c in _input.Classes)
            {
                var s = c.Schedule!;
                var days = TimetableInputService.ParseDays(s.Days);
                var all = s.Slots.OrderBy(x => x.Rank).ToList();
                var lessons = all.Where(x => x.Kind == BellSlotKind.Lesson).ToList();
                var g = new ClassGrid { Class = c, Days = days, Lessons = lessons.Count, Cells = new Cell[days.Count, lessons.Count] };
                for (var d = 0; d < days.Count; d++)
                    for (var i = 0; i < lessons.Count; i++)
                    {
                        var pos = all.IndexOf(lessons[i]);
                        g.Cells[d, i] = new Cell
                        {
                            Day = days[d],
                            Index = i,
                            Bell = lessons[i],
                            Afternoon = lessons[i].Part == DayPart.Afternoon,
                            LastOfDay = i == lessons.Count - 1,
                            LinkedToNext = i + 1 < lessons.Count && pos + 1 < all.Count && all[pos + 1].Id == lessons[i + 1].Id,
                            Blocked = s.FixedSlots.Any(f => f.SubjectId == null && f.Day == days[d] && f.BellSlotId == lessons[i].Id
                                && (f.LearningLevelId == null || f.LearningLevelId == c.SchoolClass.LearningLevelId))
                        };
                    }
                _classes.Add(g);

                foreach (var n in c.Subjects.Where(x => !x.Excluded && x.StaffDetailsId != null))
                    if (n.TeacherName != null) _teacherNames[n.StaffDetailsId!.Value] = n.TeacherName;

                // Subjects on their own: doubles first, then singles.
                foreach (var n in c.Subjects.Where(x => !x.Excluded && x.OptionGroupId == null))
                {
                    var doubles = Math.Min(n.Rule?.DoubleLessons ?? 0, n.LessonsPerWeek / 2);
                    var singles = n.LessonsPerWeek - doubles * 2;
                    // More meetings than days (6 lessons over 5 days) needs some days twice.
                    var perDay = Math.Max(1, n.Rule?.MaxPerDay ?? 1);
                    var needed = (int)Math.Ceiling((doubles + singles) / (double)Math.Max(1, days.Count));
                    if (needed > perDay)
                    {
                        Warnings.Add($"{c.SchoolClass.Name}: {n.SubjectAbbr} has {n.LessonsPerWeek} lessons over {days.Count} days, so it meets up to {needed} times on some days.");
                        perDay = needed;
                    }
                    for (var k = 0; k < doubles + singles; k++)
                        _units.Add(new Unit
                        {
                            Grid = g,
                            Members = { new Member { SubjectId = n.SubjectId, StaffId = n.StaffDetailsId } },
                            Length = k < doubles ? 2 : 1,
                            Placement = n.Rule?.Placement ?? SubjectPlacement.Any,
                            MaxPerDay = perDay,
                            AvoidLast = n.Rule?.AvoidLastLesson ?? false,
                            Key = "S" + n.SubjectId,
                            Label = n.SubjectAbbr
                        });
                }

                // Option groups: as many meetings as the busiest subject; a subject
                // with fewer lessons sits out the later meetings.
                foreach (var grp in c.Subjects.Where(x => !x.Excluded && x.OptionGroupId != null).GroupBy(x => x.OptionGroupId))
                {
                    var meetings = grp.Max(x => x.LessonsPerWeek);
                    for (var k = 0; k < meetings; k++)
                        _units.Add(new Unit
                        {
                            Grid = g,
                            Members = grp.Where(x => x.LessonsPerWeek > k)
                                .Select(x => new Member { SubjectId = x.SubjectId, StaffId = x.StaffDetailsId }).ToList(),
                            Length = 1,
                            Placement = SubjectPlacement.Any,
                            MaxPerDay = 1,
                            Key = "G" + grp.Key,
                            Label = string.Join("/", grp.Select(x => x.SubjectAbbr))
                        });
                }
            }
            for (var i = 0; i < _units.Count; i++) _units[i].Id = i;
        }

        // ------------------------------------------------------------------ rules

        private (TimeOnly Start, TimeOnly End) Span(Unit u, int dayPos, int index) =>
            (u.Grid.Cells[dayPos, index].Bell.StartTime, u.Grid.Cells[dayPos, index + u.Length - 1].Bell.EndTime);

        /// <summary>Whether the card can go at (day, lesson): every rule that must hold.</summary>
        private bool CanPlace(Unit u, int dayPos, int index, Unit? ignore = null)
        {
            var g = u.Grid;
            if (index + u.Length > g.Lessons) return false;
            for (var k = 0; k < u.Length; k++)
            {
                var cell = g.Cells[dayPos, index + k];
                if (cell.Blocked || (cell.Unit != null && cell.Unit != ignore)) return false;
                if (k > 0 && !g.Cells[dayPos, index + k - 1].LinkedToNext) return false;
                if (u.Placement == SubjectPlacement.MorningOnly && cell.Afternoon) return false;
                if (u.Placement == SubjectPlacement.AfternoonOnly && !cell.Afternoon) return false;
            }

            var day = g.Days[dayPos];
            var sameDay = 0;
            for (var i = 0; i < g.Lessons; i++)
            {
                var o = g.Cells[dayPos, i].Unit;
                if (o != null && o != ignore && o.Key == u.Key && (i == 0 || g.Cells[dayPos, i - 1].Unit != o)) sameDay++;
            }
            if (sameDay >= u.MaxPerDay) return false;

            var (start, end) = Span(u, dayPos, index);
            foreach (var t in u.Teachers)
            {
                if (_input.Unavailable.TryGetValue(t, out var off) && off.Any(x => x.Day == day && x.StartTime < end && start < x.EndTime))
                    return false;
                if (_busy.TryGetValue(t, out var busy))
                {
                    if (busy.Any(b => b.Day == day && b.Unit != ignore && b.Start < end && start < b.End)) return false;
                    if (_input.TeacherRules.TryGetValue(t, out var rule) && rule.MaxLessonsPerDay is int max
                        && busy.Where(b => b.Day == day && b.Unit != ignore).Sum(b => b.Unit.Length) + u.Length > max)
                        return false;
                }
            }
            return true;
        }

        /// <summary>How good a free slot is: lower is better.</summary>
        private double Cost(Unit u, int dayPos, int index, bool jitter = true)
        {
            var g = u.Grid;
            double cost = 0;
            for (var k = 0; k < u.Length; k++)
            {
                var cell = g.Cells[dayPos, index + k];
                if (u.Placement == SubjectPlacement.PreferMorning && cell.Afternoon) cost += 3;
                if (u.Placement == SubjectPlacement.PreferAfternoon && !cell.Afternoon) cost += 3;
            }
            if (u.AvoidLast && g.Cells[dayPos, index + u.Length - 1].LastOfDay) cost += 4;
            // Earlier is a little better: a class with spare slots (Pre-Primary)
            // gets its lessons at the start of the day, not scattered with gaps.
            cost += index * 0.08;

            // Spread over the week: the same subject on the day before or after, or already today.
            for (var d = Math.Max(0, dayPos - 1); d <= Math.Min(g.Days.Count - 1, dayPos + 1); d++)
                for (var i = 0; i < g.Lessons; i++)
                    if (g.Cells[d, i].Unit?.Key == u.Key) cost += d == dayPos ? 4 : 1;

            // Teachers: too many in a row, and lessons bunched on one day.
            var day = g.Days[dayPos];
            var (start, end) = Span(u, dayPos, index);
            foreach (var t in u.Teachers)
            {
                if (!_busy.TryGetValue(t, out var busy)) continue;
                var today = busy.Where(b => b.Day == day).ToList();
                cost += today.Sum(b => b.Unit.Length) * 0.15;
                if (_input.TeacherRules.TryGetValue(t, out var rule) && rule.MaxConsecutiveLessons is int run
                    && RunLength(today, start, end) > run)
                    cost += 6;
            }
            // A little noise so repeated runs try different, equally good slots.
            return jitter ? cost + _rng.NextDouble() * 0.6 : cost;
        }

        /// <summary>Lessons in a row a teacher would teach around a new lesson (gaps over 15 minutes break the run).</summary>
        private static int RunLength(List<Busy> today, TimeOnly start, TimeOnly end)
        {
            var spans = today.Select(b => (b.Start, b.End, b.Unit.Length)).Append((start, end, 1)).OrderBy(x => x.Item1).ToList();
            int best = 0, current = 0;
            TimeOnly? lastEnd = null;
            foreach (var s in spans)
            {
                current = lastEnd != null && (s.Item1 - lastEnd.Value).TotalMinutes <= 15 ? current + s.Item3 : s.Item3;
                best = Math.Max(best, current);
                lastEnd = s.Item2;
            }
            return best;
        }

        private void Put(Unit u, int dayPos, int index)
        {
            u.DayPos = dayPos;
            u.Index = index;
            for (var k = 0; k < u.Length; k++) u.Grid.Cells[dayPos, index + k].Unit = u;
            var (start, end) = Span(u, dayPos, index);
            var day = u.Grid.Days[dayPos];
            foreach (var t in u.Teachers)
            {
                if (!_busy.TryGetValue(t, out var list)) _busy[t] = list = new List<Busy>();
                list.Add(new Busy(day, start, end, u));
            }
        }

        private void Take(Unit u)
        {
            if (!u.Placed) return;
            for (var k = 0; k < u.Length; k++) u.Grid.Cells[u.DayPos, u.Index + k].Unit = null;
            foreach (var t in u.Teachers)
                if (_busy.TryGetValue(t, out var list)) list.RemoveAll(b => b.Unit == u);
            u.DayPos = u.Index = -1;
        }

        private bool PlaceBest(Unit u, Unit? ignore = null)
        {
            var g = u.Grid;
            double best = double.MaxValue;
            (int d, int i) at = (-1, -1);
            for (var d = 0; d < g.Days.Count; d++)
                for (var i = 0; i < g.Lessons; i++)
                    if (CanPlace(u, d, i, ignore))
                    {
                        var c = Cost(u, d, i);
                        if (c < best) { best = c; at = (d, i); }
                    }
            if (at.d < 0) return false;
            Put(u, at.d, at.i);
            return true;
        }

        /// <summary>Units in the way of placing u at (day, lesson): in the class's cells or teaching u's teachers then.</summary>
        private HashSet<Unit> Blockers(Unit u, int dayPos, int index)
        {
            var set = new HashSet<Unit>();
            var g = u.Grid;
            if (index + u.Length > g.Lessons) return set;
            for (var k = 0; k < u.Length; k++)
            {
                var c = g.Cells[dayPos, index + k];
                if (c.Unit != null) set.Add(c.Unit);
            }
            var (start, end) = Span(u, dayPos, index);
            var day = g.Days[dayPos];
            foreach (var t in u.Teachers)
                if (_busy.TryGetValue(t, out var busy))
                    foreach (var b in busy.Where(b => b.Day == day && b.Start < end && start < b.End)) set.Add(b.Unit);
            return set;
        }

        /// <summary>Places u by moving one card that is in its way, if that card fits somewhere else.</summary>
        private bool Bump(Unit u)
        {
            var g = u.Grid;
            var spots = new List<(int d, int i)>();
            for (var d = 0; d < g.Days.Count; d++)
                for (var i = 0; i < g.Lessons; i++) spots.Add((d, i));
            foreach (var (d, i) in spots.OrderBy(_ => _rng.Next()))
            {
                var blockers = Blockers(u, d, i);
                if (blockers.Count != 1) continue;
                var b = blockers.First();
                if (b.Fixed || !CanPlace(u, d, i, b)) continue;
                int bd = b.DayPos, bi = b.Index;
                Take(b);
                Put(u, d, i);
                if (PlaceBest(b)) return true;
                Take(u);
                Put(b, bd, bi);
            }
            return false;
        }

        // ------------------------------------------------------------------ run

        public Result Run(IEnumerable<LockedLesson> locked, TimeSpan budget)
        {
            var watch = Stopwatch.StartNew();
            Build();
            var lockedList = locked.ToList();
            var total = _units.Count;

            // How hard each card is to place: doubles, option groups, strict
            // placements and busy teachers go first.
            var load = _units.SelectMany(u => u.Teachers.Select(t => (t, u.Length))).GroupBy(x => x.t).ToDictionary(x => x.Key, x => x.Sum(y => y.Length));
            foreach (var u in _units)
                u.Difficulty = (u.Length == 2 ? 40 : 0) + (u.Members.Count > 1 ? 30 : 0)
                    + (u.Placement is SubjectPlacement.MorningOnly or SubjectPlacement.AfternoonOnly ? 25 : 0)
                    + u.Teachers.Sum(t => load.GetValueOrDefault(t)) * 0.5;

            int[]? bestAt = null;
            int bestUnplaced = int.MaxValue;
            double bestScore = double.MaxValue;
            var attempts = 0;

            do
            {
                _rng = new Random(1000 + attempts);
                foreach (var u in _units) { u.DayPos = u.Index = -1; u.Fixed = false; }
                foreach (var g in _classes) foreach (var c in g.Cells) c.Unit = null;
                _busy.Clear();

                PlaceFixed(lockedList, attempts == 0);

                var order = _units.Where(u => !u.Placed)
                    .OrderByDescending(u => u.Difficulty + (attempts == 0 ? 0 : _rng.NextDouble() * 25)).ToList();
                foreach (var u in order)
                    if (!PlaceBest(u)) Bump(u);
                // Second chance for anything left, now the week has filled in.
                foreach (var u in _units.Where(u => !u.Placed).ToList())
                    if (!PlaceBest(u)) Bump(u);
                Polish();

                var unplaced = _units.Count(u => !u.Placed);
                var score = Score();
                if (unplaced < bestUnplaced || (unplaced == bestUnplaced && score < bestScore))
                {
                    bestUnplaced = unplaced;
                    bestScore = score;
                    bestAt = _units.SelectMany(u => new[] { u.DayPos, u.Index }).ToArray();
                }
                attempts++;
                // Stop early once everything is placed and a few tries have been compared.
            } while (watch.Elapsed < budget && !(bestUnplaced == 0 && attempts >= 6));

            // Put the best attempt back - from a clean slate, so nothing keeps a
            // place from the last attempt.
            foreach (var u in _units) { u.DayPos = u.Index = -1; u.Fixed = false; }
            foreach (var g in _classes) foreach (var c in g.Cells) c.Unit = null;
            _busy.Clear();
            PlaceFixed(lockedList, false);
            for (var k = 0; k < _units.Count; k++)
            {
                var u = _units[k];
                if (u.Fixed) continue;
                if (bestAt![k * 2] >= 0) Put(u, bestAt[k * 2], bestAt[k * 2 + 1]);
            }

            return Collect(total, attempts, watch.Elapsed.TotalSeconds, bestScore);
        }

        /// <summary>
        /// Lifts each card and drops it in its best free slot (its own slot is free
        /// again, so it never gets worse): mornings kept for morning subjects,
        /// subjects spread over the week.
        /// </summary>
        private void Polish()
        {
            for (var pass = 0; pass < 2; pass++)
                foreach (var u in _units.Where(u => u.Placed && !u.Fixed).ToList())
                {
                    int d = u.DayPos, i = u.Index;
                    Take(u);
                    var here = Cost(u, d, i, jitter: false);
                    double best = here;
                    (int d, int i) at = (d, i);
                    for (var dd = 0; dd < u.Grid.Days.Count; dd++)
                        for (var ii = 0; ii < u.Grid.Lessons; ii++)
                            if ((dd != d || ii != i) && CanPlace(u, dd, ii))
                            {
                                var c = Cost(u, dd, ii, jitter: false);
                                if (c < best - 0.01) { best = c; at = (dd, ii); }
                            }
                    Put(u, at.d, at.i);
                }
        }

        /// <summary>Locked cards and subjects given a fixed slot go in first, where they were put.</summary>
        private void PlaceFixed(List<LockedLesson> locked, bool warn)
        {
            foreach (var l in locked)
            {
                var g = _classes.FirstOrDefault(x => x.Class.SchoolClass.Id == l.SchoolClassId);
                var dayPos = g?.Days.IndexOf(l.Day) ?? -1;
                var index = g == null ? -1 : Enumerable.Range(0, g.Lessons).FirstOrDefault(i => g.Cells[0, i].Bell.Id == l.BellSlotId, -1);
                var u = g == null ? null : _units.FirstOrDefault(x => x.Grid == g && !x.Placed && x.Length == l.Length
                    && x.Members.Any(m => m.SubjectId == l.SubjectId));
                if (u == null || dayPos < 0 || index < 0 || !CanPlace(u, dayPos, index))
                {
                    if (warn) Warnings.Add($"A locked {(u?.Label ?? "lesson")} for {g?.Name ?? "a class"} could not stay where it was and was placed again.");
                    continue;
                }
                Put(u, dayPos, index);
                u.Fixed = true;
            }

            foreach (var g in _classes)
                foreach (var f in g.Class.Schedule!.FixedSlots.Where(f => f.SubjectId != null
                    && (f.LearningLevelId == null || f.LearningLevelId == g.Class.SchoolClass.LearningLevelId)))
                {
                    var dayPos = g.Days.IndexOf(f.Day);
                    var index = Enumerable.Range(0, g.Lessons).FirstOrDefault(i => g.Cells[0, i].Bell.Id == f.BellSlotId, -1);
                    var u = _units.Where(x => x.Grid == g && !x.Placed && x.Members.Any(m => m.SubjectId == f.SubjectId))
                        .OrderBy(x => x.Length).FirstOrDefault();
                    if (u == null) continue;   // the class does not take the subject
                    if (dayPos < 0 || index < 0 || !CanPlace(u, dayPos, index))
                    {
                        if (warn) Warnings.Add($"{g.Name}: the fixed {u.Label} slot could not be kept (the class or teacher is taken then).");
                        continue;
                    }
                    Put(u, dayPos, index);
                    u.Fixed = true;
                }
        }

        /// <summary>The week's total cost: each card scored where it sits, against the rest.</summary>
        private double Score()
        {
            double s = 0;
            foreach (var u in _units.Where(u => u.Placed).ToList())
            {
                int d = u.DayPos, i = u.Index;
                Take(u);
                s += Cost(u, d, i, jitter: false);
                Put(u, d, i);
            }
            return s;
        }

        // ------------------------------------------------------------------ result

        private Result Collect(int total, int attempts, double seconds, double score)
        {
            var result = new Result { Total = total, Attempts = attempts, Seconds = Math.Round(seconds, 1), Score = Math.Round(score, 1) };
            var link = 0;
            foreach (var u in _units)
            {
                int? linkNo = u.Members.Count > 1 ? ++link : null;
                foreach (var m in u.Members)
                    result.Lessons.Add(new Placement(u.Grid.Class.SchoolClass.Id, m.SubjectId, m.StaffId,
                        u.Placed ? u.Grid.Days[u.DayPos] : null,
                        u.Placed ? u.Grid.Cells[u.DayPos, u.Index].Bell.Id : null,
                        u.Length, linkNo, u.Fixed && u.Placed));
                if (u.Placed) result.Placed++;
                else result.Unplaced.Add(Explain(u));
            }
            return result;
        }

        /// <summary>Why a card found no slot: the class is full, its teacher is busy whenever the class is free, or rules leave nothing.</summary>
        private Unplaced Explain(Unit u)
        {
            var g = u.Grid;
            int classFree = 0, teacherFree = 0;
            for (var d = 0; d < g.Days.Count; d++)
                for (var i = 0; i + u.Length <= g.Lessons; i++)
                {
                    var ok = true;
                    for (var k = 0; k < u.Length && ok; k++)
                    {
                        var c = g.Cells[d, i + k];
                        ok = !c.Blocked && c.Unit == null && (k == 0 || g.Cells[d, i + k - 1].LinkedToNext);
                    }
                    if (!ok) continue;
                    classFree++;
                    if (Blockers(u, d, i).Count == 0) teacherFree++;
                }
            var teachers = string.Join(", ", u.Teachers.Select(t => _teacherNames.GetValueOrDefault(t, "the teacher")));
            string reason;
            if (classFree == 0)
                reason = u.Length == 2 ? $"{g.Name} has no two lessons in a row left free." : $"{g.Name} has no free lesson left.";
            else if (teacherFree == 0)
                reason = $"{(teachers == "" ? "The teacher" : teachers)} {(u.Teachers.Count() > 1 ? "are" : "is")} teaching elsewhere (or off) whenever {g.Name} is free.";
            else
                reason = "The subject's rules (time of day, most a day) leave no free slot.";
            return new Unplaced(g.Name, u.Label, teachers, u.Length, reason);
        }
    }
}
