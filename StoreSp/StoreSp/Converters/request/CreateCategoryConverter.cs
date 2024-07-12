using Google.Cloud.Firestore;
using StoreSp.Dtos.request;
using StoreSp.Entities;

namespace StoreSp.Converters.request;

public class CreateCategoryConverter : IBaseConverter<Category, CreateCategoryDto>
{
    CreateCategoryDto IBaseConverter<Category, CreateCategoryDto>.ToDto(Category entity)
    {
        throw new NotImplementedException();
    }

    Category IBaseConverter<Category, CreateCategoryDto>.ToEntity(CreateCategoryDto dto)
    {
        return new Category
        {
            Name = dto.Name,
            CreatedAt = Timestamp.FromDateTime(DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc))
        };
    }
}
