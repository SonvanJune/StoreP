using Microsoft.AspNetCore.Mvc;
using StoreSp.Dtos.request;
using StoreSp.Services;
using StoreSp.Services.Impl;

namespace StoreSp.Endpoints;

public static class CategoryEndpoint
{
    public static ICategoryService? CategoryService { get; set; }
    public static IAuthService? authService { get; set; }

    public static RouteGroupBuilder MapCategoryEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/categories");
        CategoryService = new CategoryServiceImpl();
        authService = new AuthServiceImpl();
        
        group.MapGet("/{isMobile}", (bool isMobile , [FromHeader] string authorization) =>
        {
           return authService.GetResult(authorization, CategoryService!.GetAllCategories(isMobile));
        }).RequireAuthorization();

        group.MapPost("/", (CreateCategoryDto createCategoryDto , [FromHeader] string authorization) =>
        {
            return authService.GetResult(authorization, CategoryService!.AddCategory(createCategoryDto));
        }).WithParameterValidation().RequireAuthorization("quan-tri-vien");

        group.MapPut("/update", (UpdateCategoryDto updateCategoryDto , [FromHeader] string authorization) =>
        {
            return authService.GetResult(authorization, CategoryService!.UpdateCategory(updateCategoryDto));
        }).WithParameterValidation().RequireAuthorization("quan-tri-vien");

        return group;
    }
}
