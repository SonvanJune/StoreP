
using FirebaseAdmin.Messaging;

namespace StoreSp.Stores;

public class FcmService
{
    public async Task SendNotificationAsync(string token, string title, string body)
    {
        var message = new Message()
        {
            Token = token,
            Notification = new Notification
            {
                Title = title,
                Body = body,
            },
        };

        // Send the message
        string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
    }
}
