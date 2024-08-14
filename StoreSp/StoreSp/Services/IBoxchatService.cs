using StoreSp.Dtos.request;

namespace StoreSp.Services;

public interface IBoxchatService
{
    public IResult CreateBoxChat(string usernameSender, string usernameReceiver);
    public IResult GetBoxchats(string username);
    public IResult CreateMessage(CreateMessageDto createMessageDto ,string username);
}
