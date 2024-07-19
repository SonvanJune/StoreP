using StoreSp.Dtos.response;
using StoreSp.Entities;

namespace StoreSp.Converters.response;

public class CartItemConverter : IBaseConverter<CartItem, CartItemDto>
{
    CartItemDto IBaseConverter<CartItem, CartItemDto>.ToDto(CartItem entity)
    {
        return new CartItemDto
        {
            Quantity = entity.Quantity,
            Price = entity.Price,
            Total = entity.Total
        };
    }

    CartItem IBaseConverter<CartItem, CartItemDto>.ToEntity(CartItemDto dto)
    {
        throw new NotImplementedException();
    }
}
