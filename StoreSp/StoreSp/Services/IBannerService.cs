using StoreSp.Dtos.request;

namespace StoreSp.Services;

public interface IBannerService
{
    public IResult AddBanners(AddBannerDto addBannerDto);
    public IResult DeleteBanners(AddBannerDto addBannerDto);
    public IResult GetBanners();
}
