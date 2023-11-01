using Server.Models;

namespace Server.Services.Interfaces;

public interface IUserService
{
    Task<DirectoryModel> GetListAsync(int page);
    Task<DirectoryModel> SearchListAsync(string search);
    Task<UserModel> CreateAsync(UserModel input);
    Task<UserModel> ReadAsync(string userId);
    Task<UserModel> UpdateAsync(string userId, UserModel input);
    Task DeleteAsync(string userId);
}
