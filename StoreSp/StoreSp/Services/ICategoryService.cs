using StoreSp.Dtos.request;

namespace StoreSp.Services;

public interface ICategoryService
{
    public IResult AddCategory(CreateCategoryDto createCategoryDto);
    public IResult GetAllCategories(bool isMobile);
    public IResult UpdateCategory(UpdateCategoryDto updateCategoryDto);
}
