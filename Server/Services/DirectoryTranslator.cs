using Server.DbModels;
using Server.Models;

namespace Server.Services;

public static class DirectoryTranslator
{
    public static UserModel ToUserModel(this TUser input)
    {
        var result = input == null ? null : new UserModel
        {
            Id = input.UsrId,
            Name = input.UsrName,
            GivenName = input.UsrGivenName,
            FamilyName = input.UsrFamilyName,
            Email = input.UsrEmail,
            CreatedAt = input.UsrCreatedAt.ToUtcDate(),
            UpdatedAt = input.UsrUpdatedAt.ToUtcDate()
        };

        return result;
    }

    public static ICollection<UserModel> ToUserListModel(this ICollection<TUser> input)
    {
        var result = input?.Select(x => x.ToUserModel()).ToList();
        return result;
    }

    public static DirectoryModel ToDirectoryModel(this ICollection<TUser> input, int page, int dbCount, int totalPages, int itemsPerPage)
    {
        var result = new DirectoryModel
        {
            Page = page,
            PerPage = itemsPerPage,
            Total = dbCount,
            TotalPages = totalPages,
            UserList = input.ToUserListModel()
        };

        return result;
    }

    public static TUser ToCreate(this UserModel input)
    {
        var result = input == null ? null : new TUser
        {
            UsrId = input.Id,
            UsrName = $"{input.GivenName} {input.FamilyName}",
            UsrGivenName = input.GivenName,
            UsrFamilyName = input.FamilyName,
            UsrEmail = input.Email,
            UsrCreatedAt = DateTime.UtcNow.ToStrDate()
        };

        return result;
    }

    public static TUser ToUpdate(this UserModel input, TUser result)
    {
        result.UsrId = input.Id;
        result.UsrName = $"{input.GivenName} {input.FamilyName}";
        result.UsrGivenName = input.GivenName;
        result.UsrFamilyName = input.FamilyName;
        result.UsrEmail = input.Email;
        result.UsrUpdatedAt = DateTime.UtcNow.ToStrDate();
        return result;
    }
}
