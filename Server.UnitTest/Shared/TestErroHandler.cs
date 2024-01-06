using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Moq;
using Server.Shared;
using Xunit;

namespace Server.UnitTest.Shared;

public class TestErroHandler
{
    private readonly IHostEnvironment _env;
    private readonly HttpContext _httpContext;

    public TestErroHandler()
    {
        _env = Mock.Of<IHostEnvironment>();
        _httpContext = new DefaultHttpContext();
        _httpContext.Response.Body = new MemoryStream();
        _httpContext.Response.StatusCode = 200;
    }

    [Fact]
    public async Task ErrorHandler_NotFound()
    {
        // Arrange
        var errorHandler = new ErrorHandler(_env);
        var exception = new KeyNotFoundException("Not found");

        // Act
        var result = await errorHandler.TryHandleAsync(_httpContext, exception, It.IsAny<CancellationToken>());

        // Assert
        Assert.Equal(404, _httpContext.Response.StatusCode);
        Assert.True(result);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ErrorHandler_Business_Conflict(bool isBusiness)
    {
        // Arrange
        var errorHandler = new ErrorHandler(_env);
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
        var errorHandler = new ErrorHandler(_env);
        var exception = new DivideByZeroException("Unhandled exception");

        // Act
        var result = await errorHandler.TryHandleAsync(_httpContext, exception, It.IsAny<CancellationToken>());

        // Assert
        Assert.Equal(400, _httpContext.Response.StatusCode);
        Assert.True(result);
    }
}
