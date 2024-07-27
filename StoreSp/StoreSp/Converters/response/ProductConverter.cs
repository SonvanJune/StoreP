using StoreSp.Dtos.response;
using StoreSp.Entities;

namespace StoreSp.Converters.response;

public class ProductConverter : IBaseConverter<Product, ProductDto>
{
    ProductDto IBaseConverter<Product, ProductDto>.ToDto(Product entity)
    {
        return new ProductDto
        {
            Name = entity.Name,
            Description = entity.Description,
            Price = entity.Price,
            PriceSaleOff = entity.PriceSaleOff,
            SaleOff = entity.SaleOff,
            ShippingUnit = entity.ShippingUnit,
            Code = entity.Code!,
            QuantitySelled = entity.QuantitySelled,
            Likes = entity.Likes
        };
    }

    Product IBaseConverter<Product, ProductDto>.ToEntity(ProductDto dto)
    {
        throw new NotImplementedException();
    }
}
