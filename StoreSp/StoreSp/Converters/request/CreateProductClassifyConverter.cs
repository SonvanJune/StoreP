using StoreSp.Dtos.request;
using StoreSp.Entities;

namespace StoreSp.Converters.request;

public class CreateProductClassifyConverter : IBaseConverter<ProductClassify, CreateProductClassifyDto>
{
    CreateProductClassifyDto IBaseConverter<ProductClassify, CreateProductClassifyDto>.ToDto(ProductClassify entity)
    {
        throw new NotImplementedException();
    }

    ProductClassify IBaseConverter<ProductClassify, CreateProductClassifyDto>.ToEntity(CreateProductClassifyDto dto)
    {
        return new ProductClassify
        {
            GroupName = dto.GroupName,
            Name = dto.Name,
            Image = dto.Image,
            Quantity = dto.Quantity,
            IncreasePercent = dto.IncreasePercent,
            Status = 1
        };
    }
}
