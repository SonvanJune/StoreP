using Google.Cloud.Firestore;
using StoreSp.Dtos.request;
using StoreSp.Dtos.response;
using StoreSp.Entities;

namespace StoreSp.Stores;

public class BannerFirestore(FirestoreDb firestoreDb) : FirestoreService(firestoreDb)
{
    public static string _collectionBanner = "Banners";

    public Task<string> AddBanner(AddBannerDto addBannerDto)
    {
        var db = _firestoreDb.Collection(_collectionBanner);
        foreach (var item in addBannerDto.Images)
        {
            var banner = new Banner
            {
                Name = item,
                CreatedAt = Timestamp.FromDateTime(DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc))
            };
            db.AddAsync(banner);
        }
        return Task.FromResult("success");
    }

    public List<BannerDto> GetBanners()
    {
        var bannerDb = base.GetSnapshots(_collectionBanner);
        var banners = bannerDb.Documents.Select(r => r.ConvertTo<Banner>()).ToList();
        List<BannerDto> result = new List<BannerDto>();

        foreach (var item in banners)
        {
            var banner = new BannerDto
            {
                CreatedAt = item.CreatedAt.ToDateTime().ToString(),
                Name = item.Name
            };
            result.Add(banner);
        } 
        return result;
    }
}
