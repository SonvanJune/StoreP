using StoreSp.Dtos.request;
using StoreSp.Entities;

namespace StoreSp.Converters.request;

public class CreateAddressConverter : IBaseConverter<Address, CreateAddressDto>
{
    CreateAddressDto IBaseConverter<Address, CreateAddressDto>.ToDto(Address entity)
    {
        throw new NotImplementedException();
    }

    Address IBaseConverter<Address, CreateAddressDto>.ToEntity(CreateAddressDto dto)
    {
        return new Address
        {
            Description = dto.Description,
            PhoneGet = dto.PhoneGet,
            NameGet = dto.NameGet,
            Status = dto.Status
        };
    }
}
