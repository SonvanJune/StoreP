using Microsoft.AspNetCore.Mvc;
using StoreSp.Dtos.request;
using StoreSp.Services;
using StoreSp.Services.Impl;

namespace StoreSp.Endpoints;

public static class ProductEndpoint
{
    public static IProductService? ProductService { get; set; }
    public static IAuthService? authService { get; set; }

    public static RouteGroupBuilder MapProductEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/products");
        ProductService = new ProductServiceImpl();
        authService = new AuthServiceImpl();
        
        group.MapGet("/categories", ([FromQuery] string code , [FromHeader] string authorization) =>
        {
           return authService.GetResult(authorization, ProductService.GetProductsByCategory(code));
        }).RequireAuthorization();

        group.MapGet("", ([FromQuery] string pCode , [FromHeader] string authorization) =>
        {
           return authService.GetResult(authorization, ProductService.GetProductByCode(pCode));
        }).RequireAuthorization();

        group.MapGet("/search", ([FromQuery] string name , [FromHeader] string authorization) =>
        {
           return authService.GetResult(authorization, ProductService.GetProductsBySearch(name));
        }).RequireAuthorization();

        group.MapPost("/", (CreateProductDto createProductDto , [FromHeader] string authorization) =>
        {
            return authService.GetResult(authorization, ProductService.AddProduct(createProductDto));
        }).WithParameterValidation().RequireAuthorization("nguoi-ban");

        group.MapPost("/like", (LikeProductDto likeProductDto , [FromHeader] string authorization) =>
        {
            return authService.GetResult(authorization, ProductService.LikeProduct(likeProductDto));
        }).WithParameterValidation().RequireAuthorization("nguoi-ban");
        return group;
    }
}
