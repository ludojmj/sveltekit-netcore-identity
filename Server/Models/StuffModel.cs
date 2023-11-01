namespace Server.Models;

public class StuffModel
{
    public int Page { get; set; }
    public int PerPage { get; set; }
    public long Total { get; set; }
    public int TotalPages { get; set; }
    public ICollection<DatumModel> DatumList { get; set; }
}
