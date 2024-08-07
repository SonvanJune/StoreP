using StoreSp.Dtos.request;
using StoreSp.Entities;

namespace StoreSp.Converters.request;

public class AddCartItemConverter : IBaseConverter<CartItem, AddCartItemDto>
{
    AddCartItemDto IBaseConverter<CartItem, AddCartItemDto>.ToDto(CartItem entity)
    {
        throw new NotImplementedException();
    }

    CartItem IBaseConverter<CartItem, AddCartItemDto>.ToEntity(AddCartItemDto dto)
    {
        return new CartItem{
            Quantity = dto.Quantity,
            Status = dto.Status != null && dto.Status != "" ? Convert.ToInt32(dto.Status) : 0  
        };
    }
}
