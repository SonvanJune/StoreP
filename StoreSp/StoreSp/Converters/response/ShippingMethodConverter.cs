using StoreSp.Dtos.response;
using StoreSp.Entities;

namespace StoreSp.Converters.response;

public class ShippingMethodConverter : IBaseConverter<ShippingMethod, ShippingMethodDto>
{
    ShippingMethodDto IBaseConverter<ShippingMethod, ShippingMethodDto>.ToDto(ShippingMethod entity)
    {
        return new ShippingMethodDto
        {
            Name = entity.Name,
            Price = entity.Price,
            CreatedAt = entity.CreatedAt.ToDateTime().ToString(),
            Code = entity.Code!,
            Location = entity.Location,
            Ensure = entity.Ensure,
            Status = entity.Status,
            Lat = entity.Lat,
            Long = entity.Long
        };
    }

    ShippingMethod IBaseConverter<ShippingMethod, ShippingMethodDto>.ToEntity(ShippingMethodDto dto)
    {
        throw new NotImplementedException();
    }
}
