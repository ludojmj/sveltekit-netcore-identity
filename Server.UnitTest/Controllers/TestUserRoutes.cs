using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using Server.Controllers;
using Server.Models;
using Server.Services.Interfaces;
using Xunit;

namespace Server.UnitTest.Controllers;

public class TestUserRoutes
{
    private static MethodInfo? CallCrud(string methodName)
    {
        return typeof(UserRouteHandlers).GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);
    }

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
        UserList = [TestUser]
    };

    // ***** ***** ***** LIST
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public async Task UserController_GetUserList_ShouldReturn_Ok(int page)
    {
        // Arrange
        var mockService = Mock.Of<IUserService>(x =>
            x.GetListAsync(page) == Task.FromResult(TestDirectory));

        // Act
        var result = await (Task<IResult>)CallCrud("GetListAsync")!.Invoke(null, [page, string.Empty, mockService])!;

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
            x.SearchListAsync("foo") == Task.FromResult(TestDirectory));

        // Act
        var result = await (Task<IResult>)CallCrud("GetListAsync")!.Invoke(null, [page, "foo", mockService])!;

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
            x.CreateAsync(TestUser) == Task.FromResult(TestUser));

        // Act
        var result = await (Task<IResult>)CallCrud("CreateAsync")!.Invoke(null, [TestUser, mockService])!;

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
            x.ReadAsync(It.IsAny<string>()) == Task.FromResult(TestUser));

        // Act
        var result = await (Task<IResult>)CallCrud("ReadAsync")!.Invoke(null, [TestUser.Id, mockService])!;

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
        var result = await (Task<IResult>)CallCrud("ReadAsync")!.Invoke(null, [TestUser.Id, mockService])!;

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
            x.UpdateAsync(It.IsAny<string>(), TestUser) == Task.FromResult(TestUser));
        string existingId = TestUser.Id;

        // Act
        var result = await (Task<IResult>)CallCrud("UpdateAsync")!.Invoke(null, [existingId, TestUser, mockService])!;

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
        var result = await (Task<IResult>)CallCrud("DeleteAsync")!.Invoke(null, ["2", mockService])!;

        // Assert
        Assert.IsType<NoContent>(result);
    }
}
