using System.Net;
using Microsoft.AspNetCore.Mvc;
using StoreSp.Commonds;
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
            if (authService.GetResult(authorization) == 1)
            {
                return LogService!.GetLogs();
            }
            else
            {
                return Results.BadRequest(new HttpStatusConfig
                {
                    status = HttpStatusCode.BadRequest,
                    message = "Token has expired",
                    data = null
                });
            }
        }).WithParameterValidation().RequireAuthorization("quan-tri-vien");

        return group;
    }
}
