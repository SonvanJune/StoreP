using Google.Cloud.Firestore;
using StoreSp.Context;
using StoreSp.Converters;
using StoreSp.Converters.request;
using StoreSp.Converters.response;
using StoreSp.Dtos.request;
using StoreSp.Dtos.response;
using StoreSp.Models;

namespace StoreSp.Stores;

public class ShippingMethodFirestore
{
    private readonly IBaseConverter<ShippingMethod, AddShippingMethodDto> createShippingMethod = new AddShippingMethodConverter();
    private readonly IBaseConverter<ShippingMethod, ShippingMethodDto> shippingMethodConverter = new ShippingMethodConverter();
    private readonly AppDbContext? _appDbContext = null;

    public ShippingMethodFirestore()
    {
        _appDbContext = AppDbContext.GetInstance();
    }

    public Task Add(AddShippingMethodDto dto)
    {
        var shippingMethod = createShippingMethod.ToEntity(dto);
        Random rnd = new Random();
        string randomCode = rnd.Next(1, 100000).ToString();
        while (_appDbContext!.ShippingMethods.SingleOrDefault(r => r.Code == randomCode) != null)
        {
            randomCode = rnd.Next(1, 100000).ToString();
        }
        shippingMethod.Code = randomCode;
        _appDbContext.ShippingMethods.Add(shippingMethod);
        return _appDbContext.SaveChangesAsync();
    }

    public Task<List<ShippingMethodDto>> GetAllShippingMethods()
    {
        var shippingMethod = _appDbContext!.ShippingMethods.ToList();
        return Task.FromResult(shippingMethod.Select(shippingMethodConverter.ToDto).ToList());
    }
}
