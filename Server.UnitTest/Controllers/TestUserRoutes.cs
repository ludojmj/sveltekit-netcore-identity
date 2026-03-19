using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using Server.Controllers;
using Server.Models;
using Server.Services.Interfaces;
using Xunit;

namespace Server.UnitTest.Controllers;

public class TestUserRoutes
{
    private static readonly UserModel TestUser = new()
    {
        Id = "11",
        Name = "GivenName FamilyName",
        GivenName = "GivenName",
        FamilyName = "FamilyName",
        Email = "Email"
    };
    private static readonly Task<UserModel> TestUserAsync = Task.FromResult(TestUser);

    private static readonly DirectoryModel TestDirectory = new()
    {
        UserList = [TestUser]
    };
    private static readonly Task<DirectoryModel> TestDirectoryAsync = Task.FromResult(TestDirectory);

    // ***** ***** ***** LIST
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public async Task UserController_GetUserList_ShouldReturn_Ok(int page)
    {
        // Arrange
        var mockService = Mock.Of<IUserService>(x =>
            x.GetListAsync(page) == TestDirectoryAsync);

        // Act
        var result = await UserRouteHandlers.GetListAsync(page, string.Empty, mockService);

        // Assert
        var okResult = result as Ok<DirectoryModel>;
        var expected = TestUser.Id;
        var actual = okResult?.Value?.UserList.ToArray()[0].Id;
        Assert.Equal(expected, actual);
    }

    // ***** ***** ***** SEARCH
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public async Task UserController_SearchUserList_ShouldReturn_Ok(int page)
    {
        // Arrange
        var mockService = Mock.Of<IUserService>(x =>
            x.SearchListAsync("foo") == TestDirectoryAsync);

        // Act
        var result = await UserRouteHandlers.GetListAsync(page, "foo", mockService);

        // Assert
        var okResult = result as Ok<DirectoryModel>;
        var expected = TestUser.Id;
        var actual = okResult?.Value?.UserList.ToArray()[0].Id;
        Assert.Equal(expected, actual);
    }

    // ***** ***** ***** CREATE
    [Fact]
    public async Task UserController_Create_ShouldReturnCreated()
    {
        // Arrange
        var mockService = Mock.Of<IUserService>(x =>
            x.CreateAsync(TestUser) == TestUserAsync);

        // Act
        var result = await UserRouteHandlers.CreateAsync(TestUser, mockService);

        // Assert
        var okResult = result as Created<UserModel>;
        var expected = TestUser.Id;
        var actual = okResult?.Value?.Id;
        Assert.Equal(expected, actual);
    }

    // ***** ***** ***** READ SINGLE
    [Fact]
    public async Task UserController_Read_ShouldReturn_Ok()
    {
        // Arrange
        var mockService = Mock.Of<IUserService>(x =>
            x.ReadAsync(It.IsAny<string>()) == TestUserAsync);

        // Act
        var result = await UserRouteHandlers.ReadAsync(TestUser.Id, mockService);

        // Assert
        var okResult = result as Ok<UserModel>;
        var expected = TestUser.Id;
        var actual = okResult?.Value?.Id;
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task UserController_Read_ShouldReturn_Null()
    {
        // Arrange
        var mockService = Mock.Of<IUserService>();

        // Act
        var result = await UserRouteHandlers.ReadAsync(TestUser.Id, mockService);

        // Assert
        var okResult = result as Ok;
        Assert.NotNull(okResult);
        var okResult2 = result as Ok<UserModel>;
        Assert.Null(okResult2);
    }

    // ***** ***** ***** UPDATE
    [Fact]
    public async Task UserController_UpdateUser_ShouldReturn_Ok()
    {
        // Arrange
        var mockService = Mock.Of<IUserService>(x =>
            x.UpdateAsync(It.IsAny<string>(), TestUser) == TestUserAsync);
        string existingId = TestUser.Id;

        // Act
        var result = await UserRouteHandlers.UpdateAsync(existingId, TestUser, mockService);

        // Assert
        var okResult = result as Ok<UserModel>;
        var expected = TestUser.Id;
        var actual = okResult?.Value?.Id;
        Assert.Equal(expected, actual);
    }

    // ***** ***** ***** DELETE
    [Fact]
    public async Task UserController_DeleteUser_ShouldReturnNoContent()
    {
        // Arrange
        var mockService = Mock.Of<IUserService>();

        // Act
        var result = await UserRouteHandlers.DeleteAsync("2", mockService);

        // Assert
        Assert.IsType<NoContent>(result);
    }
}
