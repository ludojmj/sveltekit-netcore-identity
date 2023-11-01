using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Server.DbModels;
using Server.Models;
using Server.Services;
using Server.Services.Interfaces;
using Xunit;

namespace Server.UnitTest.Services;

public class TestUserService
{
    private static readonly UserModel TestUserModel = new()
    {
        Id = "11",
        Name = "GivenName FamilyName",
        GivenName = "GivenName",
        FamilyName = "FamilyName",
        Email = "Email",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    private readonly TUser _dbUser = new()
    {
        UsrId = "11",
        UsrName = "GivenName FamilyName",
        UsrGivenName = "GivenName",
        UsrFamilyName = "FamilyName",
        UsrEmail = "Email",
        UsrCreatedAt = DateTime.UtcNow.ToString("o"),
        UsrUpdatedAt = DateTime.UtcNow.ToString("o")
    };

    private readonly SqliteConnection _connection;
    private readonly StuffDbContext _dbContext;
    private readonly IUserService _userService;

    public TestUserService()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        var options = new DbContextOptionsBuilder<StuffDbContext>()
            .UseSqlite(_connection)
            .Options;
        _dbContext = new StuffDbContext(options);
        _dbContext.Database.EnsureCreated();
        _userService = new UserService(_dbContext);
    }

    [Fact]
    public void Dispose()
    {
        _dbContext.Dispose();
        _connection.Close();
    }

    // ***** ***** ***** LIST
    [Fact]
    public async Task UserService_GetListAsync_ShouldReturn_Ok()
    {
        // Arrange
        _dbContext.Add(_dbUser);
        await _dbContext.SaveChangesAsync();

        // Act
        var serviceResult = await _userService.GetListAsync(1);

        // Assert
        var expected = TestUserModel.Id;
        var actual = serviceResult.UserList.ToArray()[0].Id;
        Assert.Equal(expected, actual);
    }

    // ***** ***** ***** SEARCH
    [Fact]
    public async Task UserService_SearchListAsync_ShouldReturn_Ok()
    {
        // Arrange
        _dbContext.Add(_dbUser);
        await _dbContext.SaveChangesAsync();

        // Act
        var serviceResult = await _userService.SearchListAsync("GIVENNAME");

        // Assert
        var expected = TestUserModel.Id;
        var actual = serviceResult.UserList.ToArray()[0].Id;
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task UserService_SearchListAsync_ShouldThrow_ArgumentException()
    {
        // Arrange
        var dbUserList = new List<TUser>();
        for (int idx = 0; idx < 7; idx++)
        {
            var tpmUser = new TUser { UsrId = _dbUser.UsrId + idx, UsrGivenName = _dbUser.UsrGivenName + idx };
            dbUserList.Add(tpmUser);
        }

        _dbContext.AddRange(dbUserList);
        await _dbContext.SaveChangesAsync();

        // Act
        var serviceResult = _userService.SearchListAsync("GIVENNAME");
        var exception = await Record.ExceptionAsync(() => serviceResult);

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("Too many results. Please narrow your search.", exception.Message);
    }

    // ***** ***** ***** CREATE
    [Fact]
    public async Task UserService_CreateAsync_ShouldReturn_Ok()
    {
        // Arrange
        // No user

        // Act
        var serviceResult = await _userService.CreateAsync(TestUserModel);

        // Assert
        var expected = TestUserModel.Id;
        var actual = serviceResult.Id;
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task UserService_CreateAsync_ShouldThrow_InvalidOperationException()
    {
        // Arrange
        _dbContext.Add(_dbUser);
        await _dbContext.SaveChangesAsync();

        // Act
        var serviceResult = _userService.CreateAsync(TestUserModel);
        var exception = await Record.ExceptionAsync(() => serviceResult);

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
        // Id null
        Assert.Contains("The instance of entity type", exception.Message);
    }

    [Fact]
    public async Task UserService_CreateAsync_ShouldReturn_ArgumentException()
    {
        // Arrange1
        TestUserModel.Id = string.Empty;

        // Act1
        var serviceResult = _userService.CreateAsync(TestUserModel);
        var exception = await Record.ExceptionAsync(() => serviceResult);

        // Assert1
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("The id cannot be empty.", exception.Message);
        // Restore
        TestUserModel.Id = "11";

        // ***
        // Arrange2
        TestUserModel.GivenName = string.Empty;

        // Act2
        serviceResult = _userService.CreateAsync(TestUserModel);
        exception = await Record.ExceptionAsync(() => serviceResult);

        // Assert2
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("The given name cannot be empty.", exception.Message);
        // Restore
        TestUserModel.GivenName = "GivenName";

        // ***
        // Arrange3
        TestUserModel.FamilyName = string.Empty;

        // Act3
        serviceResult = _userService.CreateAsync(TestUserModel);
        exception = await Record.ExceptionAsync(() => serviceResult);

        // Assert3
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("The family name cannot be empty.", exception.Message);
        // Restore
        TestUserModel.FamilyName = "FamilyName";

        // ***
        // Arrange4
        TestUserModel.Email = string.Empty;

        // Act4
        serviceResult = _userService.CreateAsync(TestUserModel);
        exception = await Record.ExceptionAsync(() => serviceResult);

        // Assert4
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("The email cannot be empty.", exception.Message);
        // Restore
        TestUserModel.Email = "Email";
    }

    // ***** ***** ***** READ SINGLE
    [Fact]
    public async Task UserService_ReadAsync_ShouldReturn_Ok()
    {
        // Arrange
        _dbContext.Add(_dbUser);
        await _dbContext.SaveChangesAsync();

        // Act
        var serviceResult = await _userService.ReadAsync("11");

        // Assert
        var expected = TestUserModel.Id;
        var actual = serviceResult.Id;
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task UserService_ReadAsync_ShouldReturnNull()
    {
        // Arrange
        // No user

        // Act
        var serviceResult = await _userService.ReadAsync("Unknown");

        // Assert
        Assert.Null(serviceResult);
    }

    // ***** ***** ***** UPDATE
    [Fact]
    public async Task UserService_UpdateAsync_ShouldReturn_Ok()
    {
        // Arrange
        _dbContext.Add(_dbUser);
        await _dbContext.SaveChangesAsync();

        // Act
        var serviceResult = await _userService.UpdateAsync("11", TestUserModel);

        // Assert
        var expected = TestUserModel.Id;
        var actual = serviceResult.Id;
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task UserService_UpdateAsync_ShouldThrow_ArgumentException()
    {
        // Arrange1
        // _userModelTest.Id != input.Id

        // Act1
        var serviceResult = _userService.UpdateAsync("Fake", TestUserModel);
        var exception = await Record.ExceptionAsync(() => serviceResult);

        // Assert1
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("Corrupted data.", exception.Message);

        // ***
        // Arrange2
        TestUserModel.Id = string.Empty;

        // Act2
        serviceResult = _userService.UpdateAsync(string.Empty, TestUserModel);
        exception = await Record.ExceptionAsync(() => serviceResult);

        // Assert2
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("The id cannot be empty.", exception.Message);
        // Restore
        TestUserModel.Id = "11";

        // ***
        // Arrange3
        TestUserModel.GivenName = string.Empty;

        // Act3
        serviceResult = _userService.UpdateAsync(TestUserModel.Id, TestUserModel);
        exception = await Record.ExceptionAsync(() => serviceResult);

        // Assert3
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("The given name cannot be empty.", exception.Message);
        // Restore
        TestUserModel.GivenName = "GivenName";

        // ***
        // Arrange4
        TestUserModel.FamilyName = string.Empty;

        // Act4
        serviceResult = _userService.UpdateAsync(TestUserModel.Id, TestUserModel);
        exception = await Record.ExceptionAsync(() => serviceResult);

        // Assert4
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("The family name cannot be empty.", exception.Message);
        // Restore
        TestUserModel.FamilyName = "FamilyName";

        // ***
        // Arrange5
        TestUserModel.Email = string.Empty;

        // Act5
        serviceResult = _userService.UpdateAsync(TestUserModel.Id, TestUserModel);
        exception = await Record.ExceptionAsync(() => serviceResult);

        // Assert5
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("The email cannot be empty.", exception.Message);
        // Restore
        TestUserModel.Email = "Email";
    }

    // ***** ***** ***** DELETE
    [Fact]
    public async Task UserService_DeleteAsync_ShouldReturn_Ok()
    {
        // Arrange
        _dbContext.Add(_dbUser);
        await _dbContext.SaveChangesAsync();

        // Act
        await _userService.DeleteAsync("11");
        var actual = _dbContext.TUsers.FirstOrDefault(x => x.UsrId == "1");

        // Assert
        Assert.Null(actual);
    }

    [Fact]
    public async Task UserService_DeleteAsync_ShouldThrow_UserNotFound()
    {
        // Arrange
        // No user

        // Act
        var serviceResult = _userService.DeleteAsync("11");
        var exception = await Record.ExceptionAsync(() => serviceResult);

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("Corrupted data.", exception.Message);
    }

    [Fact]
    public async Task UserService_DeleteAsync_ShouldThrow_ArgumentException()
    {
        // Arrange
        // No user

        // Act
        var serviceResult = _userService.DeleteAsync("Unknown");
        var exception = await Record.ExceptionAsync(() => serviceResult);

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("Corrupted data.", exception.Message);
    }
}
