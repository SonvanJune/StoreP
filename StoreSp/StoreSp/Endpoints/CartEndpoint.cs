using System.Net;
using Microsoft.AspNetCore.Mvc;
using StoreSp.Commonds;
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
            if (authService.GetResult(authorization) == 1)
            {
                return CartService!.GetCartByUser(username);
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
        }).WithParameterValidation().RequireAuthorization("nguoi-mua");

        group.MapPost("/", (AddCartItemDto addCartItemDto , [FromHeader] string authorization) =>
        {
            if (authService.GetResult(authorization) == 1)
            {
                return CartService!.AddToCart(addCartItemDto);
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
        }).WithParameterValidation().RequireAuthorization("nguoi-mua");

        group.MapPut("/update", (UpdateCartDto updateCartDto , [FromHeader] string authorization) =>
        {
            if (authService.GetResult(authorization) == 1)
            {
                return CartService!.UpdateCartByUser(updateCartDto);
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
        }).WithParameterValidation().RequireAuthorization("nguoi-mua");

        group.MapPost("/check", (CheckoutCartItemDto checkoutCartItemDto , [FromHeader] string authorization) =>
        {
            if (authService.GetResult(authorization) == 1)
            {
                return CartService!.CheckoutCartItem(checkoutCartItemDto.CartItemCode);
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
        }).WithParameterValidation().RequireAuthorization("nguoi-mua");

        return group;
    }
}
