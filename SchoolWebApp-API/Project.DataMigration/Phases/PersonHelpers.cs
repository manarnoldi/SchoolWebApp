using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.Entities.Settings;

namespace Project.DataMigration.Phases;

// Shared helpers used by StaffDetailsPhase and StudentPhase.
public static class PersonHelpers
{
    // Loads existing religions into a case-insensitive name -> Id map.
    public static async Task<Dictionary<string, int>> LoadReligionMapAsync(ApplicationDbContext db, CancellationToken ct)
    {
        var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var r in await db.Religions.AsNoTracking().ToListAsync(ct))
            map[r.Name] = r.Id;
        return map;
    }

    // Resolves a religion name to its Id, auto-creating a Religion row when missing.
    // Empty/whitespace input falls back to the default Religion (typically Christian, Id=1).
    public static async Task<int> ResolveReligionAsync(
        ApplicationDbContext db,
        Dictionary<string, int> map,
        string? religionText,
        int defaultReligionId,
        CancellationToken ct)
    {
        var name = religionText?.Trim();
        if (string.IsNullOrEmpty(name)) return defaultReligionId;
        if (map.TryGetValue(name, out var id)) return id;

        var entity = new Religion
        {
            Name = name,
            Description = "Auto-created by data migration",
            Rank = map.Count + 1
        };
        db.Religions.Add(entity);
        await db.SaveChangesAsync(ct);
        map[name] = entity.Id;
        return entity.Id;
    }

    // Maps the source 'sex' bit to a GenderId.
    //   true  -> Male  (Id=1, EF-seeded)
    //   false -> Female (Id ensured by PersonReferenceSeedPhase)
    //   null  -> Male  (caller logs a warning)
    public static int GenderIdFromBit(bool? sex, int maleId, int femaleId)
        => sex switch { true => maleId, false => femaleId, null => maleId };

    // Builds a full name from First/Middle/Last when the source FullName column is empty.
    public static string ComposeFullName(string? fullName, string? f, string? m, string? l)
    {
        if (!string.IsNullOrWhiteSpace(fullName)) return fullName.Trim();
        var parts = new[] { f, m, l }.Where(p => !string.IsNullOrWhiteSpace(p)).Select(p => p!.Trim());
        return string.Join(' ', parts);
    }

    // Truncates a string to a maximum length so we don't overflow VARCHAR limits.
    public static string? Cap(string? s, int max)
    {
        if (string.IsNullOrEmpty(s)) return s;
        return s.Length <= max ? s : s[..max];
    }
}
