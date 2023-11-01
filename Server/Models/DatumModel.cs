
namespace Server.Models;

public class DatumModel
{
    public string Id { get; set; }
    public string Label { get; set; }
    public string Description { get; set; }
    public string OtherInfo { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public UserModel User { get; set; }
}
