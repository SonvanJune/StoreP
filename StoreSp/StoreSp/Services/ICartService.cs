using StoreSp.Dtos.request;

namespace StoreSp.Services;

public interface ICartService
{
    public IResult AddToCart(AddCartItemDto addCartItemDto);
    public IResult GetCartByUser(string username);
    public IResult CheckoutCartItem(string username);
}
