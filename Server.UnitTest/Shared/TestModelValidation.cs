using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;
using Server.Shared;

namespace Server.UnitTest.Shared;

public class TestModelValidation
{
    [Fact]
    public void ModelValidationFilterAttribute_ShouldThrowArgumentException_IfModelIsInvalid()
    {
        // Arrange
        var validationFilter = new ModelValidationFilterAttribute();
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
        var exception = Record.Exception(() => validationFilter.OnActionExecuting(actionExecutingContext));

        // Assert
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("invalid", exception.Message);
    }
}
