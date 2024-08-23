using Google.Cloud.Firestore;
using StoreSp.Dtos.request;
using StoreSp.Models;

namespace StoreSp.Converters.request;

public class AddShippingMethodConverter : IBaseConverter<ShippingMethod, AddShippingMethodDto>
{
    AddShippingMethodDto IBaseConverter<ShippingMethod, AddShippingMethodDto>.ToDto(ShippingMethod entity)
    {
        throw new NotImplementedException();
    }

    ShippingMethod IBaseConverter<ShippingMethod, AddShippingMethodDto>.ToEntity(AddShippingMethodDto dto)
    {
        return new ShippingMethod
        {
            Name = dto.Name,
            CreatedAt = DateTime.Now,
            Status = 0,
            Location = dto.Location,
            Ensure = dto.Ensure,
            Price = dto.Price,
            Lat = dto.Lat,
            Long = dto.Long,
        };
    }
}
