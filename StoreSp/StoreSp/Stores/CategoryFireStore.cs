using StoreSp.Context;
using StoreSp.Converters;
using StoreSp.Converters.request;
using StoreSp.Converters.response;
using StoreSp.Dtos.request;
using StoreSp.Dtos.response;
using StoreSp.Models;

namespace StoreSp.Stores;

public class CategoryFireStore
{
    private readonly AppDbContext? _appDbContext = null;

    public CategoryFireStore()
    {
        _appDbContext = AppDbContext.GetInstance();
    }
    public static readonly IBaseConverter<Category, CategoryDto> categoryConverter = new CategoryConverter();
    private readonly IBaseConverter<Category, CreateCategoryDto> createCategoryConverter = new CreateCategoryConverter();

    public int Add(CreateCategoryDto categoryDto)
    {
        var category = createCategoryConverter.ToEntity(categoryDto);
        var existCategory = _appDbContext!.Categories.SingleOrDefault(r => r.Code == category.Code);
        if (existCategory != null)
        {
            return -1;
        }

        if (categoryDto.ParentCategoryCode != null)
        {
            var parent = _appDbContext!.Categories.SingleOrDefault(r => r.Code == categoryDto.ParentCategoryCode);

            if (parent != null)
            {
                category.ParentCategoryId = parent.Id!;
                category.ParentCategory = parent;
                category.Level = parent.Level + 1;
            }
            else
            {
                return 0;
            }
        }
        else
        {
            category.ParentCategoryId = -1;
            category.ParentCategory = null;
            category.Level = 0;
        }

        _appDbContext!.Categories.Add(category);
        _appDbContext!.SaveChanges();
        return 1;
    }

    public List<CategoryDto> GetAllCategories(bool isMobile)
    {
        var categories = _appDbContext!.Categories.ToList();
        List<CategoryDto> result = new List<CategoryDto>();

        var categoryDtos = new List<CategoryDto>();
        int high = 0;

        foreach (var category in categories)
        {
            if (high <= category.Level)
            {
                high = category.Level;
            }
            categoryDtos.Add(categoryConverter.ToDto(category));
        }

        if (isMobile == false)
        {
            for (int i = 0; i < high; i++)
            {
                for (int j = 0; j < categoryDtos.Count; j++)
                {
                    if (categoryDtos[j].ParentCategoryId == -1 && categoryDtos[j].Level == i)
                    {
                        result.Add(categoryDtos[j]);
                        List<CategoryDto> arr = categoryDtos.FindAll(c => c.ParentCategoryId == categoryDtos[j].Id);
                        categoryDtos[j].Children = arr;
                        break;
                    }

                    if (categoryDtos[j].ParentCategoryId != -1 && categoryDtos[j].Level == i)
                    {
                        List<CategoryDto> arr = categoryDtos.FindAll(c => c.ParentCategoryId == categoryDtos[j].Id);
                        categoryDtos[j].Children = arr;
                    }
                }
            }
            return result;
        }
        return categoryDtos;
    }

    public async Task UpdateCategory(UpdateCategoryDto categoryDto)
    {
        var category = _appDbContext!.Categories.SingleOrDefault(c => c.Code == categoryDto.Code);
        if (category != null)
        {
            category.Name = categoryDto.Name;
            category.Avatar = categoryDto.Avatar;
            _appDbContext.Categories.Update(category);
            await _appDbContext.SaveChangesAsync();
        }
    }
}
