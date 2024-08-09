using System.Net;
using StoreSp.Commonds;
using StoreSp.Dtos.request;
using StoreSp.Stores;

namespace StoreSp.Services.Impl;

public class BannerServiceImpl : IBannerService
{
    public static BannerFirestore? BannerFirestore { get; set; }
    IResult IBannerService.AddBanners(AddBannerDto addBannerDto)
    {
        var banners = BannerFirestore!.AddBanner(addBannerDto).Result;
        return Results.Ok(new HttpStatusConfig
        {
            status = HttpStatusCode.OK,
            message = "success",
            data = banners
        });
    }

    IResult IBannerService.GetBanners()
    {
        return Results.Ok(new HttpStatusConfig
        {
            status = HttpStatusCode.OK,
            message = "success",
            data = BannerFirestore!.GetBanners()
        });
    }
}
