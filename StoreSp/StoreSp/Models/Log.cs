namespace StoreSp.Models;

public class Log
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public int UserId { get; set; }

    public User? User { get; set; }

    public int Status { get; set; }

    public string? Code { get; set; }

    public string? Message { get; set; }
}
