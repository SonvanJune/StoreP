namespace StoreSp.Models;

public class Banner
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public required string Name { get; set; }
}
