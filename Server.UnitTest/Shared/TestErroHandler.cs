using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Server.Shared;
using Xunit;

namespace Server.UnitTest.Shared;

public class TestErroHandler
{
    private readonly ILogger<ErrorHandler> _logger;
    private readonly IHostEnvironment _env;
    private readonly IProblemDetailsService _problemDetailsService;
    private readonly HttpContext _httpContext;

    public TestErroHandler()
    {
        _logger = Mock.Of<ILogger<ErrorHandler>>();
        _env = Mock.Of<IHostEnvironment>();
        _problemDetailsService = Mock.Of<IProblemDetailsService>();
        _httpContext = new DefaultHttpContext();
        _httpContext.Response.Body = new MemoryStream();
        _httpContext.Response.StatusCode = 200;
    }

    [Theory]
    [InlineData("Development")]
    [InlineData("Production")]
    public async Task ErrorHandler_NotFound(string env)
    {
        // Arrange
        Mock.Get(_env).Setup(x => x.EnvironmentName).Returns(env);
        var errorHandler = new ErrorHandler(_logger, _env, _problemDetailsService);
        var exception = new KeyNotFoundException("Not found");

        // Act
        var result = await errorHandler.TryHandleAsync(_httpContext, exception, It.IsAny<CancellationToken>());

        // Assert
        Assert.Equal(404, _httpContext.Response.StatusCode);
        Assert.True(result);
    }

    [Theory]
    [InlineData("Development", true)]
    [InlineData("Development", false)]
    [InlineData("Production", true)]
    [InlineData("Production", false)]
    public async Task ErrorHandler_Business_Conflict(string env, bool isBusiness)
    {
        // Arrange
        Mock.Get(_env).Setup(x => x.EnvironmentName).Returns(env);
        var errorHandler = new ErrorHandler(_logger, _env, _problemDetailsService);
        dynamic exception = isBusiness ? new BusinessException("Business error") : new DbUpdateException("DB Update error");

        // Act
        var result = await errorHandler.TryHandleAsync(_httpContext, exception, It.IsAny<CancellationToken>());

        // Assert
        Assert.Equal(409, _httpContext.Response.StatusCode);
        Assert.True(result);
    }

    [Theory]
    [InlineData("Development")]
    [InlineData("Production")]
    public async Task ErrorHandler_BadRequest(string env)
    {
        // Arrange
        Mock.Get(_env).Setup(x => x.EnvironmentName).Returns(env);
        var errorHandler = new ErrorHandler(_logger, _env, _problemDetailsService);
        var exception = new DivideByZeroException("Unhandled exception");

        // Act
        var result = await errorHandler.TryHandleAsync(_httpContext, exception, It.IsAny<CancellationToken>());

        // Assert
        Assert.Equal(400, _httpContext.Response.StatusCode);
        Assert.True(result);
    }
}
