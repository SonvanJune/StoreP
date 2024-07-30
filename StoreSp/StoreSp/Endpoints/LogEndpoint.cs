using StoreSp.Services;
using StoreSp.Services.Impl;

namespace StoreSp.Endpoints;

public static class LogEndpoint
{
    public static ILogService? LogService { get; set; }

    public static RouteGroupBuilder MapLogEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/logs");
        LogService = new LogServiceImpl();

        group.MapPost("/", () =>
        {
            return LogService!.GetLogs();
        }).WithParameterValidation().RequireAuthorization("quan-tri-vien");

        return group;
    }
}
