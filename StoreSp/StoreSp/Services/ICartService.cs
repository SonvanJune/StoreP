using StoreSp.Dtos.request;

namespace StoreSp.Services;

public interface ICartService
{
    public IResult AddToCart(AddCartItemDto addCartItemDto);
}
