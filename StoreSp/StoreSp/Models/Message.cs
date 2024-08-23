namespace StoreSp.Models;

public class Message
{
    public int Id { get; set; }

    public int SenderId { get; set; }

    public User? Sender { get; set; }

    public int ReceiverId { get; set; }

    public User? Receiver { get; set; }

    public DateTime CreatedAt { get; set; }
    
    public required string Text { get; set; }

    public required int Status { get; set; }

    public int BoxchatId { get; set; }

    public Boxchat? Boxchat { get; set; }
}
