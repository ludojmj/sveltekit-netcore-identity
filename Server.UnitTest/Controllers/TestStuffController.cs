using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Server.Controllers;
using Server.Models;
using Server.Services.Interfaces;

namespace Server.UnitTest.Controllers;

public class TestStuffController
{
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
        Id = "1",
        Label = "Label",
        Description = "Description",
        OtherInfo = "OtherInfo",
        User = CurrentUserModelTest
    };

    private static readonly StuffModel TestStuff = new()
    {
        DatumList = new Collection<DatumModel>
            {
                TestDatum
            }
    };

    // ***** ***** ***** LIST
    [Fact]
    public async Task StuffController_GetStuffList_ShouldReturn_Ok()
    {
        // Arrange
        var mockStuffService = Mock.Of<IStuffService>(x => x.GetListAsync(1) == Task.FromResult(TestStuff));
        var controller = new StuffController();

        // Act
        IActionResult actionResult = await controller.GetList(1, null, mockStuffService);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var contentResult = Assert.IsType<StuffModel>(okResult.Value);
        var expected = TestDatum.Id;
        var actual = contentResult.DatumList.ToArray()[0].Id;
        Assert.Equal(expected, actual);
    }

    // ***** ***** ***** SEARCH
    [Fact]
    public async Task StuffController_SearchStuffList_ShouldReturn_Ok()
    {
        // Arrange
        var mockStuffService = Mock.Of<IStuffService>(x => x.SearchListAsync("foo") == Task.FromResult(TestStuff));
        var controller = new StuffController();

        // Act
        IActionResult actionResult = await controller.GetList(0, "foo", mockStuffService);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var contentResult = Assert.IsType<StuffModel>(okResult.Value);
        var expected = TestDatum.Id;
        var actual = contentResult.DatumList.ToArray()[0].Id;
        Assert.Equal(expected, actual);
    }

    // ***** ***** ***** CREATE
    [Fact]
    public async Task StuffController_Create_ShouldReturnCreated()
    {
        // Arrange
        var mockStuffService = Mock.Of<IStuffService>(x => x.CreateAsync(TestDatum) == Task.FromResult(TestDatum));
        var controller = new StuffController();

        // Act
        IActionResult actionResult = await controller.Create(TestDatum, mockStuffService);

        // Assert
        var okResult = Assert.IsType<CreatedAtActionResult>(actionResult);
        var contentResult = Assert.IsType<DatumModel>(okResult.Value);
        Assert.Equal(TestDatum.Id, contentResult.Id);
    }

    // ***** ***** ***** READ SINGLE
    [Fact]
    public async Task StuffController_Read_ShouldReturn_Ok()
    {
        // Arrange
        var mockStuffService = Mock.Of<IStuffService>(x => x.ReadAsync("1") == Task.FromResult(TestDatum));
        var controller = new StuffController();

        // Act
        IActionResult actionResult = await controller.Read("1", mockStuffService);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var contentResult = Assert.IsType<DatumModel>(okResult.Value);
        var expected = TestDatum.Id;
        var actual = contentResult.Id;
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task StuffController_Read_ShouldReturn_Null()
    {
        // Arrange
        var mockStuffService = Mock.Of<IStuffService>();
        var controller = new StuffController();

        // Act
        IActionResult actionResult = await controller.Read("1", mockStuffService);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var contentResult = okResult.Value;
        var actual = contentResult;
        Assert.Null(actual);
    }

    // ***** ***** ***** UPDATE
    [Fact]
    public async Task StuffController_UpdateStuff_ShouldReturn_Ok()
    {
        // Arrange
        var mockStuffService = Mock.Of<IStuffService>(x => x.UpdateAsync("1", TestDatum) == Task.FromResult(TestDatum));
        var controller = new StuffController();

        // Act
        IActionResult actionResult = await controller.Update("1", TestDatum, mockStuffService);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var contentResult = Assert.IsType<DatumModel>(okResult.Value);
        Assert.Equal(TestDatum.Id, contentResult.Id);
    }

    // ***** ***** ***** DELETE
    [Fact]
    public async Task StuffController_DeleteStuff_ShouldReturnNoContent()
    {
        // Arrange
        var mockStuffService = Mock.Of<IStuffService>();
        var controller = new StuffController();

        // Act
        IActionResult actionResult = await controller.Delete("2", mockStuffService);

        // Assert
        Assert.IsType<NoContentResult>(actionResult);
    }
}
