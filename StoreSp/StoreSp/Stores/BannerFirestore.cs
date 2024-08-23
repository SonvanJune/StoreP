using Microsoft.EntityFrameworkCore;
using StoreSp.Context;
using StoreSp.Dtos.request;
using StoreSp.Dtos.response;
using StoreSp.Models;

namespace StoreSp.Stores;

public class BannerFirestore
{
    private readonly AppDbContext? _appDbContext = null;

    public BannerFirestore()
    {
        _appDbContext = AppDbContext.GetInstance();
    }

    public async Task<string> AddBanner(AddBannerDto addBannerDto)
    {
        foreach (var item in addBannerDto.Images)
        {
            Banner banner = new Banner
            {
                CreatedAt = DateTime.Now,
                Name = item
            };
            _appDbContext?.Banners.Add(banner);
        }
        await _appDbContext!.SaveChangesAsync();
        return "success";
    }

    public async Task<List<BannerDto>> GetBanners()
    {
        var banners = await _appDbContext!.Banners.ToListAsync();
        List<BannerDto> result = new List<BannerDto>();

        foreach (var item in banners)
        {
            var banner = new BannerDto
            {
                Id = item.Id,
                CreatedAt = item.CreatedAt.ToString(),
                Name = item.Name
            };
            result.Add(banner);
        }
        return result;
    }

    public async Task<string> DeleteBanner(DeleteBannerDto deleteBannerDto)
    {
        foreach (var item in deleteBannerDto.Ids)
        {
            var banner = _appDbContext!.Banners.FirstOrDefault(b => b.Id == item);
            if (banner != null)
            {
                _appDbContext.Banners.Remove(banner);
                await _appDbContext.SaveChangesAsync();
            }
        }
        return "success";
    }
}
