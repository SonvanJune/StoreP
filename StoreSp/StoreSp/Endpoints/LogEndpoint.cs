using Microsoft.AspNetCore.Mvc;
using StoreSp.Services;
using StoreSp.Services.Impl;

namespace StoreSp.Endpoints;

public static class LogEndpoint
{
    public static ILogService? LogService { get; set; }
    public static IAuthService? authService { get; set; }

    public static RouteGroupBuilder MapLogEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/logs");
        LogService = new LogServiceImpl();
        authService = new AuthServiceImpl();

        group.MapPost("/", ([FromHeader] string authorization) =>
        {
            return authService.GetResult(authorization, LogService!.GetLogs());
        }).WithParameterValidation().RequireAuthorization("quan-tri-vien");

        return group;
    }
}
