using StoreSp.Dtos.response;
using StoreSp.Entities;

namespace StoreSp.Converters.response;

public class AddressConverter : IBaseConverter<Address, AddressDto>
{
    AddressDto IBaseConverter<Address, AddressDto>.ToDto(Address entity)
    {
        return new AddressDto
        {
            Code = entity.Code!,
            PhoneGet = entity.PhoneGet,
            NameGet = entity.NameGet,
            Description = entity.Description,
            Lat = entity.Lat,
            Long = entity.Long,
            Location = entity.Location,
            Status = entity.Status
        };
    }

    Address IBaseConverter<Address, AddressDto>.ToEntity(AddressDto dto)
    {
        throw new NotImplementedException();
    }
}
