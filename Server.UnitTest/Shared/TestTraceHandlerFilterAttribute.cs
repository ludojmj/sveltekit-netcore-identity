using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using Server.Models;
using Server.Shared;
using System.Security.Claims;
using System.Text.Json;
using Xunit;

namespace Server.UnitTest.Shared;

public class TestTraceHandlerFilterAttribute
{
    private static ILogger<TraceHandlerFilterAttribute> _logger = null!;

    public TestTraceHandlerFilterAttribute()
    {
        _logger = Mock.Of<ILogger<TraceHandlerFilterAttribute>>();
    }

    [Fact]
    public void Test_OnActionExecuting_Context_Null()
    {
        // Arrange
        var filter = new TraceHandlerFilterAttribute(_logger);

        // Act
        filter.OnActionExecuting(null);

        // Assert
        Mock.Get(_logger).Verify(x => x.Log(
            It.IsAny<LogLevel>(),
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            Times.Never);
    }

    [Fact]
    public void Test_OnActionExecuted_Context_Null()
    {
        // Arrange
        var filter = new TraceHandlerFilterAttribute(_logger);

        // Act
        filter.OnActionExecuted(null);

        // Assert
        Mock.Get(_logger).Verify(x => x.Log(
            It.IsAny<LogLevel>(),
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            Times.Never);
    }

    [Fact]
    public void Test_OnActionExecuting_No_Log_If_Service()
    {
        // Arrange
        var context = new ActionContext(
            Mock.Of<HttpContext>(x =>
            x.User.FindFirst(It.IsAny<string>()) == new Claim("name", "UserAPI")
         && x.Request.Path == "/path"
         && x.Request.RouteValues == new RouteValueDictionary("GetList")),
            Mock.Of<RouteData>(),
            Mock.Of<ActionDescriptor>(),
            Mock.Of<ModelStateDictionary>()
        );
        var executingContext = new ActionExecutingContext(
            context,
            new List<IFilterMetadata>(),
            new Dictionary<string, object?>(),
            Mock.Of<Controller>()
        )
        {
            ActionArguments =
                {
                    ["service"] = "123"
                }
        };

        var filter = new TraceHandlerFilterAttribute(_logger);

        // Act
        filter.OnActionExecuting(executingContext);

        // Assert
        Mock.Get(_logger).Verify(x => x.Log(
            It.IsAny<LogLevel>(),
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            Times.Never);
    }

    [Fact]
    public void Test_OnActionExecuting_Operation_AuthToken()
    {
        // Arrange
        UserModel expectedUser = new()
        {
            AppId = "UserAPI",
            Email = "UserAPI",
            Id = "UserAPI",
            Name = "UserAPI",
            Operation = "7: /path"
        };
        var userLog = JsonSerializer.Serialize(expectedUser, new JsonSerializerOptions { WriteIndented = true });
        var context = new ActionContext(
            Mock.Of<HttpContext>(x =>
            x.User.FindFirst(It.IsAny<string>()) == new Claim("name", "UserAPI")
         && x.Request.Path == "/path"
         && x.Request.RouteValues == new RouteValueDictionary("GetList")),
            Mock.Of<RouteData>(),
            Mock.Of<ActionDescriptor>(),
            Mock.Of<ModelStateDictionary>()
        );
        var executingContext = new ActionExecutingContext(
            context,
            new List<IFilterMetadata>(),
            new Dictionary<string, object?>(),
            Mock.Of<Controller>()
        )
        {
            ActionArguments =
                {
                    ["Id"] = "123"
                }
        };

        var filter = new TraceHandlerFilterAttribute(_logger);

        // Act
        filter.OnActionExecuting(executingContext);

        // Assert
        Mock.Get(_logger).Verify(x => x.Log(
            It.Is<LogLevel>(l => l == LogLevel.Information),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString() == userLog + " Request: Id=\"123\""),
            It.IsAny<Exception>(),
            (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()));
    }

    [Fact]
    public void Test_OnActionExecuted_Operation_AuthToken()
    {
        // Arrange
        UserModel expectedUser = new()
        {
            AppId = "UserAPI",
            Email = "UserAPI",
            Id = "UserAPI",
            Name = "UserAPI",
            Operation = "7: /path"
        };
        var userLog = JsonSerializer.Serialize(expectedUser, new JsonSerializerOptions { WriteIndented = true });
        var context = new ActionContext(
            Mock.Of<HttpContext>(x =>
            x.User.FindFirst(It.IsAny<string>()) == new Claim("name", "UserAPI")
         && x.Request.Path == "/path"
         && x.Request.RouteValues == new RouteValueDictionary("GetList")),
            Mock.Of<RouteData>(),
            Mock.Of<ActionDescriptor>(),
            Mock.Of<ModelStateDictionary>()
        );
        var exectudedContext = new ActionExecutedContext(
            context,
            new List<IFilterMetadata>(),
            Mock.Of<Controller>()
        )
        {
            Result = new ObjectResult("fubar")
        };

        var filter = new TraceHandlerFilterAttribute(_logger);

        // Act
        filter.OnActionExecuted(exectudedContext);

        // Assert
        Mock.Get(_logger).Verify(x => x.Log(
            It.Is<LogLevel>(l => l == LogLevel.Information),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString() == userLog + " Response: \"fubar\""),
            It.IsAny<Exception>(),
            (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()));
    }

    [Fact]
    public void Test_OnActionExecuting_Operation_Anonymous()
    {
        // Arrange
        UserModel expectedUser = new()
        {
            Operation = "7: /path"
        };
        var userLog = JsonSerializer.Serialize(expectedUser, new JsonSerializerOptions { WriteIndented = true });
        var context = new ActionContext(
            Mock.Of<HttpContext>(x =>
            x.User.FindFirst(It.IsAny<string>()) == null
         && x.Request.Path == "/path"
         && x.Request.RouteValues == new RouteValueDictionary("GetList")),
            Mock.Of<RouteData>(),
            Mock.Of<ActionDescriptor>(),
            Mock.Of<ModelStateDictionary>()
        );
        var executingContext = new ActionExecutingContext(
            context,
            new List<IFilterMetadata>(),
            new Dictionary<string, object?>(),
            Mock.Of<Controller>()
        )
        {
            ActionArguments =
                {
                    ["Id"] = "123"
                }
        };

        var filter = new TraceHandlerFilterAttribute(_logger);

        // Act
        filter.OnActionExecuting(executingContext);

        // Assert
        Mock.Get(_logger).Verify(x => x.Log(
            It.Is<LogLevel>(l => l == LogLevel.Information),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString() == userLog + " Request: Id=\"123\""),
            It.IsAny<Exception>(),
            (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()));
    }

    [Fact]
    public void Test_OnActionExecuted_Operation_Anonymous()
    {
        // Arrange
        UserModel expectedUser = new()
        {
            Operation = "7: /path"
        };
        var userLog = JsonSerializer.Serialize(expectedUser, new JsonSerializerOptions { WriteIndented = true });
        var context = new ActionContext(
            Mock.Of<HttpContext>(x =>
            x.User.FindFirst(It.IsAny<string>()) == null
         && x.Request.Path == "/path"
         && x.Request.RouteValues == new RouteValueDictionary("GetList")),
            Mock.Of<RouteData>(),
            Mock.Of<ActionDescriptor>(),
            Mock.Of<ModelStateDictionary>()
        );
        var exectudedContext = new ActionExecutedContext(
            context,
            new List<IFilterMetadata>(),
            Mock.Of<Controller>()
        )
        {
            Result = new ObjectResult("fubar")
        };

        var filter = new TraceHandlerFilterAttribute(_logger);

        // Act
        filter.OnActionExecuted(exectudedContext);

        // Assert
        Mock.Get(_logger).Verify(x => x.Log(
            It.Is<LogLevel>(l => l == LogLevel.Information),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString() == userLog + " Response: \"fubar\""),
            It.IsAny<Exception>(),
            (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()));
    }

    [Fact]
    public void Test_Truncate()
    {
        // Arrange
        string input = new('A', 4096);
        string depassement = $"{input}BBBB";

        // Act
        var result = depassement.Truncate();

        // Assert
        Assert.Equal(input, result);
    }

    [Fact]
    public void Test_Truncate_Input_Null()
    {
        // Act
        var result = Utils.Truncate(null);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Test_Truncate_Input_Empty()
    {
        // Act
        var result = string.Empty.Truncate();

        // Assert
        Assert.Equal(string.Empty, result);
    }
}
