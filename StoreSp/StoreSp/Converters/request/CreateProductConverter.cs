using Google.Cloud.Firestore;
using StoreSp.Dtos.request;
using StoreSp.Entities;

namespace StoreSp.Converters.request;

public class CreateProductConverter : IBaseConverter<Product, CreateProductDto>
{
    CreateProductDto IBaseConverter<Product, CreateProductDto>.ToDto(Product entity)
    {
        throw new NotImplementedException();
    }

    Product IBaseConverter<Product, CreateProductDto>.ToEntity(CreateProductDto dto)
    {
        return new Product
        {
            CreatedAt = Timestamp.FromDateTime(DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)),
            Name = dto.Name,
            Description = dto.Description,
            ShippingUnit = dto.ShippingUnit,
            Active = 0,
            Price = dto.Price,
            SaleOff = dto.SaleOff,
            PriceSaleOff = dto.SaleOff == 0 ? dto.Price : dto.Price - (dto.Price * dto.SaleOff /100),
            QuantitySelled = 0,
            Likes = 0
        };
    }
}
