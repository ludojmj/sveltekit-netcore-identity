using Microsoft.EntityFrameworkCore;
using Server.DbModels;
using Server.Models;
using Server.Services.Interfaces;

namespace Server.Services;

public class UserService : IUserService
{
    private const int CstItemsPerPage = 6;
    private readonly StuffDbContext _dbContext;

    public UserService(StuffDbContext context)
    {
        _dbContext = context;
    }

    public async Task<DirectoryModel> GetListAsync(int page)
    {
        if (page == 0)
        {
            page = 1;
        }

        int dbCount = await _dbContext.TUsers.CountAsync();
        int totalPages = ((dbCount - 1) / CstItemsPerPage) + 1;
        if (dbCount == 0 || page > totalPages)
        {
            page = 1;
            totalPages = 1;
        }

        ICollection<TUser> dbUserList = await _dbContext.TUsers
            .OrderByDescending(x => x.UsrUpdatedAt)
            .ThenByDescending(x => x.UsrCreatedAt)
            .Skip(CstItemsPerPage * (page - 1))
            .Take(CstItemsPerPage)
            .ToListAsync();
        var result = dbUserList.ToDirectoryModel(page, dbCount, totalPages, CstItemsPerPage);
        return result;
    }

    public async Task<DirectoryModel> SearchListAsync(string search)
    {
        IQueryable<TUser> query = _dbContext.TUsers.Where(x =>
            EF.Functions.Like(x.UsrGivenName, $"%{search}%")
            || EF.Functions.Like(x.UsrFamilyName, $"%{search}%")
        );

        int dbCount = await query.CountAsync();
        if (dbCount > CstItemsPerPage)
        {
            throw new ArgumentException("Too many results. Please narrow your search.");
        }

        List<TUser> dbUserList = await query.ToListAsync();
        var result = dbUserList.ToDirectoryModel(1, dbCount, 1, CstItemsPerPage);
        return result;
    }

    public async Task<UserModel> CreateAsync(UserModel input)
    {
        input.CheckUser();
        TUser dbUser = input.ToCreate();
        await _dbContext.TUsers.AddAsync(dbUser);
        await _dbContext.SaveChangesAsync();
        var result = dbUser.ToUserModel();
        return result;
    }

    public async Task<UserModel> ReadAsync(string userId)
    {
        TUser dbUser = await _dbContext.TUsers.FirstOrDefaultAsync(x => x.UsrId == userId);
        var result = dbUser.ToUserModel();
        return result;
    }

    public async Task<UserModel> UpdateAsync(string userId, UserModel input)
    {
        input.CheckUser();

        if (userId != input.Id)
        {
            throw new ArgumentException("Corrupted data.");
        }

        TUser dbUser = await _dbContext.TUsers.FirstOrDefaultAsync(x => x.UsrId == userId);
        if (dbUser == null)
        {
            throw new ArgumentException("Corrupted data.");
        }

        dbUser = input.ToUpdate(dbUser);
        await _dbContext.SaveChangesAsync();
        var result = dbUser.ToUserModel();
        return result;
    }

    public async Task DeleteAsync(string userId)
    {
        TUser dbUser = await _dbContext.TUsers.FirstOrDefaultAsync(x => x.UsrId == userId);
        if (dbUser == null)
        {
            throw new ArgumentException("Corrupted data.");
        }

        _dbContext.TUsers.Remove(dbUser);
        await _dbContext.SaveChangesAsync();
    }
}
