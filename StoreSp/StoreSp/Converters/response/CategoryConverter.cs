using StoreSp.Converters;
using StoreSp.Dtos.response;
using StoreSp.Entities;

namespace StoreSp.Converters.response;

public class CategoryConverter : IBaseConverter<Category, CategoryDto>
{
    CategoryDto IBaseConverter<Category, CategoryDto>.ToDto(Category entity)
    {
        return new CategoryDto
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            ParentCategoryId = entity.ParentCategoryId,
            Level = entity.Level
        };
    }

    Category IBaseConverter<Category, CategoryDto>.ToEntity(CategoryDto dto)
    {
        throw new NotImplementedException();
    }
}
