using StoreSp.Dtos.request;
using StoreSp.Services;
using StoreSp.Services.Impl;

namespace StoreSp.Endpoints;

public static class CategoryEndpoint
{
    public static ICategoryService? categoryService { get; set; }

    public static RouteGroupBuilder MapCategoryEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/categories");
        categoryService = new CategoryServiceImpl();
        
        group.MapGet("/", () =>
        {
           return categoryService!.GetAllCategories();
        });

        group.MapPost("/", (CreateCategoryDto createCategoryDto) =>
        {
            return categoryService!.AddCategory(createCategoryDto);
        });

        return group;
    }
}
