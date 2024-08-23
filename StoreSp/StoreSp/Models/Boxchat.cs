namespace StoreSp.Models;

public class Boxchat
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? Code { get; set; }

    public required int Status { get; set; }

    public ICollection<Message>? Messages  { get; set; }
    public ICollection<User>? Users  { get; set; }
}
