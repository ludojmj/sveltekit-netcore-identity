using Server.Models;
using System.Security.Claims;
using System.Text.Json;

namespace Server.Shared;

public static class Utils
{
    private static readonly JsonSerializerOptions JSerOptions = new() { WriteIndented = true };

    public static string IndentSerialize<T>(this T value)
    {
        return JsonSerializer.Serialize(value, JSerOptions);
    }

    public static IList<object> CleanTrace(this IEnumerable<object> argumentList)
    {
        List<object> list = [];
        list.AddRange(argumentList.Where(arg => arg != null && arg.GetType().Namespace?.StartsWith("Server", StringComparison.OrdinalIgnoreCase) == true));
        return list;
    }

    public static string Truncate(this string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        return value.Length <= maxLength ? value : value[..maxLength];
    }

    public static UserModel GetCurrentUser(this HttpContext context)
    {
        var operation = $"{context.GetEndpoint()?.DisplayName}";
        return new UserModel
        {
            Operation = $"{operation.Replace('>', '=')}",
            AppId = context.User.FindFirstValue("client_id"),
            Id = context.User.FindFirstValue("sub"),
            Name = context.User.FindFirstValue("name"),
            FamilyName = context.User.FindFirstValue("family_name"),
            GivenName = context.User.FindFirstValue("given_name"),
            Email = context.User.FindFirstValue("email"),
            Ip = context.Connection.RemoteIpAddress?.ToString()
        };
    }
}
