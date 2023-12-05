using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
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

    public static void RequestLogger(this ActionExecutingContext context, ILogger logger, string userInfo)
    {
        string flux = context.ActionArguments.IndentSerialize();
        logger.LogInformation("{userInfo} Request: {flux}", userInfo, flux);
    }

    public static void ResponseLogger(this ActionExecutedContext context, ILogger logger, string userInfo)
    {
        if (context.Result is ObjectResult elt)
        {
            string fluxFull = elt.Value.IndentSerialize();
            string flux = fluxFull.Truncate();
            logger.LogInformation("{userInfo} Response: {flux}", userInfo, flux);
        }
    }

    public static UserModel GetCurrentUser(this HttpContext context)
    {
        return new UserModel
        {
            Operation = $"{context.Request.RouteValues.Values.FirstOrDefault()}: {context.Request.Path}",
            AppId = context.User.FindFirstValue("client_id"),
            Id = context.User.FindFirstValue(ClaimTypes.NameIdentifier),
            Name = context.User.FindFirstValue("name"),
            Email = context.User.FindFirstValue(ClaimTypes.Email),
            Ip = context.Connection.RemoteIpAddress?.ToString()
        };
    }

    public static string Truncate(this string value)
    {
        const int cstLength = 4096;
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        return value.Length <= cstLength ? value : value[..cstLength];
    }
}
