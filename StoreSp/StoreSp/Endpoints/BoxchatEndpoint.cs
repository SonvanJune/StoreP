using System.Net;
using Microsoft.AspNetCore.Mvc;
using StoreSp.Commonds;
using StoreSp.Dtos.request;
using StoreSp.Services;
using StoreSp.Services.Impl;

namespace StoreSp.Endpoints;

public static class BoxchatEndpoint
{
    public static IBoxchatService? BoxchatService { get; set; }
    public static IAuthService? authService { get; set; }

    public static RouteGroupBuilder MapBoxchatEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/boxchats");
        BoxchatService = new BoxchatServiceImpl();
        authService = new AuthServiceImpl();

        group.MapPost("/create", (CreateBoxchatDto createBoxchatDto,[FromHeader] string authorization) =>
        {
            if (authService.GetResult(authorization) == 1)
            {
                string[] str = authorization.Split(' ');
                var username = authService.GetFirstByToken(str[1]);
                return BoxchatService!.CreateBoxChat( createBoxchatDto.Username , username);
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
        }).WithParameterValidation().RequireAuthorization();

        return group;
    }
}
