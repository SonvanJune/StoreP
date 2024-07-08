using StoreSp.Converters;
using StoreSp.Entities;

namespace StoreSp;

public class RoleConverter : IBaseConverter<Role, RoleDto>
{
    RoleDto IBaseConverter<Role, RoleDto>.ToDto(Role entity)
    {
        return new RoleDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Code = entity.Code
        };
    }

    Role IBaseConverter<Role, RoleDto>.ToEntity(RoleDto dto)
    {
        throw new NotImplementedException();
    }
}
