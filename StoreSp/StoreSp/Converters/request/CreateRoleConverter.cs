using Google.Cloud.Firestore;
using StoreSp.Dtos.request;
using StoreSp.Entities;

namespace StoreSp.Converters.request;

public class CreateRoleConverter : IBaseConverter<Role, CreateRoleDto>
{
    CreateRoleDto IBaseConverter<Role, CreateRoleDto>.ToDto(Role entity)
    {
        throw new NotImplementedException();
    }

    Role IBaseConverter<Role, CreateRoleDto>.ToEntity(CreateRoleDto dto)
    {
        return new Role
        {
            Name = dto.Name,
            Code = dto.Code,
            CreatedAt = new Timestamp()
        };
    }
}
