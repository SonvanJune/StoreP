using Microsoft.AspNetCore.Mvc;
using StoreSp.Dtos.request;
using StoreSp.Services;
using StoreSp.Services.Impl;

namespace StoreSp.Endpoints;

public static class BillEndpoint
{
    public static IBillService? BillService { get; set; }
    public static IAuthService? authService { get; set; }

    public static RouteGroupBuilder MapBillEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/bills");
        BillService = new BillServiceImpl();
        authService = new AuthServiceImpl();  

        group.MapPost("/check-out", (CreateBillDto createBillDto , [FromHeader] string authorization) =>
        {
            return authService.GetResult(authorization, BillService!.Checkout(createBillDto));
        }).WithParameterValidation().RequireAuthorization("nguoi-mua");
        
        group.MapPost("/" , (GetBillOfUserDto getBillOfUserDto , [FromHeader] string authorization) => 
        {
            return authService.GetResult(authorization, BillService!.GetBillByUser(getBillOfUserDto));
        }).WithParameterValidation().RequireAuthorization("nguoi-mua");
        return group;
    }
}
