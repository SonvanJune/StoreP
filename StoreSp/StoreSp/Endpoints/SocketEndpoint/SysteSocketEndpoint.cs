using StoreSp.Services.Sockets;

namespace StoreSp.Endpoints.SocketEndpoint;

public static class SysteSocketEndpoint
{
    public static SystemSocketService? SystemSocketService {get; set; }
    public static WebApplication MapCartSocketEndpoint(this WebApplication app)
    {
        SystemSocketService = new SystemSocketService();
        
        app.Map("/ws/system", SystemSocketService.GetByUserNameSocket);

        return app;
    }
}
