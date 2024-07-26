using Google.Cloud.Firestore;
using StoreSp.Commonds;
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
            Code = StringHelper.CreateCodeFromName(dto.Name),
            Name = dto.Name,
            Avatar = dto.Avatar ?? null!,
            CreatedAt = Timestamp.FromDateTime(DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc))
        };
    }
}
