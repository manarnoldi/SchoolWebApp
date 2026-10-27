using System.Text.RegularExpressions;

namespace SchoolWebApp.API.Services.Elections
{
    /// <summary>
    /// Candidate photos as files on disk, in Elections:PhotoFolder or by default
    /// uploads/elections under the app folder (outside wwwroot, so a deploy never
    /// replaces it). Files get random names; the database keeps only the name.
    /// </summary>
    public partial class ElectionPhotoStore
    {
        public const long MaxBytes = 3 * 1024 * 1024;
        private readonly string _folder;

        public ElectionPhotoStore(IConfiguration config, IWebHostEnvironment env)
        {
            var configured = config["Elections:PhotoFolder"];
            _folder = string.IsNullOrWhiteSpace(configured)
                ? Path.Combine(env.ContentRootPath, "uploads", "elections")
                : configured;
        }

        [GeneratedRegex("^[0-9a-f]{32}\\.(jpg|png|webp)$")]
        private static partial Regex FileNamePattern();

        /// <summary>Saves an upload; returns the file name, or throws with a message for the user.</summary>
        public async Task<string> SaveAsync(IFormFile file)
        {
            if (file == null || file.Length == 0) throw new InvalidDataException("Choose a photo to upload.");
            if (file.Length > MaxBytes) throw new InvalidDataException("The photo is larger than 3 MB.");
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            return await SaveBytesAsync(ms.ToArray());
        }

        /// <summary>Copies a photo held on a student's record (a base64 data URL); null when there is none usable.</summary>
        public async Task<string?> SaveDataUrlAsync(string? dataUrl)
        {
            if (string.IsNullOrWhiteSpace(dataUrl)) return null;
            var comma = dataUrl.IndexOf(',');
            try
            {
                var bytes = Convert.FromBase64String(comma >= 0 ? dataUrl[(comma + 1)..] : dataUrl);
                if (bytes.Length == 0 || bytes.Length > MaxBytes) return null;
                return await SaveBytesAsync(bytes);
            }
            catch (FormatException) { return null; }
            catch (InvalidDataException) { return null; }
        }

        private async Task<string> SaveBytesAsync(byte[] bytes)
        {
            var ext = Extension(bytes) ?? throw new InvalidDataException("The photo must be a JPEG, PNG or WebP image.");
            Directory.CreateDirectory(_folder);
            var name = $"{Guid.NewGuid():N}.{ext}";
            await File.WriteAllBytesAsync(Path.Combine(_folder, name), bytes);
            return name;
        }

        // Decided by the file's first bytes, not its name or the browser's content type.
        private static string? Extension(byte[] b)
        {
            if (b.Length > 3 && b[0] == 0xFF && b[1] == 0xD8 && b[2] == 0xFF) return "jpg";
            if (b.Length > 8 && b[0] == 0x89 && b[1] == 0x50 && b[2] == 0x4E && b[3] == 0x47) return "png";
            if (b.Length > 12 && b[0] == 'R' && b[1] == 'I' && b[2] == 'F' && b[3] == 'F'
                && b[8] == 'W' && b[9] == 'E' && b[10] == 'B' && b[11] == 'P') return "webp";
            return null;
        }

        public (string Path, string ContentType)? Find(string name)
        {
            if (!FileNamePattern().IsMatch(name)) return null;
            var path = Path.Combine(_folder, name);
            if (!File.Exists(path)) return null;
            var type = name.EndsWith(".png") ? "image/png" : name.EndsWith(".webp") ? "image/webp" : "image/jpeg";
            return (path, type);
        }

        public void Delete(string? name)
        {
            if (string.IsNullOrEmpty(name) || !FileNamePattern().IsMatch(name)) return;
            var path = Path.Combine(_folder, name);
            try { if (File.Exists(path)) File.Delete(path); } catch (IOException) { }
        }
    }
}
