using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;
using Server.Shared;
using Microsoft.Extensions.Logging;

namespace Server.UnitTest.Shared;

public class TestModelValidation
{
    private readonly ILogger<ModelValidationFilterAttribute> _logger = Mock.Of<ILogger<ModelValidationFilterAttribute>>();

    [Fact]
    public void ModelValidationFilterAttribute_ShouldThrowArgumentException_IfModelIsInvalid()
    {
        // Arrange
        var validationFilter = new ModelValidationFilterAttribute(_logger);
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("year", "invalid");

        var actionContext = new ActionContext(
            Mock.Of<HttpContext>(),
            Mock.Of<RouteData>(),
            Mock.Of<ActionDescriptor>(),
            modelState
        );
        var actionExecutingContext = new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object?>(),
            Mock.Of<Controller>()
        );

        // Act
        validationFilter.OnActionExecuting(actionExecutingContext);

        // Assert
        Mock.Get(_logger).Verify(x => x.Log(
            It.Is<LogLevel>(l => l == LogLevel.Error),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString() == "Model state is invalid: invalid"),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }
}
