using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Server.DbModels;
using Server.Models;
using Server.Services;
using Server.Services.Interfaces;
using System.Security.Claims;
using Xunit;

namespace Server.UnitTest.Services;

public class TestStuffService
{
    private static readonly UserModel TestUserModel = new()
    {
        Id = "11",
        Name = "GivenName FamilyName",
        GivenName = "GivenName",
        FamilyName = "FamilyName",
        Email = "Email"
    };

    private static readonly DatumModel DatumModelTest = new()
    {
        Id = "1",
        Label = "Label",
        Description = "Description",
        OtherInfo = "OtherInfo",
        User = TestUserModel
    };

    private readonly TUser _dbUser = new()
    {
        UsrId = "11",
        UsrName = "GivenName FamilyName",
        UsrGivenName = "GivenName",
        UsrFamilyName = "FamilyName",
        UsrEmail = "Email"
    };

    private readonly TStuff _dbStuff = new()
    {
        StfId = "1",
        StfUserId = "11",
        StfLabel = "Label",
        StfDescription = "Description",
        StfOtherInfo = "OtherInfo",
        StfCreatedAt = DateTime.UtcNow.ToString("o")
    };

    private readonly StuffDbContext _dbContext;
    private readonly IStuffService _stuffService;

    public TestStuffService()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        var options = new DbContextOptionsBuilder<StuffDbContext>()
            .UseSqlite(connection)
            .Options;
        _dbContext = new StuffDbContext(options);
        _dbContext.Database.EnsureCreated();
        var mockHttpCtx = Mock.Of<IHttpContextAccessor>(x =>
                x.HttpContext!.User.FindFirst(It.IsAny<string>()) == new Claim("name", TestUserModel.Id)
             && x.HttpContext.Request.Path == "/path"
             && x.HttpContext.Request.RouteValues == new RouteValueDictionary("GetList"));
        _stuffService = new StuffService(_dbContext, mockHttpCtx);
    }

    // ***** ***** ***** LIST
    [Fact]
    public async Task StuffService_GetListAsync_ShouldReturn_Ok()
    {
        // Arrange
        _dbContext.Add(_dbUser);
        _dbContext.Add(_dbStuff);
        await _dbContext.SaveChangesAsync();

        // Act
        var serviceResult = await _stuffService.GetListAsync(1);

        // Assert
        var expected = DatumModelTest.Id;
        var actual = serviceResult.DatumList.ToArray()[0].Id;
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task StuffService_GetListAsync_ShouldReturn_PageOne()
    {
        // Arrange
        _dbContext.Add(_dbUser);
        _dbContext.Add(_dbStuff);
        await _dbContext.SaveChangesAsync();

        // Act
        var serviceResult = await _stuffService.GetListAsync(2);

        // Assert
        var actual = serviceResult.Page;
        Assert.Equal(1, actual);
    }

    // ***** ***** ***** SEARCH
    [Fact]
    public async Task StuffService_SearchListAsync_ShouldReturn_Ok()
    {
        // Arrange
        _dbContext.Add(_dbUser);
        _dbContext.Add(_dbStuff);
        await _dbContext.SaveChangesAsync();

        // Act
        var serviceResult = await _stuffService.SearchListAsync("LABEL");

        // Assert
        var expected = DatumModelTest.Id;
        var actual = serviceResult.DatumList.ToArray()[0].Id;
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task StuffService_SearchListAsync_ShouldThrow_ArgumentException()
    {
        // Arrange
        _dbContext.Add(_dbUser);
        var dbStuffList = new List<TStuff>();
        for (int idx = 0; idx < 7; idx++)
        {
            var tmpStuff = new TStuff
            {
                StfId = _dbStuff.StfId + (idx + 1),
                StfUserId = _dbStuff.StfUserId,
                StfLabel = _dbStuff.StfLabel
            };
            dbStuffList.Add(tmpStuff);
        }

        _dbContext.AddRange(dbStuffList);
        await _dbContext.SaveChangesAsync();

        // Act
        var serviceResult = _stuffService.SearchListAsync("LABEL");
        var exception = await Record.ExceptionAsync(() => serviceResult);

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("Too many results. Please narrow your search.", exception.Message);
    }

    // ***** ***** ***** CREATE
    [Fact]
    public async Task StuffService_CreateAsync_ShouldReturn_Ok()
    {
        // Arrange
        // Existing user
        _dbContext.Add(_dbUser);
        await _dbContext.SaveChangesAsync();

        // Act
        var serviceResult = await _stuffService.CreateAsync(DatumModelTest);

        // Assert
        int actual = serviceResult.Id.Count(x => x == '-');
        int expected = 4;
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task StuffService_CreateAsync_Without_User_ShouldThrow_KeyNotFoundException()
    {
        // Arrange
        // No user in DB

        // Act
        var serviceResult = _stuffService.CreateAsync(DatumModelTest);
        var exception = await Record.ExceptionAsync(() => serviceResult);

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<KeyNotFoundException>(exception);
        Assert.Equal("User not found.", exception.Message);
    }

    [Fact]
    public async Task StuffService_CreateAsync_ShouldThrow_ArgumentException()
    {
        // Arrange
        DatumModelTest.Label = string.Empty;
        _dbContext.Add(_dbUser);
        await _dbContext.SaveChangesAsync();

        // Act
        var serviceResult = _stuffService.CreateAsync(DatumModelTest);
        var exception = await Record.ExceptionAsync(() => serviceResult);

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("The label cannot be empty.", exception.Message);

        // Restore
        DatumModelTest.Label = "Label";
    }

    // ***** ***** ***** READ SINGLE
    [Fact]
    public async Task StuffService_ReadAsync_ShouldReturn_Ok()
    {
        // Arrange
        _dbContext.Add(_dbUser);
        _dbContext.Add(_dbStuff);
        await _dbContext.SaveChangesAsync();

        // Act
        var serviceResult = await _stuffService.ReadAsync("1");

        // Assert
        var expected = DatumModelTest.Id;
        var actual = serviceResult.Id;
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task StuffService_ReadAsync_ShouldThrow_KeyNotFoundException()
    {
        // Arrange
        // No stuff

        // Act
        var serviceResult = _stuffService.ReadAsync("2");
        var exception = await Record.ExceptionAsync(() => serviceResult);

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<KeyNotFoundException>(exception);
        Assert.Equal("Stuff not found.", exception.Message);
    }

    // ***** ***** ***** UPDATE
    [Fact]
    public async Task StuffService_UpdateAsync_ShouldReturn_Ok()
    {
        // Arrange
        _dbContext.Add(_dbUser);
        _dbContext.Add(_dbStuff);
        await _dbContext.SaveChangesAsync();

        // Act
        var serviceResult = await _stuffService.UpdateAsync("1", DatumModelTest);

        // Assert
        var expected = DatumModelTest.Id;
        var actual = serviceResult.Id;
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task StuffService_UpdateAsync_ShouldThrow_ArgumentException()
    {
        // Arrange1
        // _datumModelTest.Id != input.Id

        // Act1
        var serviceResult = _stuffService.UpdateAsync("2", DatumModelTest);
        var exception = await Record.ExceptionAsync(() => serviceResult);

        // Assert1
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("Corrupted data.", exception.Message);

        // ***
        // Arrange2
        DatumModelTest.Id = "StuffNotFound";

        // Act2
        serviceResult = _stuffService.UpdateAsync("StuffNotFound", DatumModelTest);
        exception = await Record.ExceptionAsync(() => serviceResult);

        // Assert2
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("Corrupted data.", exception.Message);
        // Restore
        DatumModelTest.Id = "1";

        // ***
        // Arrange3
        DatumModelTest.Label = string.Empty;

        // Act3
        serviceResult = _stuffService.UpdateAsync(DatumModelTest.Id, DatumModelTest);
        exception = await Record.ExceptionAsync(() => serviceResult);

        // Assert3
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("The label cannot be empty.", exception.Message);
        // Restore
        DatumModelTest.Label = "Label";

        // ***
        // Arrange4
        _dbUser.UsrId = "2";
        _dbStuff.StfUserId = "2";
        _dbContext.Add(_dbUser);
        _dbContext.Add(_dbStuff);
        await _dbContext.SaveChangesAsync();
        _dbUser.UsrId = "11";

        // Act4
        serviceResult = _stuffService.UpdateAsync("1", DatumModelTest);
        exception = await Record.ExceptionAsync(() => serviceResult);

        // Assert4
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("Corrupted data.", exception.Message);

        // Restore
        _dbStuff.StfUserId = "11";
        _dbStuff.StfUser.UsrId = "11";
    }

    // ***** ***** ***** DELETE
    [Fact]
    public async Task StuffService_DeleteAsync_ShouldReturn_Ok()
    {
        // Arrange
        _dbContext.Add(_dbUser);
        _dbContext.Add(_dbStuff);
        await _dbContext.SaveChangesAsync();

        // Act
        await _stuffService.DeleteAsync("1");
        var actual = _dbContext.TStuffs.FirstOrDefault(x => x.StfId == "1");

        // Assert
        Assert.Null(actual);
    }

    [Fact]
    public async Task StuffService_DeleteAsync_ShouldThrow_ArgumentException()
    {
        // Arrange1
        // No stuff

        // Act1
        var serviceResult = _stuffService.DeleteAsync("2");
        var exception = await Record.ExceptionAsync(() => serviceResult);

        // Assert1
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("Corrupted data.", exception.Message);

        // ***
        // Arrange2
        _dbUser.UsrId = "2";
        _dbStuff.StfUserId = "2";
        _dbContext.Add(_dbUser);
        _dbContext.Add(_dbStuff);
        await _dbContext.SaveChangesAsync();
        _dbUser.UsrId = "11";

        // Act2
        serviceResult = _stuffService.DeleteAsync("1");
        exception = await Record.ExceptionAsync(() => serviceResult);

        // Assert2
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("Corrupted data.", exception.Message);

        // Restore
        _dbStuff.StfUserId = "11";
        _dbStuff.StfUser.UsrId = "11";
    }
}
