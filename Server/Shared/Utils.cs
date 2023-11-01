using Server.Models;
using System.Security.Claims;

namespace Server.Shared;

public static class Utils
{
    private const int CstMaxLength = 4096;

    public static UserModel GetCurrentUser(this HttpContext context)
    {
        return new UserModel
        {
            Operation = $"{context.Request.RouteValues.Values.FirstOrDefault()}: {context.Request.Path}",
            AppId = context.User.FindFirstValue("client_id"),
            Id = context.User.FindFirstValue(ClaimTypes.NameIdentifier),
            Name = context.User.FindFirstValue(ClaimTypes.Name),
            Email = context.User.FindFirstValue(ClaimTypes.Email)
        };
    }

    public static string Truncate(this string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        return value.Length <= CstMaxLength ? value : value[..CstMaxLength];
    }
}
