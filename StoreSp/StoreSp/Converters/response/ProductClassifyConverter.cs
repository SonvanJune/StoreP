using StoreSp.Dtos.response;
using StoreSp.Entities;

namespace StoreSp.Converters.response;

public class ProductClassifyConverter : IBaseConverter<ProductClassify, ProductClassifyDto>
{
    ProductClassifyDto IBaseConverter<ProductClassify, ProductClassifyDto>.ToDto(ProductClassify entity)
    {
        return new ProductClassifyDto
        {
            GroupName = entity.GroupName,
            Name = entity.Name,
            Image = entity.Image,
            Quantity = entity.Quantity,
            IncreasePercent = entity.IncreasePercent,
            Status = entity.Status
        };
    }

    ProductClassify IBaseConverter<ProductClassify, ProductClassifyDto>.ToEntity(ProductClassifyDto dto)
    {
        throw new NotImplementedException();
    }
}
