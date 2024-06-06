using Microsoft.EntityFrameworkCore;
using Server.DbModels;
using Server.Models;
using Server.Services.Interfaces;
using Server.Shared;

namespace Server.Services;

public class StuffService(StuffDbContext dbContext, IHttpContextAccessor httpContext) : IStuffService
{
    private const int CstItemsPerPage = 6;

    public async Task<StuffModel> GetListAsync(int page)
    {
        if (page == 0)
        {
            page = 1;
        }

        int dbCount = await dbContext.TStuffs.CountAsync();
        int totalPages = ((dbCount - 1) / CstItemsPerPage) + 1;
        if (dbCount == 0 || page > totalPages)
        {
            page = 1;
        }

        ICollection<TStuff> dbStuffList = await dbContext.TStuffs.AsQueryable()
            .OrderByDescending(x => x.StfUpdatedAt)
            .ThenByDescending(x => x.StfCreatedAt)
            .Skip(CstItemsPerPage * (page - 1))
            .Take(CstItemsPerPage)
            .Include(x => x.StfUser)
            .ToListAsync();
        var result = dbStuffList.ToStuffModel(page, dbCount, totalPages, CstItemsPerPage);
        return result;
    }

    public async Task<StuffModel> SearchListAsync(string search)
    {
        IQueryable<TStuff> query = dbContext.TStuffs.Include(x => x.StfUser).Where(x =>
            EF.Functions.Like(x.StfLabel, $"%{search}%")
            || EF.Functions.Like(x.StfDescription, $"%{search}%")
            || EF.Functions.Like(x.StfOtherInfo, $"%{search}%")
            || EF.Functions.Like(x.StfUser.UsrGivenName, $"%{search}%")
            || EF.Functions.Like(x.StfUser.UsrFamilyName, $"%{search}%")
        );

        int dbCount = await query.CountAsync();
        if (dbCount > CstItemsPerPage)
        {
            throw new BusinessException("Too many results. Please narrow your search.");
        }

        // Get stuff and their users.
        ICollection<TStuff> dbStuffList = await query.Include(x => x.StfUser).ToListAsync();
        var result = dbStuffList.ToStuffModel(1, dbCount, 1, CstItemsPerPage);
        return result;
    }

    public async Task<DatumModel> CreateAsync(DatumModel input)
    {
        input.CheckDatum(input.Id);
        TStuff dbStuff = input.ToCreate();
        UserModel userAuth = httpContext.HttpContext.GetCurrentUser();
        TUser dbUser = await dbContext.TUsers.FirstOrDefaultAsync(x => x.UsrId == userAuth.Id)
            ?? throw new KeyNotFoundException("User not found.");

        // Attach foreign key
        dbStuff.StfUserId = dbUser.UsrId;
        // Create stuff
        await dbContext.TStuffs.AddAsync(dbStuff);
        await dbContext.SaveChangesAsync();
        // Attach user to the stuff for the response
        dbStuff.StfUser = dbUser;
        var result = dbStuff.ToDatumModel();
        return result;
    }

    public async Task<DatumModel> ReadAsync(Guid stuffId)
    {
        // Get the stuff and its user
        TStuff dbStuff = await dbContext.TStuffs
            .Where(x => x.StfId == stuffId.ToString())
            .Include(x => x.StfUser)
            .FirstOrDefaultAsync() ?? throw new KeyNotFoundException("Stuff not found.");
        return dbStuff.ToDatumModel();
    }

    public async Task<DatumModel> UpdateAsync(Guid stuffId, DatumModel input)
    {
        input.CheckDatum(stuffId);
        UserModel userAuth = httpContext.HttpContext.GetCurrentUser();
        TUser dbUser = await dbContext.TUsers.FirstOrDefaultAsync(x => x.UsrId == userAuth.Id);
        TStuff dbStuff = await dbContext.TStuffs.FirstOrDefaultAsync(x => x.StfId == stuffId.ToString());
        if (dbStuff == null || dbStuff.StfUserId != userAuth.Id)
        {
            throw new ArgumentException("Corrupted data.");
        }

        // Update stuff
        dbStuff = input.ToUpdate(dbStuff);
        await dbContext.SaveChangesAsync();
        // Attach user to the stuff for the response
        dbStuff.StfUser = dbUser;
        var result = dbStuff.ToDatumModel();
        return result;
    }

    public async Task DeleteAsync(Guid stuffId)
    {
        UserModel userAuth = httpContext.HttpContext.GetCurrentUser();
        TStuff dbStuff = await dbContext.TStuffs.FirstOrDefaultAsync(x => x.StfId == stuffId.ToString());
        if (dbStuff == null || dbStuff.StfUserId != userAuth.Id)
        {
            throw new ArgumentException("Corrupted data.");
        }

        dbContext.TStuffs.Remove(dbStuff);
        await dbContext.SaveChangesAsync();
    }
}
