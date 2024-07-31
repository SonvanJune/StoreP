using StoreSp.Dtos.request;
using StoreSp.Services;
using StoreSp.Services.Impl;

namespace StoreSp.Endpoints;

public static class ShippingMethodEndpoint
{
    public static IShippingMethodService? ShippingMethodService { get; set; }

    public static RouteGroupBuilder MapShippingMehodEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/shippingMethods");
        ShippingMethodService = new ShippingMethodServiceImpl();

        group.MapPost("/", (AddShippingMethodDto dto) =>
        {
            return ShippingMethodService.AddShippingUnit(dto);
        }).WithParameterValidation().RequireAuthorization("quan-tri-vien");;

        group.MapGet("/", () =>
        {
            return ShippingMethodService.GetAllShippingMethods();
        }).RequireAuthorization();

        return group;
    }
}
