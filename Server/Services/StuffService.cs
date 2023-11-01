using Microsoft.EntityFrameworkCore;
using Server.DbModels;
using Server.Models;
using Server.Services.Interfaces;
using Server.Shared;

namespace Server.Services;

public class StuffService : IStuffService
{
    private const int CstItemsPerPage = 6;
    private readonly StuffDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContext;

    public StuffService(StuffDbContext context, IHttpContextAccessor httpContext)
    {
        _dbContext = context;
        _httpContext = httpContext;
    }

    public async Task<StuffModel> GetListAsync(int page)
    {
        if (page == 0)
        {
            page = 1;
        }

        int dbCount = await _dbContext.TStuffs.CountAsync();
        int totalPages = ((dbCount - 1) / CstItemsPerPage) + 1;
        if (dbCount == 0 || page > totalPages)
        {
            page = 1;
        }

        ICollection<TStuff> dbStuffList = await _dbContext.TStuffs.AsQueryable()
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
        IQueryable<TStuff> query = _dbContext.TStuffs.Include(x => x.StfUser).Where(x =>
            EF.Functions.Like(x.StfLabel, $"%{search}%")
            || EF.Functions.Like(x.StfDescription, $"%{search}%")
            || EF.Functions.Like(x.StfOtherInfo, $"%{search}%")
            || EF.Functions.Like(x.StfUser.UsrGivenName, $"%{search}%")
            || EF.Functions.Like(x.StfUser.UsrFamilyName, $"%{search}%")
        );

        int dbCount = await query.CountAsync();
        if (dbCount > CstItemsPerPage)
        {
            throw new ArgumentException("Too many results. Please narrow your search.");
        }

        // Get stuff and their users.
        ICollection<TStuff> dbStuffList = await query.Include(x => x.StfUser).ToListAsync();
        var result = dbStuffList.ToStuffModel(1, dbCount, 1, CstItemsPerPage);
        return result;
    }

    public async Task<DatumModel> CreateAsync(DatumModel input)
    {
        input.CheckDatum();
        TStuff dbStuff = input.ToCreate();
        UserModel userAuth = _httpContext.HttpContext.GetCurrentUser();
        TUser dbUser = await _dbContext.TUsers.FirstOrDefaultAsync(x => x.UsrId == userAuth.Id);
        if (dbUser == null)
        {
            throw new KeyNotFoundException("User not found.");
        }

        // Attach foreign key
        dbStuff.StfUserId = dbUser.UsrId;
        // Create stuff
        await _dbContext.TStuffs.AddAsync(dbStuff);
        await _dbContext.SaveChangesAsync();
        // Attach user to the stuff for the response
        dbStuff.StfUser = dbUser;
        var result = dbStuff.ToDatumModel();
        return result;
    }

    public async Task<DatumModel> ReadAsync(string stuffId)
    {
        // Get the stuff and its user
        TStuff dbStuff = await _dbContext.TStuffs
            .Where(x => x.StfId == stuffId)
            .Include(x => x.StfUser)
            .FirstOrDefaultAsync();
        if (dbStuff == null)
        {
            throw new KeyNotFoundException("Stuff not found.");
        }

        var result = dbStuff.ToDatumModel();
        return result;
    }

    public async Task<DatumModel> UpdateAsync(string stuffId, DatumModel input)
    {
        input.CheckDatum();
        if (stuffId != input.Id)
        {
            throw new ArgumentException("Corrupted data.");
        }

        UserModel userAuth = _httpContext.HttpContext.GetCurrentUser();
        TUser dbUser = await _dbContext.TUsers.FirstOrDefaultAsync(x => x.UsrId == userAuth.Id);
        TStuff dbStuff = await _dbContext.TStuffs.FirstOrDefaultAsync(x => x.StfId == stuffId);
        if (dbStuff == null || dbStuff.StfUserId != userAuth.Id)
        {
            throw new ArgumentException("Corrupted data.");
        }

        // Update stuff
        dbStuff = input.ToUpdate(dbStuff);
        await _dbContext.SaveChangesAsync();
        // Attach user to the stuff for the response
        dbStuff.StfUser = dbUser;
        var result = dbStuff.ToDatumModel();
        return result;
    }

    public async Task DeleteAsync(string stuffId)
    {
        UserModel userAuth = _httpContext.HttpContext.GetCurrentUser();
        TStuff dbStuff = await _dbContext.TStuffs.FirstOrDefaultAsync(x => x.StfId == stuffId);
        if (dbStuff == null || dbStuff.StfUserId != userAuth.Id)
        {
            throw new ArgumentException("Corrupted data.");
        }

        _dbContext.TStuffs.Remove(dbStuff);
        await _dbContext.SaveChangesAsync();
    }
}
