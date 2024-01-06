
namespace Server.Models;

public class DatumModel
{
    private string _label = string.Empty;
    private string description = string.Empty;
    private string otherInfo = string.Empty;

    public Guid Id { get; set; }
    public string Label
    {
        get => _label;
        set => _label = value.Trim();
    }
    public string Description
    {
        get => description ?? string.Empty;
        set => description = value.Trim();
    }
    public string OtherInfo
    {
        get => otherInfo ?? string.Empty;
        set => otherInfo = value.Trim();
    }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public UserModel User { get; set; }
}
