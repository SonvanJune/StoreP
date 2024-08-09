using Microsoft.AspNetCore.Mvc;
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

        group.MapPost("/", (AddBannerDto addBannerDto ,[FromHeader] string authorization) =>
        {
            return authService.GetResult(authorization, BannerService!.AddBanners(addBannerDto));
        }).WithParameterValidation().RequireAuthorization("quan-tri-vien");

        group.MapPost("/delete", (AddBannerDto addBannerDto ,[FromHeader] string authorization) =>
        {
            return authService.GetResult(authorization, BannerService!.DeleteBanners(addBannerDto));
        }).WithParameterValidation().RequireAuthorization("quan-tri-vien");

        group.MapGet("/" , ([FromHeader] string authorization) => 
        {
            return authService.GetResult(authorization, BannerService!.GetBanners());
        }).WithParameterValidation().RequireAuthorization("quan-tri-vien");

        return group;
    }
}
