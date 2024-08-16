using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using StoreSp.Services.Sockets;

namespace StoreSp.Endpoints.SocketEndpoint;

public static class ChatSocketEndpoint
{
    public static ChatSocketService? ChatSocketService { get; set; }
    
    
    public static WebApplication MapChatSocketEndpoint(this WebApplication app)
    {
        ChatSocketService = new ChatSocketService();

        app.Map("/ws/chat/{boxchatCode}", ChatSocketService.GetMessageByUserNameSocket);
        return app;
    }
}
