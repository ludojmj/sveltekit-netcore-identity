using Server.DbModels;
using Server.Models;

namespace Server.Services;

public static class StuffTranslator
{
    public static DatumModel ToDatumModel(this TStuff input)
    {
        var result = input == null ? null : new DatumModel
        {
            Id = input.StfId,
            Label = input.StfLabel,
            Description = input.StfDescription,
            OtherInfo = input.StfOtherInfo,
            CreatedAt = input.StfCreatedAt.ToUtcDate(),
            UpdatedAt = input.StfUpdatedAt.ToUtcDate(),
            User = input.StfUser.ToUserModel()
        };

        return result;
    }

    public static ICollection<DatumModel> ToDatumListModel(this ICollection<TStuff> input)
    {
        var result = input?.Select(x => x.ToDatumModel()).ToList();
        return result;
    }

    public static StuffModel ToStuffModel(this ICollection<TStuff> input, int page, int dbCount, int totalPages, int itemsPerPage)
    {
        var result = new StuffModel
        {
            Page = page,
            PerPage = itemsPerPage,
            Total = dbCount,
            TotalPages = totalPages,
            DatumList = input.ToDatumListModel()
        };

        return result;
    }

    public static TStuff ToCreate(this DatumModel input)
    {
        string now = DateTime.UtcNow.ToStrDate();
        var result = input == null ? null : new TStuff
        {
            StfId = Guid.NewGuid().ToString(),
            StfLabel = input.Label,
            StfDescription = input.Description,
            StfOtherInfo = input.OtherInfo,
            StfCreatedAt = now,
            StfUpdatedAt = now
        };

        return result;
    }

    public static TStuff ToUpdate(this DatumModel input, TStuff result)
    {
        result.StfId = input.Id;
        result.StfLabel = input.Label;
        result.StfDescription = input.Description;
        result.StfOtherInfo = input.OtherInfo;
        result.StfUpdatedAt = DateTime.UtcNow.ToStrDate();

        return result;
    }
}
