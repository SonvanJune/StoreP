using StoreSp.Services;
using StoreSp.Services.Impl;

namespace StoreSp.Endpoints;

public static class UploadEndpoint
{
    public static IUploadService? UploadService { get; set; }

    public static RouteGroupBuilder MapUploadEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/uploads");
        UploadService = new UploadServiceImpl();

        
        return group;
    }
}
