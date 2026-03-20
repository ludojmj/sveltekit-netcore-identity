using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using Server.Controllers;
using Server.Models;
using Server.Services.Interfaces;
using Xunit;

namespace Server.UnitTest.Controllers;

public class TestStuffRoutes
{
    private static readonly Guid IdTest = Guid.NewGuid();

    private static readonly UserModel CurrentUserModelTest = new()
    {
        Id = "11",
        Name = "GivenName FamilyName",
        GivenName = "GivenName",
        FamilyName = "FamilyName",
        Email = "Email"
    };

    private static readonly DatumModel TestDatum = new()
    {
        Id = IdTest,
        Label = "Label",
        Description = "Description",
        OtherInfo = "OtherInfo",
        User = CurrentUserModelTest
    };
    private static readonly Task<DatumModel> TestDatumAsync = Task.FromResult(TestDatum);

    private static readonly StuffModel TestStuff = new()
    {
        DatumList = [TestDatum]
    };
    private static readonly Task<StuffModel> TestStuffAsync = Task.FromResult(TestStuff);

    // ***** ***** ***** LIST
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public async Task StuffController_GetStuffList_ShouldReturn_Ok(int page)
    {
        // Arrange
        var mockService = Mock.Of<IStuffService>(x =>
            x.GetListAsync(page) == TestStuffAsync);

        // Act
        var result = await StuffRouteHandlers.GetListAsync(page, string.Empty, mockService);

        // Assert
        var okResult = result as Ok<StuffModel>;
        var expected = TestDatum.Id;
        var actual = okResult?.Value?.DatumList.ToArray()[0].Id;
        Assert.Equal(expected, actual);
    }

    // ***** ***** ***** SEARCH
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public async Task StuffController_SearchStuffList_ShouldReturn_Ok(int page)
    {
        // Arrange
        var mockService = Mock.Of<IStuffService>(x =>
            x.SearchListAsync("foo") == TestStuffAsync);

        // Act
        var result = await StuffRouteHandlers.GetListAsync(page, "foo", mockService);

        // Assert
        var okResult = result as Ok<StuffModel>;
        var expected = TestDatum.Id;
        var actual = okResult?.Value?.DatumList.ToArray()[0].Id;
        Assert.Equal(expected, actual);
    }

    // ***** ***** ***** CREATE
    [Fact]
    public async Task StuffController_Create_ShouldReturnCreated()
    {
        // Arrange
        var mockService = Mock.Of<IStuffService>(x =>
            x.CreateAsync(TestDatum) == TestDatumAsync);

        // Act
        var result = await StuffRouteHandlers.CreateAsync(TestDatum, mockService);

        // Assert
        var okResult = result as Created<DatumModel>;
        var expected = TestDatum.Id;
        var actual = okResult?.Value?.Id;
        Assert.Equal(expected, actual);
    }

    // ***** ***** ***** READ SINGLE
    [Fact]
    public async Task StuffController_Read_ShouldReturn_Ok()
    {
        // Arrange
        var mockService = Mock.Of<IStuffService>(x =>
            x.ReadAsync(It.IsAny<Guid>()) == TestDatumAsync);

        // Act
        var result = await StuffRouteHandlers.ReadAsync(TestDatum.Id, mockService);

        // Assert
        var okResult = result as Ok<DatumModel>;
        var expected = TestDatum.Id;
        var actual = okResult?.Value?.Id;
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task StuffController_Read_ShouldReturn_Null()
    {
        // Arrange
        var mockService = Mock.Of<IStuffService>();

        // Act
        var result = await StuffRouteHandlers.ReadAsync(TestDatum.Id, mockService);

        // Assert
        var okResult = result as Ok;
        Assert.NotNull(okResult);
        var okResult2 = result as Ok<DatumModel>;
        Assert.Null(okResult2);
    }

    // ***** ***** ***** UPDATE
    [Fact]
    public async Task StuffController_UpdateStuff_ShouldReturn_Ok()
    {
        // Arrange
        var mockService = Mock.Of<IStuffService>(x =>
            x.UpdateAsync(It.IsAny<Guid>(), TestDatum) == TestDatumAsync);
        Guid existingId = TestDatum.Id;

        // Act
        var result = await StuffRouteHandlers.UpdateAsync(existingId, TestDatum, mockService);

        // Assert
        var okResult = result as Ok<DatumModel>;
        var expected = TestDatum.Id;
        var actual = okResult?.Value?.Id;
        Assert.Equal(expected, actual);
    }

    // ***** ***** ***** DELETE
    [Fact]
    public async Task StuffController_DeleteStuff_ShouldReturnNoContent()
    {
        // Arrange
        var mockService = Mock.Of<IStuffService>();

        // Act
        var result = await StuffRouteHandlers.DeleteAsync(Guid.NewGuid(), mockService);

        // Assert
        Assert.IsType<NoContent>(result);
    }
}
