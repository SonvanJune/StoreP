namespace StoreSp.Dtos.response;

public class BoxchatDto
{
    public required UserInBoxChatDto Sender{ get; set; }
    public required string LastMessage { get; set; }
    public required int CountMessNotRead { get; set; }
    public required string Code { get; set; }
}
