using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.DTOs.Communications;
using SchoolWebApp.Core.Entities.Communications;
using SchoolWebApp.Core.Entities.Enums;

namespace SchoolWebApp.API.Services.Communications
{
    /// <summary>
    /// One person a message can go to, with normalised contacts (either may be
    /// null) and the placeholder values that belong to them.
    /// </summary>
    public class ContactTarget
    {
        public RecipientType Type { get; set; }
        public int? PersonId { get; set; }
        public int? StudentId { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public Dictionary<string, string?> Values { get; set; } = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Values that read differently in an email - a results list one subject
        /// per line instead of the SMS's compact "MAT 78, ENG 65".
        /// </summary>
        public Dictionary<string, string?> EmailValues { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    }

    public class StudentInfo
    {
        public int Id { get; set; }
        public required string FullName { get; set; }
        public string? AdmissionNo { get; set; }
        public string? ClassName { get; set; }
    }

    /// <summary>
    /// Turns "parents of Grade 4", "all teaching staff", "these learners" into
    /// contacts, honouring the school's parent-contact source.
    /// </summary>
    public class RecipientResolver
    {
        public const string ContactSourceModule = "Communications";
        public const string ContactSourceKey = "ParentContactSource";

        private readonly ApplicationDbContext _db;

        public RecipientResolver(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<ParentContactSource> GetParentContactSourceAsync(CancellationToken ct = default)
        {
            var value = await _db.GlobalSettings.AsNoTracking()
                .Where(g => g.Module == ContactSourceModule && g.SettingKey == ContactSourceKey)
                .Select(g => g.SettingValue)
                .FirstOrDefaultAsync(ct);
            return Enum.TryParse<ParentContactSource>(value, true, out var source)
                ? source
                : ParentContactSource.ParentThenStudent;
        }

        /// <summary>The academic year marked current, else the latest one.</summary>
        public async Task<int?> CurrentAcademicYearIdAsync(CancellationToken ct = default)
        {
            return await _db.AcademicYears.AsNoTracking()
                .OrderByDescending(y => y.Status).ThenByDescending(y => y.StartDate)
                .Select(y => (int?)y.Id)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<List<ContactTarget>> ResolveAsync(RecipientCriteriaDto c, CancellationToken ct = default)
        {
            switch (c.Group)
            {
                case RecipientGroup.AllStaff:
                case RecipientGroup.StaffByCategory:
                case RecipientGroup.SelectedStaff:
                    return await StaffContactsAsync(c, ct);
                case RecipientGroup.CustomContacts:
                    return CustomContacts(c.CustomContacts);
                default:
                    var studentIds = await StudentIdsAsync(c, ct);
                    var targets = await ParentContactsAsync(studentIds, ct);
                    return targets.SelectMany(t => t.Value).ToList();
            }
        }

        public async Task<string> DescribeAsync(RecipientCriteriaDto c, CancellationToken ct = default)
        {
            switch (c.Group)
            {
                case RecipientGroup.AllParents: return "All parents";
                case RecipientGroup.ParentsOfClasses:
                    var classes = await ClassLabelsAsync(c.SchoolClassIds, ct);
                    return "Parents of " + string.Join(", ", classes.Values);
                case RecipientGroup.ParentsOfEducationLevels:
                    var levels = await _db.EducationLevels.AsNoTracking()
                        .Where(l => c.EducationLevelIds.Contains(l.Id)).Select(l => l.Name).ToListAsync(ct);
                    return "Parents in " + string.Join(", ", levels);
                case RecipientGroup.ParentsOfStudents: return $"Parents of {c.StudentIds.Count} selected learner(s)";
                case RecipientGroup.AllStaff: return "All staff";
                case RecipientGroup.StaffByCategory:
                    var cats = await _db.StaffCategories.AsNoTracking()
                        .Where(s => c.StaffCategoryIds.Contains(s.Id)).Select(s => s.Name).ToListAsync(ct);
                    return string.Join(", ", cats) + " staff";
                case RecipientGroup.SelectedStaff: return $"{c.StaffIds.Count} selected staff member(s)";
                case RecipientGroup.CustomContacts: return "Custom numbers / emails";
                default: return c.Group.ToString();
            }
        }

        private async Task<List<int>> StudentIdsAsync(RecipientCriteriaDto c, CancellationToken ct)
        {
            if (c.Group == RecipientGroup.ParentsOfStudents)
                return c.StudentIds.Distinct().ToList();

            var active = _db.Students.AsNoTracking().Where(s => s.Status == Status.Active);
            if (c.Group == RecipientGroup.AllParents)
                return await active.Select(s => s.Id).ToListAsync(ct);

            var yearId = await CurrentAcademicYearIdAsync(ct);
            var enrolments = _db.StudentClasses.AsNoTracking()
                .Where(sc => sc.Student!.Status == Status.Active);
            enrolments = c.Group == RecipientGroup.ParentsOfClasses
                ? enrolments.Where(sc => c.SchoolClassIds.Contains(sc.SchoolClassId))
                : enrolments.Where(sc => sc.SchoolClass!.AcademicYearId == yearId
                                         && c.EducationLevelIds.Contains(sc.SchoolClass.LearningLevel!.EducationLevelId));
            return await enrolments.Select(sc => sc.StudentId).Distinct().ToListAsync(ct);
        }

        /// <summary>
        /// Name, admission number and current class (e.g. "Grade 4 E") of each learner.
        /// </summary>
        public async Task<Dictionary<int, StudentInfo>> StudentInfoAsync(IReadOnlyCollection<int> studentIds, CancellationToken ct = default)
        {
            var yearId = await CurrentAcademicYearIdAsync(ct);
            var students = await _db.Students.AsNoTracking()
                .Where(s => studentIds.Contains(s.Id))
                .Select(s => new StudentInfo { Id = s.Id, FullName = s.FullName, AdmissionNo = s.UPI })
                .ToListAsync(ct);
            var classes = await _db.StudentClasses.AsNoTracking()
                .Where(sc => studentIds.Contains(sc.StudentId))
                .Select(sc => new
                {
                    sc.StudentId,
                    sc.SchoolClass!.AcademicYearId,
                    StartDate = sc.SchoolClass.AcademicYear!.StartDate,
                    Level = sc.SchoolClass.LearningLevel!.Name,
                    Stream = sc.SchoolClass.SchoolStream!.Name
                })
                .ToListAsync(ct);
            foreach (var s in students)
            {
                // The current year's class; failing that, the most recent one.
                var cls = classes.Where(x => x.StudentId == s.Id)
                    .OrderByDescending(x => x.AcademicYearId == yearId).ThenByDescending(x => x.StartDate)
                    .FirstOrDefault();
                if (cls != null) s.ClassName = $"{cls.Level} {cls.Stream}".Trim();
            }
            return students.ToDictionary(s => s.Id);
        }

        /// <summary>
        /// The parent contacts of each learner, per the school's contact source.
        /// A learner with nobody reachable still gets one target with no phone or
        /// email, so callers can count and report them.
        /// </summary>
        public async Task<Dictionary<int, List<ContactTarget>>> ParentContactsAsync(IReadOnlyCollection<int> studentIds, CancellationToken ct = default)
        {
            var source = await GetParentContactSourceAsync(ct);
            var info = await StudentInfoAsync(studentIds, ct);
            var students = await _db.Students.AsNoTracking()
                .Where(s => studentIds.Contains(s.Id))
                .Select(s => new
                {
                    s.Id,
                    s.PhoneNumber,
                    s.Email,
                    Parents = s.StudentParents
                        .Where(sp => sp.Parent!.Status == Status.Active)
                        .Select(sp => new { sp.ParentId, sp.Parent!.FullName, sp.Parent.PhoneNumber, sp.Parent.Email, sp.Parent.Notifiable })
                        .ToList()
                })
                .ToListAsync(ct);

            var result = new Dictionary<int, List<ContactTarget>>();
            foreach (var s in students)
            {
                var student = info.GetValueOrDefault(s.Id);
                // Parents flagged "notify about student progress" when any are; all linked parents otherwise.
                var parents = s.Parents.Any(p => p.Notifiable) ? s.Parents.Where(p => p.Notifiable).ToList() : s.Parents;
                var parentPhones = source != ParentContactSource.StudentRecord;
                var parentEmails = source != ParentContactSource.StudentRecord;
                var studentPhone = source == ParentContactSource.StudentRecord ||
                    (source == ParentContactSource.ParentThenStudent && !parents.Any(p => MessageText.NormalizePhone(p.PhoneNumber) != null));
                var studentEmail = source == ParentContactSource.StudentRecord ||
                    (source == ParentContactSource.ParentThenStudent && !parents.Any(p => MessageText.NormalizeEmail(p.Email) != null));

                var targets = new List<ContactTarget>();
                if (parentPhones || parentEmails)
                {
                    foreach (var p in parents)
                    {
                        var t = new ContactTarget
                        {
                            Type = RecipientType.Parent,
                            PersonId = p.ParentId,
                            StudentId = s.Id,
                            Name = p.FullName,
                            Phone = parentPhones ? MessageText.NormalizePhone(p.PhoneNumber) : null,
                            Email = parentEmails ? MessageText.NormalizeEmail(p.Email) : null
                        };
                        if (t.Phone != null || t.Email != null) targets.Add(t);
                    }
                }
                if (studentPhone || studentEmail)
                {
                    var t = new ContactTarget
                    {
                        Type = RecipientType.StudentContact,
                        PersonId = s.Id,
                        StudentId = s.Id,
                        Name = "Parent/Guardian",
                        Phone = studentPhone ? MessageText.NormalizePhone(s.PhoneNumber) : null,
                        Email = studentEmail ? MessageText.NormalizeEmail(s.Email) : null
                    };
                    if (t.Phone != null || t.Email != null) targets.Add(t);
                }
                if (targets.Count == 0)
                    targets.Add(new ContactTarget { Type = RecipientType.StudentContact, StudentId = s.Id, Name = "Parent/Guardian" });

                foreach (var t in targets)
                {
                    t.Values["ParentName"] = t.Name;
                    t.Values["RecipientName"] = t.Name;
                    t.Values["StudentName"] = student?.FullName;
                    t.Values["AdmissionNo"] = student?.AdmissionNo;
                    t.Values["ClassName"] = student?.ClassName;
                }
                result[s.Id] = targets;
            }
            return result;
        }

        private async Task<List<ContactTarget>> StaffContactsAsync(RecipientCriteriaDto c, CancellationToken ct)
        {
            var query = _db.StaffDetails.AsNoTracking();
            query = c.Group switch
            {
                RecipientGroup.SelectedStaff => query.Where(s => c.StaffIds.Contains(s.Id)),
                RecipientGroup.StaffByCategory => query.Where(s => s.Status == Status.Active && s.CurrentlyEmployed
                                                                   && c.StaffCategoryIds.Contains(s.StaffCategoryId)),
                _ => query.Where(s => s.Status == Status.Active && s.CurrentlyEmployed)
            };
            var staff = await query.Select(s => new { s.Id, s.FullName, s.PhoneNumber, s.Email }).ToListAsync(ct);
            return staff.Select(s =>
            {
                var t = new ContactTarget
                {
                    Type = RecipientType.Staff,
                    PersonId = s.Id,
                    Name = s.FullName,
                    Phone = MessageText.NormalizePhone(s.PhoneNumber),
                    Email = MessageText.NormalizeEmail(s.Email)
                };
                t.Values["RecipientName"] = s.FullName;
                t.Values["StaffName"] = s.FullName;
                return t;
            }).ToList();
        }

        private static List<ContactTarget> CustomContacts(string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return new();
            return text.Split(new[] { '\n', '\r', ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(token => token.Contains('@')
                    ? new ContactTarget { Type = RecipientType.Other, Name = token, Email = MessageText.NormalizeEmail(token) }
                    : new ContactTarget { Type = RecipientType.Other, Name = token, Phone = MessageText.NormalizePhone(token) })
                .ToList();
        }

        public async Task<Dictionary<int, string>> ClassLabelsAsync(IEnumerable<int> classIds, CancellationToken ct = default)
        {
            var ids = classIds.ToList();
            return await _db.SchoolClasses.AsNoTracking()
                .Where(sc => ids.Contains(sc.Id))
                .Select(sc => new { sc.Id, Label = sc.LearningLevel!.Name + " " + sc.SchoolStream!.Name })
                .ToDictionaryAsync(sc => sc.Id, sc => sc.Label, ct);
        }
    }
}
