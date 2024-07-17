using StoreSp.Dtos.request;
using StoreSp.Services;
using StoreSp.Services.Impl;

namespace StoreSp.Endpoints;

public static class CategoryEndpoint
{
    public static ICategoryService? CategoryService { get; set; }

    public static RouteGroupBuilder MapCategoryEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/categories");
        CategoryService = new CategoryServiceImpl();
        
        group.MapGet("/", () =>
        {
           return CategoryService!.GetAllCategories();
        });

        group.MapPost("/", (CreateCategoryDto createCategoryDto) =>
        {
            return CategoryService!.AddCategory(createCategoryDto);
        });

        return group;
    }
}
