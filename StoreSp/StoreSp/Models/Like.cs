namespace StoreSp.Models;

public class Like
{
    public DateTime CreatedAt { get; set; }

    public int UserId { get; set; }

    public User? User { get; set; }

    public int ProductId { get; set; }

    public Product? Product { get; set; }
}
