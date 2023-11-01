using System.Text.Json;

namespace Server.Models;

public class UserModel
{
    public string Operation { get; set; }
    public string AppId { get; set; }
    public string Id { get; set; }
    public string Name { get; set; }
    public string GivenName { get; set; }
    public string FamilyName { get; set; }
    public string Email { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
    }
}
