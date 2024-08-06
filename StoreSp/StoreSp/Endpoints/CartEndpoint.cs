using Microsoft.AspNetCore.Mvc;
using StoreSp.Dtos.request;
using StoreSp.Services;
using StoreSp.Services.Impl;

namespace StoreSp.Endpoints;

public static class CartEndpoint
{
    public static ICartService? CartService { get; set; }
    public static IAuthService? authService { get; set; }

    public static RouteGroupBuilder MapCartEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/carts");
        CartService = new CartServiceImpl();
        authService = new AuthServiceImpl();

        group.MapGet("/{username}", (string username , [FromHeader] string authorization) =>
        {
            return authService.GetResult(authorization, CartService!.GetCartByUser(username));
        }).WithParameterValidation().RequireAuthorization("nguoi-mua");

        group.MapPost("/", (AddCartItemDto addCartItemDto , [FromHeader] string authorization) =>
        {
            return authService.GetResult(authorization, CartService!.AddToCart(addCartItemDto));
        }).WithParameterValidation().RequireAuthorization("nguoi-mua");

        group.MapPut("/update", (UpdateCartDto updateCartDto , [FromHeader] string authorization) =>
        {
            return authService.GetResult(authorization, CartService!.UpdateCartByUser(updateCartDto));
        }).WithParameterValidation().RequireAuthorization("nguoi-mua");

        group.MapPost("/check", (CheckoutCartItemDto checkoutCartItemDto , [FromHeader] string authorization) =>
        {
            return authService.GetResult(authorization, CartService!.CheckoutCartItem(checkoutCartItemDto.CartItemCode));
        }).WithParameterValidation().RequireAuthorization("nguoi-mua");

        return group;
    }
}
