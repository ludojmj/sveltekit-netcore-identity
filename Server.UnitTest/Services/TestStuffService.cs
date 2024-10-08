using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Server.DbModels;
using Server.Models;
using Server.Services;
using Server.Shared;
using System.Globalization;
using System.Security.Claims;
using Xunit;

namespace Server.UnitTest.Services;

public class TestStuffService
{
    private readonly StuffDbContext _dbContext;
    private readonly StuffService _stuffService;

    private static readonly Guid IdTest = Guid.NewGuid();

    private static readonly UserModel TestUserModel = new()
    {
        Id = "11",
        Name = "GivenName FamilyName",
        GivenName = "GivenName",
        FamilyName = "FamilyName",
        Email = "Email",
        Ip = "127.0.0.1"
    };

    private static readonly DatumModel DatumModelTest = new()
    {
        Id = IdTest,
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
        StfId = IdTest.ToString(),
        StfUserId = "11",
        StfLabel = "Label",
        StfDescription = "Description",
        StfOtherInfo = "OtherInfo",
        StfCreatedAt = DateTime.UtcNow.ToString("o")
    };

    public TestStuffService()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        var options = new DbContextOptionsBuilder<StuffDbContext>()
            .UseSqlite(connection)
            .Options;
        _dbContext = new StuffDbContext(options);
        _dbContext.Database.EnsureCreated();
        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, TestUserModel.Id)
            ]))
        };
        var httpContext = Mock.Of<IHttpContextAccessor>(x => x.HttpContext == context);
        _stuffService = new StuffService(_dbContext, httpContext);
    }

    // ***** ***** ***** LIST
    [Fact]
    public async Task StuffService_GetListAsync_ShouldReturn_Ok()
    {
        // Arrange
        await _dbContext.AddAsync(_dbUser);
        await _dbContext.AddAsync(_dbStuff);
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
        await _dbContext.AddAsync(_dbUser);
        await _dbContext.AddAsync(_dbStuff);
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
        await _dbContext.AddAsync(_dbUser);
        await _dbContext.AddAsync(_dbStuff);
        await _dbContext.SaveChangesAsync();

        // Act
        var serviceResult = await _stuffService.SearchListAsync("LABEL");

        // Assert
        var expected = DatumModelTest.Id;
        var actual = serviceResult.DatumList.ToArray()[0].Id;
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task StuffService_SearchListAsync_ShouldThrow_BusinessException()
    {
        // Arrange
        await _dbContext.AddAsync(_dbUser);
        var dbStuffList = new List<TStuff>();
        for (int idx = 0; idx < 7; idx++)
        {
            var tmpStuff = new TStuff
            {
                StfId = _dbStuff.StfId + (idx + 1).ToString(CultureInfo.InvariantCulture),
                StfUserId = _dbStuff.StfUserId,
                StfLabel = _dbStuff.StfLabel
            };
            dbStuffList.Add(tmpStuff);
        }

        await _dbContext.AddRangeAsync(dbStuffList);
        await _dbContext.SaveChangesAsync();

        // Act
        var serviceResult = _stuffService.SearchListAsync("LABEL");
        var exception = await Record.ExceptionAsync(() => serviceResult);

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<BusinessException>(exception);
        Assert.Equal("Too many results. Please narrow your search.", exception.Message);
    }

    // ***** ***** ***** CREATE
    [Fact]
    public async Task StuffService_CreateAsync_ShouldReturn_Ok()
    {
        // Arrange
        // Existing user
        await _dbContext.AddAsync(_dbUser);
        await _dbContext.SaveChangesAsync();

        // Act
        var serviceResult = await _stuffService.CreateAsync(DatumModelTest);

        // Assert
        var expected = DatumModelTest.Label;
        var actual = serviceResult.Label;
        Assert.Equal(expected, actual);
        Assert.NotEqual(DatumModelTest.Id, serviceResult.Id);
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
        await _dbContext.AddAsync(_dbUser);
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
        await _dbContext.AddAsync(_dbUser);
        await _dbContext.AddAsync(_dbStuff);
        await _dbContext.SaveChangesAsync();

        // Act
        var serviceResult = await _stuffService.ReadAsync(DatumModelTest.Id);

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
        var serviceResult = _stuffService.ReadAsync(Guid.NewGuid());
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
        await _dbContext.AddAsync(_dbUser);
        await _dbContext.AddAsync(_dbStuff);
        await _dbContext.SaveChangesAsync();

        // Act
        var serviceResult = await _stuffService.UpdateAsync(DatumModelTest.Id, DatumModelTest);

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
        var serviceResult = _stuffService.UpdateAsync(Guid.NewGuid(), DatumModelTest);
        var exception = await Record.ExceptionAsync(() => serviceResult);

        // Assert1
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("Corrupted data.", exception.Message);

        // ***
        // Arrange2
        DatumModelTest.Id = Guid.NewGuid();

        // Act2
        serviceResult = _stuffService.UpdateAsync(Guid.NewGuid(), DatumModelTest);
        exception = await Record.ExceptionAsync(() => serviceResult);

        // Assert2
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("Corrupted data.", exception.Message);
        // Restore
        DatumModelTest.Id = IdTest;

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
        await _dbContext.AddAsync(_dbUser);
        await _dbContext.AddAsync(_dbStuff);
        await _dbContext.SaveChangesAsync();
        _dbUser.UsrId = "11";

        // Act4
        serviceResult = _stuffService.UpdateAsync(Guid.NewGuid(), DatumModelTest);
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
        await _dbContext.AddAsync(_dbUser);
        await _dbContext.AddAsync(_dbStuff);
        await _dbContext.SaveChangesAsync();

        // Act
        await _stuffService.DeleteAsync(IdTest);
        var actual = await _dbContext.TStuffs.FirstOrDefaultAsync(x => x.StfId == "1");

        // Assert
        Assert.Null(actual);
    }

    [Fact]
    public async Task StuffService_DeleteAsync_ShouldThrow_ArgumentException()
    {
        // Arrange1
        // No stuff

        // Act1
        var serviceResult = _stuffService.DeleteAsync(Guid.NewGuid());
        var exception = await Record.ExceptionAsync(() => serviceResult);

        // Assert1
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("Corrupted data.", exception.Message);

        // ***
        // Arrange2
        _dbUser.UsrId = "2";
        _dbStuff.StfUserId = "2";
        await _dbContext.AddAsync(_dbUser);
        await _dbContext.AddAsync(_dbStuff);
        await _dbContext.SaveChangesAsync();
        _dbUser.UsrId = "11";

        // Act2
        serviceResult = _stuffService.DeleteAsync(IdTest);
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
