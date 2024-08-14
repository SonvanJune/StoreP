using System.Net;
using Microsoft.AspNetCore.Mvc;
using StoreSp.Commonds;
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
            if (authService.GetResult(authorization) == 1)
            {
                return BillService!.Checkout(createBillDto);
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
        }).WithParameterValidation().RequireAuthorization("nguoi-mua");
        
        group.MapPost("/" , (GetBillOfUserDto getBillOfUserDto , [FromHeader] string authorization) => 
        {
            if (authService.GetResult(authorization) == 1)
            {
                return BillService!.GetBillByUser(getBillOfUserDto);
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
        }).WithParameterValidation().RequireAuthorization("nguoi-mua");

        group.MapPost("/update-status" , (UpdateBillDto updateBillDto , [FromHeader] string authorization) => 
        {
            if (authService.GetResult(authorization) == 1)
            {
                return BillService!.UpdateBillStatus(updateBillDto);
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

        group.MapGet("/" , ([FromHeader] string authorization) => 
        {
            if (authService.GetResult(authorization) == 1)
            {
                return BillService!.GetBills();
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

        group.MapPost("/re-order" , (ReOrderProductsDto request , [FromHeader] string authorization) => 
        {
            if (authService.GetResult(authorization) == 1)
            {
                return BillService!.ReOrderProducts(request.Code);
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
        }).WithParameterValidation().RequireAuthorization("nguoi-mua");
        return group;
    }
}
