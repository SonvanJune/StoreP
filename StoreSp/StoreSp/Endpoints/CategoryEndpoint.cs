using System.Net;
using Microsoft.AspNetCore.Mvc;
using StoreSp.Commonds;
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
            if (authService.GetResult(authorization) == 1)
            {
                return CategoryService!.GetAllCategories(isMobile);
            }
            else
            {
                return Results.BadRequest(new HttpStatusConfig
                {
                    status = HttpStatusCode.BadRequest,
                    message = "Token has expired",
                    data = null
                });
            }
        }).RequireAuthorization();

        group.MapPost("/", (CreateCategoryDto createCategoryDto , [FromHeader] string authorization) =>
        {
            if (authService.GetResult(authorization) == 1)
            {
                return CategoryService!.AddCategory(createCategoryDto);
            }
            else
            {
                return Results.BadRequest(new HttpStatusConfig
                {
                    status = HttpStatusCode.BadRequest,
                    message = "Token has expired",
                    data = null
                });
            }
        }).WithParameterValidation().RequireAuthorization("quan-tri-vien");

        group.MapPut("/update", (UpdateCategoryDto updateCategoryDto , [FromHeader] string authorization) =>
        {
            if (authService.GetResult(authorization) == 1)
            {
                return CategoryService!.UpdateCategory(updateCategoryDto);
            }
            else
            {
                return Results.BadRequest(new HttpStatusConfig
                {
                    status = HttpStatusCode.BadRequest,
                    message = "Token has expired",
                    data = null
                });
            }
        }).WithParameterValidation().RequireAuthorization("quan-tri-vien");

        return group;
    }
}
