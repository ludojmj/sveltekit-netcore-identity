using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Server.Controllers;
using Server.Models;
using Server.Services.Interfaces;

namespace Server.UnitTest.Controllers;

public class TestUserController
{
    private static readonly UserModel TestUser = new()
    {
        Id = "11",
        Name = "GivenName FamilyName",
        GivenName = "GivenName",
        FamilyName = "FamilyName",
        Email = "Email"
    };

    private static readonly DirectoryModel TestDirectory = new()
    {
        UserList = new Collection<UserModel>
            {
                TestUser
            }
    };

    // ***** ***** ***** LIST
    [Fact]
    public async Task UserController_GetUserList_ShouldReturn_Ok()
    {
        // Arrange
        var mockUserService = Mock.Of<IUserService>(x => x.GetListAsync(1) == Task.FromResult(TestDirectory));
        var controller = new UserController();

        // Act
        IActionResult actionResult = await controller.GetList(1, null, mockUserService);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var contentResult = Assert.IsType<DirectoryModel>(okResult.Value);
        var expected = TestUser.Id;
        var actual = contentResult.UserList.ToArray()[0].Id;
        Assert.Equal(expected, actual);
    }

    // ***** ***** ***** SEARCH
    [Fact]
    public async Task UserController_SearchUserList_ShouldReturn_Ok()
    {
        // Arrange
        var mockUserService = Mock.Of<IUserService>(x => x.SearchListAsync("foo") == Task.FromResult(TestDirectory));
        var controller = new UserController();

        // Act
        IActionResult actionResult = await controller.GetList(0, "foo", mockUserService);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var contentResult = Assert.IsType<DirectoryModel>(okResult.Value);
        var expected = TestUser.Id;
        var actual = contentResult.UserList.ToArray()[0].Id;
        Assert.Equal(expected, actual);
    }

    // ***** ***** ***** CREATE
    [Fact]
    public async Task UserController_Create_ShouldReturnCreated()
    {
        // Arrange
        var mockUserService = Mock.Of<IUserService>(x => x.CreateAsync(TestUser) == Task.FromResult(TestUser));
        var controller = new UserController();

        // Act
        IActionResult actionResult = await controller.Create(TestUser, mockUserService);

        // Assert
        var okResult = Assert.IsType<CreatedAtActionResult>(actionResult);
        var contentResult = Assert.IsType<UserModel>(okResult.Value);
        Assert.Equal(TestUser.Id, contentResult.Id);
    }

    // ***** ***** ***** READ SINGLE
    [Fact]
    public async Task UserController_Read_ShouldReturn_Ok()
    {
        // Arrange
        var mockUserService = Mock.Of<IUserService>(x => x.ReadAsync("1") == Task.FromResult(TestUser));
        var controller = new UserController();

        // Act
        IActionResult actionResult = await controller.Read("1", mockUserService);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var contentResult = Assert.IsType<UserModel>(okResult.Value);
        var expected = TestUser.Id;
        var actual = contentResult.Id;
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task UserController_Read_ShouldReturn_Null()
    {
        // Arrange
        var mockUserService = Mock.Of<IUserService>();
        var controller = new UserController();

        // Act
        IActionResult actionResult = await controller.Read("1", mockUserService);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var contentResult = okResult.Value;
        var actual = contentResult;
        Assert.Null(actual);
    }

    // ***** ***** ***** UPDATE
    [Fact]
    public async Task UserController_UpdateUser_ShouldReturn_Ok()
    {
        // Arrange
        var mockUserService = Mock.Of<IUserService>(x => x.UpdateAsync("1", TestUser) == Task.FromResult(TestUser));
        var controller = new UserController();

        // Act
        IActionResult actionResult = await controller.Update("1", TestUser, mockUserService);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var contentResult = Assert.IsType<UserModel>(okResult.Value);
        Assert.Equal(TestUser.Id, contentResult.Id);
    }

    // ***** ***** ***** DELETE
    [Fact]
    public async Task UserController_DeleteUser_ShouldReturnNoContent()
    {
        // Arrange
        var mockUserService = Mock.Of<IUserService>();
        var controller = new UserController();

        // Act
        IActionResult actionResult = await controller.Delete("2", mockUserService);

        // Assert
        Assert.IsType<NoContentResult>(actionResult);
    }
}
