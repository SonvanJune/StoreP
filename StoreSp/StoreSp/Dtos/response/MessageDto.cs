namespace StoreSp.Dtos.response;

public class MessageDto
{
    public required string Id { get; set; }
    public required string Text { get; set; }
    public required UserInBoxChatDto Sender { get; set; }
    public required UserInBoxChatDto Receiver { get; set; }
    public required string CreatedAt { get; set; }
    public required int Status { get; set; }
}
