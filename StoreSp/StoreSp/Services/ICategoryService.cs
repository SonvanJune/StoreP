using StoreSp.Dtos.request;
using StoreSp.Dtos.response;
using StoreSp.Stores;

namespace StoreSp.Services;

public interface ICategoryService
{
    public IResult AddCategory(CreateCategoryDto createCategoryDto);
    public IResult GetAllCategories();
}
