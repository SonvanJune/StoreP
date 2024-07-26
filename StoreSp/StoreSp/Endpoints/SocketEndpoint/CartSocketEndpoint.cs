using StoreSp.Services.Sockets;

namespace StoreSp.Endpoints.SocketEndpoint;

public static class CartSocketEndpoint
{
    public static CartSocketService? CartSocketService {get; set; }
    public static WebApplication MapCartSocketEndpoint(this WebApplication app)
    {
        CartSocketService = new CartSocketService();
        
        app.Map("/ws", CartSocketService.GetCartByUserNameSocket);

        return app;
    }
}
