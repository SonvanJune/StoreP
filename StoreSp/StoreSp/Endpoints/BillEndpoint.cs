using StoreSp.Dtos.request;
using StoreSp.Services;
using StoreSp.Services.Impl;

namespace StoreSp.Endpoints;

public static class BillEndpoint
{
    public static IBillService? BillService { get; set; }

    public static RouteGroupBuilder MapBillEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/bills");
        BillService = new BillServiceImpl();  

        group.MapPost("/check-out", (CreateBillDto createBillDto) =>
        {
            return BillService!.Checkout(createBillDto);
        }).WithParameterValidation().RequireAuthorization("nguoi-mua");

        return group;
    }
}
