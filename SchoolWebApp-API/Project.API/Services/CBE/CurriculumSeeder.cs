using System.Text.Json;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;

namespace SchoolWebApp.API.Services.CBE
{
    /// <summary>
    /// Loads the KICD curriculum designs shipped with the API (SeedData/Curriculum,
    /// listed in manifest.json) at startup, through the same importer as Curriculum
    /// Upload. A design is loaded only when its subject and grade have none yet, so
    /// each runs once and an upload made in the app is never overwritten. Subjects
    /// and grades are found by name, as their ids differ between databases.
    /// </summary>
    public static class CurriculumSeeder
    {
        private class Entry
        {
            public string File { get; set; } = "";
            public string Subject { get; set; } = "";
            public string Level { get; set; } = "";
        }

        public static async Task ApplyAsync(IServiceProvider services, ILogger logger)
        {
            var dir = Path.Combine(AppContext.BaseDirectory, "SeedData", "Curriculum");
            var manifestPath = Path.Combine(dir, "manifest.json");
            if (!File.Exists(manifestPath)) return;
            var entries = JsonSerializer.Deserialize<List<Entry>>(await File.ReadAllTextAsync(manifestPath),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

            var scopes = services.GetRequiredService<IServiceScopeFactory>();
            foreach (var e in entries)
            {
                // A scope per design keeps each import's tracked entities apart.
                using var scope = scopes.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var subject = await db.Subjects.AsNoTracking().FirstOrDefaultAsync(s => s.Name == e.Subject);
                var level = await db.LearningLevels.AsNoTracking().FirstOrDefaultAsync(l => l.Name == e.Level);
                if (subject == null || level == null)
                {
                    logger.LogWarning("Curriculum seed {File}: no subject '{Subject}' or grade '{Level}' here - skipped.", e.File, e.Subject, e.Level);
                    continue;
                }
                if (await db.CurriculumDesigns.AnyAsync(d => d.SubjectId == subject.Id && d.LearningLevelId == level.Id)) continue;

                var path = Path.Combine(dir, e.File);
                if (!File.Exists(path))
                {
                    logger.LogWarning("Curriculum seed {File}: file missing - skipped.", e.File);
                    continue;
                }
                try
                {
                    using var content = new MemoryStream();
                    using (var wb = new XLWorkbook(path))
                    {
                        // Point the workbook at this database's subject and grade.
                        var ws = wb.Worksheets.First(w => w.Cell(CurriculumWorkbookService.KeyRow, 1).GetString() == CurriculumWorkbookService.Marker);
                        ws.Cell(CurriculumWorkbookService.KeyRow, 2).Value = subject.Id;
                        ws.Cell(CurriculumWorkbookService.KeyRow, 3).Value = level.Id;
                        wb.SaveAs(content);
                    }
                    content.Position = 0;
                    var result = await scope.ServiceProvider.GetRequiredService<CurriculumWorkbookService>().ImportAsync(content, commit: true);
                    if (result.Saved)
                        logger.LogInformation("Curriculum seed: loaded {Subject} / {Level}.", e.Subject, e.Level);
                    else
                        logger.LogWarning("Curriculum seed {File} not loaded: {Errors}", e.File,
                            string.Join("; ", result.Errors.Take(5).Select(x => $"{x.Sheet} {x.Row}: {x.Message}")));
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Curriculum seed {File} failed.", e.File);
                }
            }
        }
    }
}
