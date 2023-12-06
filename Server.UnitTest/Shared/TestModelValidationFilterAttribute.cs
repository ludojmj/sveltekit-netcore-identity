using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Moq;
using Server.Shared;
using Xunit;

namespace Server.UnitTest.Shared;

public class TestModelValidationFilterAttribute
{
    [Fact]
    public void ModelValidationFilterAttribute_Should_Should_Pass()
    {
        // Arrange
        var filter = new ModelValidationFilterAttribute();
        var actionContext = new ActionContext(
            Mock.Of<HttpContext>(),
            Mock.Of<RouteData>(),
            Mock.Of<ActionDescriptor>(),
            Mock.Of<ModelStateDictionary>()
        );
        var actionExecutingContext = new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object?>(),
            Mock.Of<Controller>()
        );

        // Act
        filter.OnActionExecuting(actionExecutingContext);
        var exception = Record.Exception(() => filter.OnActionExecuting(actionExecutingContext));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void ModelValidationFilterAttribute_Should_Throw_ArgumentException()
    {
        // Arrange
        var filter = new ModelValidationFilterAttribute();
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
        var ex = Assert.Throws<ArgumentException>(() => filter.OnActionExecuting(actionExecutingContext));

        // Assert
        Assert.Equal("Model state is invalid: invalid", ex.Message);
    }
}
