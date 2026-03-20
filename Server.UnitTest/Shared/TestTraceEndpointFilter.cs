using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Moq;
using Server.Models;
using Server.Shared;
using Xunit;

namespace Server.UnitTest.Shared;

public class TestTraceEndpointFilter
{
    private readonly ILogger<TraceEndpointFilter> _logger = Mock.Of<ILogger<TraceEndpointFilter>>();
    private readonly EndpointFilterDelegate _nextMock = Mock.Of<EndpointFilterDelegate>();

    private static readonly UserModel User = new()
    {
        Name = "Name",
        AppId = "AppId",
        Email = "Email",
        Id = "Id",
        Ip = "127.0.0.1",
        Operation = "Operation"
    };

    private static readonly Claim[] Identity =
    [
        new("client_id", "AppId"),
        new("sub", "Id"),
        new("name", "Name"),
        new("email", "Email")
    ];

    [Fact]
    public async Task TraceEndpointFilter_Should_Log_User_Req_Resp()
    {
        // Arrange
        string userInfo = User.IndentSerialize();
        var metadata = new EndpointMetadataCollection();
        var endpointInstance = new Endpoint(_ => Task.CompletedTask, metadata, "Operation");
        var endpoint = Mock.Of<IEndpointFeature>(x => x.Endpoint == endpointInstance);
        var ipAddress = IPAddress.Parse("127.0.0.1");
        var context = Mock.Of<HttpContext>(x =>
            x.User == new ClaimsPrincipal(new ClaimsIdentity(Identity))
         && x.Features.Get<IEndpointFeature>() == endpoint
         && x.Connection.RemoteIpAddress == ipAddress
         && x.Request.Path == "/path/to/resource"
         && x.Request.Method == "GET"
        );
        IList<object> objectList = [];
        var executingContext = Mock.Of<EndpointFilterInvocationContext>(x =>
            x.HttpContext == context
         && x.Arguments == objectList);

        var filter = new TraceEndpointFilter(_logger);

        // Act
        await filter.InvokeAsync(executingContext, _nextMock);

        // Assert
        Mock.Get(_logger).Verify(x => x.Log(
            It.Is<LogLevel>(l => l == LogLevel.Information),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) =>
                v.ToString()!.Contains(userInfo)
             && v.ToString()!.Contains("Request:")
             && v.ToString()!.Contains("Response:")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }
}
