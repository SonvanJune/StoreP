using System.Net;
using Microsoft.AspNetCore.Mvc;
using StoreSp.Commonds;
using StoreSp.Dtos.request;
using StoreSp.Services;
using StoreSp.Services.Impl;

namespace StoreSp.Endpoints;

public static class BannerEndpoint
{
    public static IBannerService? BannerService { get; set; }
    public static IAuthService? authService { get; set; }

    public static RouteGroupBuilder MapBannerEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/banners");
        BannerService = new BannerServiceImpl();
        authService = new AuthServiceImpl();

        group.MapPost("/", (AddBannerDto addBannerDto, [FromHeader] string authorization) =>
        {
            if (authService.GetResult(authorization) == 1)
            {
                return BannerService!.AddBanners(addBannerDto);
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

        group.MapPost("/delete", (AddBannerDto addBannerDto, [FromHeader] string authorization) =>
        {
            if (authService.GetResult(authorization) == 1)
            {
                return BannerService!.DeleteBanners(addBannerDto);
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
                return BannerService!.GetBanners();
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
