using Google.Cloud.Firestore;
using StoreSp.Dtos.request;
using StoreSp.Entities;

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
            CreatedAt = Timestamp.FromDateTime(DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)),
            Status = 0,
            Location = dto.Location,
            Ensure = dto.Ensure,
            Price = dto.Price
        };
    }
}
