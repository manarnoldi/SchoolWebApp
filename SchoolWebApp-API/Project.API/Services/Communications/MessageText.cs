using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace SchoolWebApp.API.Services.Communications
{
    /// <summary>
    /// Phone numbers, SMS part counting and {Placeholder} rendering.
    /// </summary>
    public static class MessageText
    {
        /// <summary>Kenya (EAT, UTC+3, no daylight saving) - month and day boundaries in reports.</summary>
        public static readonly TimeSpan SchoolUtcOffset = TimeSpan.FromHours(3);

        private static readonly Regex PlaceholderPattern = new(@"\{(\w+)\}", RegexOptions.Compiled);

        /// <summary>
        /// Normalises a Kenyan mobile number to 2547XXXXXXXX / 2541XXXXXXXX, the form
        /// the gateway expects. Records hold every shape - 0712..., 712... (the
        /// leading zero lost in a spreadsheet), +254 712 ... - and sometimes two
        /// numbers in one field ("0712.../0722..."); the first valid one is used.
        /// Returns null when nothing usable is found.
        /// </summary>
        public static string? NormalizePhone(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;
            foreach (var part in raw.Split(new[] { '/', ',', ';', '|', '&' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var digits = new string(part.Where(char.IsDigit).ToArray());
                string? candidate = digits.Length switch
                {
                    12 when digits.StartsWith("254") => digits,
                    10 when digits.StartsWith("0") => "254" + digits[1..],
                    9 when digits[0] is '7' or '1' => "254" + digits,
                    _ => null
                };
                if (candidate != null && (candidate[3] == '7' || candidate[3] == '1'))
                    return candidate;
            }
            return null;
        }

        public static string? NormalizeEmail(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;
            var email = raw.Trim();
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$") ? email : null;
        }

        // GSM 03.38 basic character set, and the extension table (each of those
        // costs two characters).
        private const string GsmBasic =
            "@£$¥èéùìòÇ\nØø\rÅåΔ_ΦΓΛΩΠΨΣΘΞÆæßÉ !\"#¤%&'()*+,-./0123456789:;<=>?" +
            "¡ABCDEFGHIJKLMNOPQRSTUVWXYZÄÖÑÜ§¿abcdefghijklmnopqrstuvwxyzäöñüà";
        private const string GsmExtended = "^{}\\[~]|€\f";

        /// <summary>
        /// Swaps characters that word processors and phones insert - curly quotes,
        /// dashes, non-breaking spaces - for their plain equivalents. The gateway
        /// requires GSM-7 text, and one stray curly quote would otherwise switch the
        /// whole message to Unicode and more than double its cost.
        /// </summary>
        public static string SanitizeSms(string text)
        {
            var sb = new StringBuilder(text.Length);
            foreach (var c in text.Replace("\r\n", "\n"))
            {
                sb.Append(c switch
                {
                    '‘' or '’' or '‛' or '′' => '\'',
                    '“' or '”' or '‟' or '″' => '"',
                    '–' or '—' or '−' => '-',
                    ' ' or ' ' or ' ' or '\t' => ' ',
                    '…' => '.',
                    _ => c
                });
            }
            return sb.ToString().Trim();
        }

        /// <summary>
        /// Billable SMS parts: 160 GSM-7 characters in one part, 153 per part when
        /// concatenated; 70 / 67 when the text needs Unicode.
        /// </summary>
        public static int SmsParts(string? text)
        {
            if (string.IsNullOrEmpty(text)) return 0;
            var gsm = true;
            var length = 0;
            foreach (var c in text)
            {
                if (GsmBasic.IndexOf(c) >= 0) length++;
                else if (GsmExtended.IndexOf(c) >= 0) length += 2;
                else { gsm = false; break; }
            }
            if (!gsm)
            {
                length = text.Length;
                return length <= 70 ? 1 : (int)Math.Ceiling(length / 67.0);
            }
            return length <= 160 ? 1 : (int)Math.Ceiling(length / 153.0);
        }

        /// <summary>
        /// Replaces each {Key} with its value (keys match case-insensitively).
        /// Unknown placeholders are left as typed, so a misspelt one shows up in the
        /// preview rather than silently vanishing.
        /// </summary>
        public static string Render(string? template, IReadOnlyDictionary<string, string?> values)
        {
            if (string.IsNullOrEmpty(template)) return string.Empty;
            return PlaceholderPattern.Replace(template, m =>
                values.TryGetValue(m.Groups[1].Value, out var v) ? v ?? string.Empty : m.Value);
        }

        /// <summary>
        /// Email bodies are written as plain text; they go out as HTML with line
        /// breaks kept. A body that already contains markup is sent as it is.
        /// </summary>
        public static string ToEmailHtml(string body, string? schoolName)
        {
            var isHtml = Regex.IsMatch(body, @"<\s*(p|br|div|table|b|strong|h\d|ul|ol)\b", RegexOptions.IgnoreCase);
            var content = isHtml ? body : WebUtility.HtmlEncode(body).Replace("\n", "<br>");
            var footer = string.IsNullOrWhiteSpace(schoolName) ? "" :
                $"<p style=\"color:#888;font-size:12px;margin-top:24px\">{WebUtility.HtmlEncode(schoolName)}</p>";
            return "<div style=\"font-family:Arial,Helvetica,sans-serif;font-size:14px;line-height:1.5;color:#222\">" +
                   content + footer + "</div>";
        }

        public static string Money(decimal amount) => amount.ToString("#,##0.00");
    }
}
