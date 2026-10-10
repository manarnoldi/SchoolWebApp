using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SchoolWebApp.Core.Entities.Communications;
using System.Text;

namespace SchoolWebApp.API.Services.Communications
{
    public record SmsSendItem(string ClientId, string Mobile, string Message);

    public record SmsSendResult(string ClientId, bool Success, bool Retryable, string? MessageId, string? Description);

    public record SmsDeliveryResult(MessageStatus? Status, string? ProviderStatus);

    /// <summary>
    /// TextSMS (sms.textsms.co.ke) bulk SMS API. Every call is a JSON POST carrying
    /// the account's apikey and partnerID; the sender ID goes out as "shortcode".
    /// </summary>
    public class TextSmsGateway
    {
        public const string DefaultApiUrl = "https://sms.textsms.co.ke/api/services/";

        // The bulk endpoint takes at most 20 messages per call.
        public const int BulkLimit = 20;

        // Gateway codes worth trying again later: low credit (the school may top
        // up), system / internal errors. Anything else - invalid number, sender ID,
        // credentials - will fail the same way however often it is retried.
        private static readonly HashSet<string> RetryableCodes = new() { "1004", "1005", "1007", "4090" };

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<TextSmsGateway> _logger;

        public TextSmsGateway(IHttpClientFactory httpClientFactory, ILogger<TextSmsGateway> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public static string? ConfigurationProblem(CommunicationSetting s)
        {
            if (string.IsNullOrWhiteSpace(s.SmsApiKey)) return "The SMS API key is not set.";
            if (string.IsNullOrWhiteSpace(s.SmsPartnerId)) return "The SMS partner ID is not set.";
            if (string.IsNullOrWhiteSpace(s.SmsSenderId)) return "The SMS sender ID is not set.";
            return null;
        }

        public async Task<List<SmsSendResult>> SendBulkAsync(CommunicationSetting s, IReadOnlyList<SmsSendItem> items, CancellationToken ct)
        {
            var body = new JObject
            {
                ["count"] = items.Count,
                ["smslist"] = new JArray(items.Select(i => new JObject
                {
                    ["partnerID"] = s.SmsPartnerId,
                    ["apikey"] = s.SmsApiKey,
                    ["pass_type"] = "plain",
                    ["clientsmsid"] = i.ClientId,
                    ["mobile"] = i.Mobile,
                    ["message"] = i.Message,
                    ["shortcode"] = s.SmsSenderId
                }))
            };

            JToken? response;
            try
            {
                response = await PostAsync(s, "sendbulk/", body, ct);
            }
            catch (Exception ex) when (ex is not OperationCanceledException || !ct.IsCancellationRequested)
            {
                _logger.LogWarning(ex, "TextSMS bulk send failed");
                return items.Select(i => new SmsSendResult(i.ClientId, false, true, null, "Gateway unreachable: " + ex.Message)).ToList();
            }

            var responses = response?["responses"] as JArray;
            var results = new List<SmsSendResult>();
            foreach (var item in items)
            {
                var r = responses?.FirstOrDefault(x => (string?)x["clientsmsid"] == item.ClientId);
                // A request-level failure (bad credentials) can come back as a
                // single response without clientsmsid; it applies to every item.
                r ??= responses?.Count == 1 && responses[0]["clientsmsid"] == null ? responses[0] : null;
                results.Add(r == null
                    ? new SmsSendResult(item.ClientId, false, true, null, "No response from the gateway for this message.")
                    : ToResult(item.ClientId, r));
            }
            return results;
        }

        public async Task<SmsSendResult> SendSingleAsync(CommunicationSetting s, string mobile, string message, CancellationToken ct)
        {
            var body = new JObject
            {
                ["apikey"] = s.SmsApiKey,
                ["partnerID"] = s.SmsPartnerId,
                ["message"] = message,
                ["shortcode"] = s.SmsSenderId,
                ["mobile"] = mobile
            };
            try
            {
                var response = await PostAsync(s, "sendsms/", body, ct);
                var r = (response?["responses"] as JArray)?.FirstOrDefault();
                return r == null
                    ? new SmsSendResult("", false, true, null, "Unexpected gateway response: " + response?.ToString(Formatting.None))
                    : ToResult("", r);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "TextSMS send failed");
                return new SmsSendResult("", false, true, null, "Gateway unreachable: " + ex.Message);
            }
        }

        /// <summary>
        /// Delivery report for one message. The API documents the request but not
        /// the response body, so the status is read from whichever field carries it
        /// and the raw text is kept for the queue view.
        /// </summary>
        public async Task<SmsDeliveryResult> GetDeliveryAsync(CommunicationSetting s, string messageId, CancellationToken ct)
        {
            var body = new JObject
            {
                ["apikey"] = s.SmsApiKey,
                ["partnerID"] = s.SmsPartnerId,
                ["messageID"] = messageId
            };
            var response = await PostAsync(s, "getdlr/", body, ct);
            if (response == null) return new SmsDeliveryResult(null, null);

            string? status = null;
            foreach (var p in response.SelectTokens("..*").OfType<JValue>())
            {
                var name = (p.Parent as JProperty)?.Name?.ToLowerInvariant() ?? "";
                if (name.Contains("status") || name.Contains("dlr") || name.Contains("delivery"))
                {
                    status = p.ToString();
                    if (!string.IsNullOrWhiteSpace(status)) break;
                }
            }
            status ??= response.ToString(Formatting.None);
            if (status.Length > 250) status = status[..250];

            var s2 = status.ToLowerInvariant();
            MessageStatus? mapped =
                s2.Contains("undeliv") || s2.Contains("fail") || s2.Contains("reject") || s2.Contains("expired") || s2.Contains("blacklist")
                    ? MessageStatus.Undelivered
                : s2.Contains("deliver") ? MessageStatus.Delivered
                : null;
            return new SmsDeliveryResult(mapped, status);
        }

        public async Task<string> GetBalanceAsync(CommunicationSetting s, CancellationToken ct)
        {
            var body = new JObject { ["apikey"] = s.SmsApiKey, ["partnerID"] = s.SmsPartnerId };
            var response = await PostAsync(s, "getbalance/", body, ct);
            return response?.ToString(Formatting.None) ?? "";
        }

        private async Task<JToken?> PostAsync(CommunicationSetting s, string endpoint, JObject body, CancellationToken ct)
        {
            var baseUrl = string.IsNullOrWhiteSpace(s.SmsApiUrl) ? DefaultApiUrl : s.SmsApiUrl.Trim();
            if (!baseUrl.EndsWith("/")) baseUrl += "/";

            var client = _httpClientFactory.CreateClient(nameof(TextSmsGateway));
            using var content = new StringContent(body.ToString(Formatting.None), Encoding.UTF8, "application/json");
            using var res = await client.PostAsync(baseUrl + endpoint, content, ct);
            var text = await res.Content.ReadAsStringAsync(ct);
            if (!res.IsSuccessStatusCode)
                throw new HttpRequestException($"HTTP {(int)res.StatusCode}: {Truncate(text, 300)}");
            try { return string.IsNullOrWhiteSpace(text) ? null : JToken.Parse(text); }
            catch (JsonException) { throw new HttpRequestException("Unexpected gateway response: " + Truncate(text, 300)); }
        }

        private static SmsSendResult ToResult(string clientId, JToken r)
        {
            // The API spells the field "respose-code"; accept the correct spelling too.
            var code = (string?)(r["respose-code"] ?? r["response-code"]) ?? "";
            var description = (string?)r["response-description"] ?? "";
            var messageId = (string?)r["messageid"];
            if (code == "200")
                return new SmsSendResult(clientId, true, false, messageId, description);
            return new SmsSendResult(clientId, false, RetryableCodes.Contains(code), messageId,
                $"{code} {Describe(code, description)}".Trim());
        }

        private static string Describe(string code, string description) => code switch
        {
            "1001" => "Invalid sender ID",
            "1002" => "Network not allowed",
            "1003" => "Invalid mobile number",
            "1004" => "Low bulk SMS credits",
            "1006" => "Invalid gateway credentials",
            "1008" => "No delivery report",
            "4090" => "Gateway internal error",
            "4091" => "No partner ID set",
            "4092" => "No API key provided",
            "4093" => "Gateway account details not found",
            _ => description
        };

        private static string Truncate(string s, int n) => s.Length <= n ? s : s[..n];
    }
}
