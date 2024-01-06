using Server.Models;

namespace Server.Services.Interfaces;

public interface IStuffService
{
    Task<StuffModel> GetListAsync(int page);
    Task<StuffModel> SearchListAsync(string search);
    Task<DatumModel> CreateAsync(DatumModel input);
    Task<DatumModel> ReadAsync(Guid stuffId);
    Task<DatumModel> UpdateAsync(Guid stuffId, DatumModel input);
    Task DeleteAsync(Guid stuffId);
}
