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

        group.MapGet("/{username}", (string username) =>
        {
            return CartService!.GetCartByUser(username);
        }).WithParameterValidation().RequireAuthorization("nguoi-mua");

        group.MapPost("/", (AddCartItemDto addCartItemDto) =>
        {
            return CartService!.AddToCart(addCartItemDto);
        }).WithParameterValidation().RequireAuthorization("nguoi-mua");

        group.MapPut("/", (UpdateCartDto updateCartDto) =>
        {
            return CartService!.UpdateCartByUser(updateCartDto);
        }).WithParameterValidation().RequireAuthorization("nguoi-mua");

        group.MapPost("/check", (CheckoutCartItemDto checkoutCartItemDto) =>
        {
            return CartService!.CheckoutCartItem(checkoutCartItemDto.CartItemCode);
        }).WithParameterValidation().RequireAuthorization("nguoi-mua");

        return group;
    }
}
