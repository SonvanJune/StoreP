using Microsoft.AspNetCore.Mvc;
using StoreSp.Dtos.request;
using StoreSp.Services;
using StoreSp.Services.Impl;

namespace StoreSp.Endpoints;

public static class ProductEndpoint
{
    public static IProductService? ProductService { get; set; }

    public static RouteGroupBuilder MapProductEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/products");
        ProductService = new ProductServiceImpl();
        
        group.MapGet("/categories", ([FromQuery] string code) =>
        {
           return ProductService.GetProductsByCategory(code);
        }).RequireAuthorization();

        group.MapGet("", ([FromQuery] string pCode) =>
        {
           return ProductService.GetProductByCode(pCode);
        }).RequireAuthorization();

        group.MapPost("/", (CreateProductDto createProductDto) =>
        {
            return ProductService.AddProduct(createProductDto);
        }).WithParameterValidation().RequireAuthorization("nguoi-ban");

        group.MapPost("/like", (LikeProductDto likeProductDto) =>
        {
            return ProductService.LikeProduct(likeProductDto);
        }).WithParameterValidation().RequireAuthorization("nguoi-ban");
        return group;
    }
}
