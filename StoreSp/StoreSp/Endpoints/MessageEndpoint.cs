using System.Net;
using Microsoft.AspNetCore.Mvc;
using StoreSp.Commonds;
using StoreSp.Dtos.request;
using StoreSp.Services;
using StoreSp.Services.Impl;

namespace StoreSp.Endpoints.SocketEndpoint;

public static class MessageEndpoint
{
    public static IBoxchatService? BoxchatService { get; set; }
    public static IAuthService? authService { get; set; }

    public static RouteGroupBuilder MapMessageEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/messages");
        BoxchatService = new BoxchatServiceImpl();
        authService = new AuthServiceImpl();
        
        group.MapPost("/create", (CreateMessageDto createMessageDto,[FromHeader] string authorization) =>
        {
            if (authService.GetResult(authorization) == 1)
            {
                string[] str = authorization.Split(' ');
                var username = authService.GetFirstByToken(str[1]);
                return BoxchatService!.CreateMessage( createMessageDto, username);
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

        group.MapGet("/get/{boxchatCode}", (string boxchatCode ,[FromHeader] string authorization) =>
        {
            if (authService.GetResult(authorization) == 1)
            {
                return BoxchatService!.GetMessages(boxchatCode);
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
