using StoreSp.Dtos.request;
using StoreSp.Services;
using StoreSp.Services.Impl;

namespace StoreSp.Endpoints;

public static class CartEndpoint
{
    public static ICartService? CartService { get; set; }

    public static RouteGroupBuilder MapCartEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/carts");
        CartService = new CartServiceImpl();

        // group.MapGet("/", () =>
        // {
        //     return CategoryService!.GetAllCategories();
        // }).RequireAuthorization();

        group.MapPost("/", (AddCartItemDto addCartItemDto) =>
        {
            return CartService!.AddToCart(addCartItemDto);
        }).WithParameterValidation();

        return group;
    }
}
