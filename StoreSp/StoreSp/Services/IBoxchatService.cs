namespace StoreSp.Services;

public interface IBoxchatService
{
    public IResult CreateBoxChat(string usernameSender, string usernameReceiver);
}
