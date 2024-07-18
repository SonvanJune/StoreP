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
                message = "Khong tim thay san pham",
                data = null
            });
        }

        return Results.Created("", new HttpStatusConfig
        {
            status = HttpStatusCode.Created,
            message = "Them vao gio hang thanh cong",
            data = null
        });
    }
}
