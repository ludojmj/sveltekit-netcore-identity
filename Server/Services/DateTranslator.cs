namespace Server.Services;

public static class DateTranslator
{
    public static DateTime? ToUtcDate(this string input)
    {
        var result = string.IsNullOrEmpty(input) ? (DateTime?)null : DateTime.Parse(input).ToUniversalTime();
        return result;
    }

    public static string ToStrDate(this DateTime input)
    {
        var result = input.ToString("o");
        return result;
    }
}
