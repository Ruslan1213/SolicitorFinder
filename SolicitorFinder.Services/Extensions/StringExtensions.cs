using System.Text.RegularExpressions;

namespace SolicitorFinder.Services.Extensions;
internal static class StringExtensions
{
    public static string Clean(this string? text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        var result = text;

        result = System.Net.WebUtility.HtmlDecode(result);

        result = result.Replace("&nbsp;", " ")
                       .Replace("\u00A0", " ");

        result = Regex.Replace(result, @"\s+", " ");

        result = result.Trim();

        return result;
    }

    public static string CleanPhone(this string? phone)
    {
        if (string.IsNullOrEmpty(phone))
            return string.Empty;

        var cleaned = Clean(phone);

        cleaned = Regex.Replace(cleaned, @"[^\d+\s]", "");

        cleaned = Regex.Replace(cleaned, @"\s+", " ").Trim();

        return cleaned;
    }

    public static string CleanUrl(this string? url)
    {
        if (string.IsNullOrEmpty(url))
            return string.Empty;

        var cleaned = Clean(url);

        cleaned = cleaned.Replace(" ", "");

        return cleaned;
    }

    public static string CleanName(this string? name)
    {
        if (string.IsNullOrEmpty(name))
            return "Unknown";

        var cleaned = Clean(name);

        cleaned = Regex.Replace(cleaned, @"\s+", " ").Trim();

        cleaned = cleaned.TrimEnd('&', 'n', 'b', 's', 'p', ';');

        return cleaned.Trim();
    }
}
