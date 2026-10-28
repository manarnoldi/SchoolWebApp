using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.Entities.Settings;

namespace SchoolWebApp.API.Licensing
{
    /// <summary>
    /// What a licence key says, as Kodetek signed it. Short JSON names keep the key
    /// small enough for a settings row.
    /// </summary>
    public class LicencePayload
    {
        [JsonProperty("l")] public string LicenceId { get; set; } = "";
        [JsonProperty("s")] public string School { get; set; } = "";
        /// <summary>The sites the licence works on (e.g. swikunda.kodetek.co.ke); empty = any.</summary>
        [JsonProperty("h")] public List<string> Hosts { get; set; } = new();
        [JsonProperty("m")] public List<string> Modules { get; set; } = new();
        [JsonProperty("i")] public DateTime Issued { get; set; }
        /// <summary>The last day the licence is valid.</summary>
        [JsonProperty("e")] public DateTime Expires { get; set; }
    }

    public class LicenceStatus
    {
        /// <summary>None, Invalid, WrongSite, Active, Expiring, Grace or Expired.</summary>
        public string State { get; set; } = "None";
        public string? Message { get; set; }
        public string? LicenceId { get; set; }
        public string? School { get; set; }
        public List<string> Hosts { get; set; } = new();
        /// <summary>The modules the licence lists.</summary>
        public List<string> LicensedModules { get; set; } = new();
        /// <summary>The modules usable now: the licensed ones while it is valid (or in grace), else none.</summary>
        public List<string> Modules { get; set; } = new();
        public DateTime? Issued { get; set; }
        public DateTime? Expires { get; set; }
        public DateTime? GraceEnds { get; set; }
        public int? DaysLeft { get; set; }
    }

    /// <summary>
    /// The school's licence: a key signed by Kodetek (ECDSA P-256) listing the
    /// modules bought, the site and the expiry date, kept in GlobalSettings
    /// Licence/Key. Only Kodetek holds the private key, so a key cannot be made or
    /// edited here - only checked. Core features never depend on it.
    /// </summary>
    public class LicenceService
    {
        public const string SettingsModule = "Licence";
        public const string SettingsKey = "Key";
        private const string Prefix = "SNL1";
        public const int GraceDays = 14;
        public const int WarnDays = 30;

        // Kodetek's licence-signing public key (the private key is never in this repo).
        private const string PublicKey =
            "MFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEQGuq+WolBV3/1HCLlzcDzcYPKl7sCW3MMPY+K87ks9MoNcRRHDhKqAFjBRDetX/K57Plto1SCCTu6GgjBdGsLA==";

        private readonly IServiceScopeFactory _scopes;
        private readonly object _gate = new();
        private (string? Key, DateTime LoadedAt)? _cache;

        public LicenceService(IServiceScopeFactory scopes)
        {
            _scopes = scopes;
        }

        /// <summary>The licence as it applies to a request for this host, today.</summary>
        public async Task<LicenceStatus> StatusAsync(string? host)
        {
            return Evaluate(await StoredKeyAsync(), host, DateTime.UtcNow.Date);
        }

        public async Task<bool> HasModuleAsync(string module, string? host) =>
            (await StatusAsync(host)).Modules.Contains(module);

        /// <summary>Checks a key without saving it.</summary>
        public LicenceStatus Check(string key, string? host) => Evaluate(key, host, DateTime.UtcNow.Date);

        public async Task SaveAsync(string key)
        {
            using var scope = _scopes.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var row = await db.GlobalSettings.FirstOrDefaultAsync(s => s.Module == SettingsModule && s.SettingKey == SettingsKey);
            if (row == null)
            {
                db.GlobalSettings.Add(new GlobalSetting
                {
                    Module = SettingsModule, SettingKey = SettingsKey, SettingValue = key.Trim(),
                    Description = "The school's ShuleNova licence key (Settings > Licence)."
                });
            }
            else row.SettingValue = key.Trim();
            await db.SaveChangesAsync();
            lock (_gate) _cache = null;
        }

        // The stored key, re-read at most once a minute.
        private async Task<string?> StoredKeyAsync()
        {
            lock (_gate)
            {
                if (_cache is { } c && DateTime.UtcNow - c.LoadedAt < TimeSpan.FromMinutes(1)) return c.Key;
            }
            using var scope = _scopes.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var key = await db.GlobalSettings.AsNoTracking()
                .Where(s => s.Module == SettingsModule && s.SettingKey == SettingsKey)
                .Select(s => s.SettingValue).FirstOrDefaultAsync();
            lock (_gate) _cache = (key, DateTime.UtcNow);
            return key;
        }

        private static LicenceStatus Evaluate(string? key, string? host, DateTime today)
        {
            if (string.IsNullOrWhiteSpace(key))
                return new LicenceStatus { State = "None", Message = "No licence is installed. Only the core features are available." };

            var payload = Verify(key.Trim());
            if (payload == null)
                return new LicenceStatus { State = "Invalid", Message = "The licence key is not valid. Check it was copied in full." };

            var status = new LicenceStatus
            {
                LicenceId = payload.LicenceId,
                School = payload.School,
                Hosts = payload.Hosts,
                LicensedModules = payload.Modules.Where(m => AppModules.All.Contains(m)).ToList(),
                Issued = payload.Issued.Date,
                Expires = payload.Expires.Date,
                GraceEnds = payload.Expires.Date.AddDays(GraceDays)
            };

            if (!HostAllowed(payload.Hosts, host))
            {
                status.State = "WrongSite";
                status.Message = $"This licence is for {string.Join(", ", payload.Hosts)}, not {host}.";
                return status;
            }

            var daysLeft = (payload.Expires.Date - today).Days;
            status.DaysLeft = daysLeft;
            if (daysLeft >= 0)
            {
                status.Modules = status.LicensedModules;
                status.State = daysLeft < WarnDays ? "Expiring" : "Active";
                if (daysLeft < WarnDays) status.Message = $"The licence expires on {payload.Expires:d MMM yyyy}. Contact Kodetek to renew it.";
            }
            else if (today <= status.GraceEnds)
            {
                status.Modules = status.LicensedModules;
                status.State = "Grace";
                status.Message = $"The licence expired on {payload.Expires:d MMM yyyy}. The modules stop working after {status.GraceEnds:d MMM yyyy} unless it is renewed.";
            }
            else
            {
                status.State = "Expired";
                status.Message = $"The licence expired on {payload.Expires:d MMM yyyy}. Only the core features are available until it is renewed.";
            }
            return status;
        }

        // A key is "SNL1.<payload>.<signature>", both base64url; the signature covers "SNL1.<payload>".
        private static LicencePayload? Verify(string key)
        {
            var parts = key.Split('.');
            if (parts.Length != 3 || parts[0] != Prefix) return null;
            try
            {
                using var ecdsa = ECDsa.Create();
                ecdsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(PublicKey), out _);
                var signed = Encoding.ASCII.GetBytes(parts[0] + "." + parts[1]);
                if (!ecdsa.VerifyData(signed, FromBase64Url(parts[2]), HashAlgorithmName.SHA256)) return null;
                return JsonConvert.DeserializeObject<LicencePayload>(Encoding.UTF8.GetString(FromBase64Url(parts[1])));
            }
            catch (Exception ex) when (ex is FormatException or CryptographicException or JsonException)
            {
                return null;
            }
        }

        // Development on this machine works with any school's licence.
        private static bool HostAllowed(List<string> hosts, string? host)
        {
            if (hosts.Count == 0 || string.IsNullOrEmpty(host)) return true;
            var h = host.ToLowerInvariant();
            if (h is "localhost" or "127.0.0.1" or "::1") return true;
            return hosts.Any(x => string.Equals(x, h, StringComparison.OrdinalIgnoreCase));
        }

        private static byte[] FromBase64Url(string s)
        {
            var b = s.Replace('-', '+').Replace('_', '/');
            return Convert.FromBase64String(b.PadRight(b.Length + (4 - b.Length % 4) % 4, '='));
        }
    }
}
