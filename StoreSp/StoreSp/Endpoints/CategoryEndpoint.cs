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
        
        group.MapGet("/{isMobile}", (bool isMobile) =>
        {
           return CategoryService!.GetAllCategories(isMobile);
        }).RequireAuthorization();

        group.MapPost("/", (CreateCategoryDto createCategoryDto) =>
        {
            return CategoryService!.AddCategory(createCategoryDto);
        }).WithParameterValidation().RequireAuthorization("quan-tri-vien");

        return group;
    }
}
