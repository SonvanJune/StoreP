using System.Net;
using StoreSp.Commonds;
using StoreSp.Dtos.request;
using StoreSp.Stores;

namespace StoreSp.Services.Impl;

public class CartServiceImpl : ICartService
{
    public static CartFireStore? CartFireStore { get; set; }
    IResult ICartService.AddToCart(AddCartItemDto addCartItemDto)
    {
        var item = CartFireStore!.AddToCart(addCartItemDto).Result;
        if (item == null)
        {
            return Results.BadRequest(new HttpStatusConfig
            {
                status = HttpStatusCode.BadRequest,
                message = "Không tìm thấy sản phẩm",
                data = null
            });
        }

        return Results.Created("", new HttpStatusConfig
        {
            status = HttpStatusCode.Created,
            message = "Thêm vào giỏ hàng thành công",
            data = null
        });
    }

    IResult ICartService.CheckoutCartItem(List<string> codes)
    {
        foreach (var code in codes)
        {
            var item = CartFireStore!.CheckoutItemInCart(code).Result;
        }
        return Results.Ok(new HttpStatusConfig
        {
            status = HttpStatusCode.OK,
            message = "Thành công",
            data = null
        });
    }

    IResult ICartService.GetCartByUser(string username)
    {
        return Results.Ok(new HttpStatusConfig
        {
            status = HttpStatusCode.OK,
            message = "Thành công",
            data = CartFireStore!.GetCartByUser(username).Result
        });
    }

    IResult ICartService.UpdateCartByUser(UpdateCartDto updateCartDto)
    {
        var a = CartFireStore!.UpdateCartByUser(updateCartDto).Result;
        return Results.Created("", new HttpStatusConfig
        {
            status = HttpStatusCode.Created,
            message = "Thành công",
            data = null
        });
    }
}
