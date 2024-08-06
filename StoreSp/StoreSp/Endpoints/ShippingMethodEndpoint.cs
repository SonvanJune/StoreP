using Microsoft.AspNetCore.Mvc;
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
            return authService.GetResult(authorization, ShippingMethodService.AddShippingUnit(dto));
        }).WithParameterValidation().RequireAuthorization("quan-tri-vien");;

        group.MapGet("/", ([FromHeader] string authorization) =>
        {
            return authService.GetResult(authorization, ShippingMethodService.GetAllShippingMethods());
        }).RequireAuthorization();

        return group;
    }
}
