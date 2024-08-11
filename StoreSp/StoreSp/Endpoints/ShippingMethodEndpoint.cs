using System.Net;
using Microsoft.AspNetCore.Mvc;
using StoreSp.Commonds;
using StoreSp.Dtos.request;
using StoreSp.Services;
using StoreSp.Services.Impl;

namespace StoreSp.Endpoints;

public static class ShippingMethodEndpoint
{
    public static IShippingMethodService? ShippingMethodService { get; set; }
    public static IAuthService? authService { get; set; }

    public static RouteGroupBuilder MapShippingMehodEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/shippingMethods");
        ShippingMethodService = new ShippingMethodServiceImpl();
        authService = new AuthServiceImpl();

        group.MapPost("/", (AddShippingMethodDto dto , [FromHeader] string authorization) =>
        {
            if (authService.GetResult(authorization) == 1)
            {
                return ShippingMethodService.AddShippingUnit(dto);
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

        group.MapGet("/", ([FromHeader] string authorization) =>
        {
            if (authService.GetResult(authorization) == 1)
            {
                return ShippingMethodService.GetAllShippingMethods();
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
        }).RequireAuthorization();

        return group;
    }
}
