using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using Server.Controllers;
using Server.Models;
using Server.Services.Interfaces;
using Xunit;

namespace Server.UnitTest.Controllers;

public class TestStuffRoutes
{
    private static MethodInfo? CallCrud(string methodName)
    {
        return typeof(StuffRouteHandlers).GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);
    }

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

    private static readonly StuffModel TestStuff = new()
    {
        DatumList = [TestDatum]
    };

    // ***** ***** ***** LIST
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public async Task StuffController_GetStuffList_ShouldReturn_Ok(int page)
    {
        // Arrange
        var mockService = Mock.Of<IStuffService>(x =>
            x.GetListAsync(page) == Task.FromResult(TestStuff));

        // Act
        var result = await (Task<IResult>)CallCrud("GetListAsync")!.Invoke(null, [page, string.Empty, mockService])!;

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
            x.SearchListAsync("foo") == Task.FromResult(TestStuff));

        // Act
        var result = await (Task<IResult>)CallCrud("GetListAsync")!.Invoke(null, [page, "foo", mockService])!;

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
            x.CreateAsync(TestDatum) == Task.FromResult(TestDatum));

        // Act
        var result = await (Task<IResult>)CallCrud("CreateAsync")!.Invoke(null, [TestDatum, mockService])!;

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
            x.ReadAsync(It.IsAny<Guid>()) == Task.FromResult(TestDatum));

        // Act
        var result = await (Task<IResult>)CallCrud("ReadAsync")!.Invoke(null, [TestDatum.Id, mockService])!;

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
        var result = await (Task<IResult>)CallCrud("ReadAsync")!.Invoke(null, [TestDatum.Id, mockService])!;

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
            x.UpdateAsync(It.IsAny<Guid>(), TestDatum) == Task.FromResult(TestDatum));
        Guid existingId = TestDatum.Id;

        // Act
        var result = await (Task<IResult>)CallCrud("UpdateAsync")!.Invoke(null, [existingId, TestDatum, mockService])!;

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
        var result = await (Task<IResult>)CallCrud("DeleteAsync")!.Invoke(null, [Guid.NewGuid(), mockService])!;

        // Assert
        Assert.IsType<NoContent>(result);
    }
}
