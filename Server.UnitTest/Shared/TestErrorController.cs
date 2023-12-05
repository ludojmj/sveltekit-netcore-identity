using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Server.Shared;
using Xunit;

namespace Server.UnitTest.Shared;

public class TestErrorController
{
    private readonly IHostEnvironment _env;
    private readonly ILogger<ErrorController> _logger;
    private readonly IExceptionHandlerFeature _exception;

    public TestErrorController()
    {
        _env = Mock.Of<IHostEnvironment>();
        _logger = Mock.Of<ILogger<ErrorController>>();
        _exception = Mock.Of<IExceptionHandlerFeature>(x => x.Error == new ArgumentException("Should be displayed"));
    }

    [Fact]
    public void ErrorController_NotFoundObjectResult()
    {
        // Arrange
        Mock.Get(_exception).Setup(x => x.Error).Returns(new KeyNotFoundException("Not found"));

        var context = new DefaultHttpContext();
        context.Features.Set(_exception);

        var controller = new ErrorController()
        {
            ControllerContext = new ControllerContext() { HttpContext = context }
        };

        // Act
        IActionResult actionResult = controller.Error(_env, _logger);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult);
        var contentResult = Assert.IsType<ErrorModel>(notFoundResult.Value);
        var actual = contentResult.Error;
        Assert.Equal("Not found", actual);
    }

    [Fact]
    public void ErrorHandlerFilter_BadRequestObjectResult_Development()
    {
        // Arrange
        Mock.Get(_env).Setup(x => x.EnvironmentName).Returns("Development");

        var context = new DefaultHttpContext();
        context.Features.Set(_exception);

        var controller = new ErrorController()
        {
            ControllerContext = new ControllerContext() { HttpContext = context }
        };

        // Act
        IActionResult actionResult = controller.Error(_env, _logger);

        // Assert
        var notFoundResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        var contentResult = Assert.IsType<ErrorModel>(notFoundResult.Value);
        var actual = contentResult.Error;
        Assert.Equal("Should be displayed", actual);
    }

    [Fact]
    public void ErrorHandlerFilter_BadRequestObjectResult_Production()
    {
        // Arrange
        Mock.Get(_env).Setup(x => x.EnvironmentName).Returns("Production");

        var context = new DefaultHttpContext();
        context.Features.Set(_exception);

        var controller = new ErrorController()
        {
            ControllerContext = new ControllerContext() { HttpContext = context }
        };

        // Act
        IActionResult actionResult = controller.Error(_env, _logger);

        // Assert
        var notFoundResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        var contentResult = Assert.IsType<ErrorModel>(notFoundResult.Value);
        var actual = contentResult.Error;
        Assert.Equal("An error occured. Please try again later.", actual);
    }
}
