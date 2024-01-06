
namespace Server.Models;

public class DatumModel
{
    private string _label = string.Empty;
    private string _description = string.Empty;
    private string _otherInfo = string.Empty;

    public Guid Id { get; set; }
    public string Label
    {
        get => _label;
        set => _label = value.Trim();
    }
    public string Description
    {
        get => _description ?? string.Empty;
        set => _description = value.Trim();
    }
    public string OtherInfo
    {
        get => _otherInfo ?? string.Empty;
        set => _otherInfo = value.Trim();
    }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public UserModel User { get; set; }
}
