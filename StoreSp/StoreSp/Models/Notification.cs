namespace StoreSp.Models;

public class Notification
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public int UserId { get; set; }

    public User? User { get; set; }

    public int Status { get; set; }

    public int Type { get; set; }

    public string? Message { get; set; }
}
