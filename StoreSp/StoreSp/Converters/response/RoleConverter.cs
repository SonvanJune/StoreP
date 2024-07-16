﻿using StoreSp.Dtos.response;
using StoreSp.Entities;

namespace StoreSp.Converters.response;

public class RoleConverter : IBaseConverter<Role, RoleDto>
{
    RoleDto IBaseConverter<Role, RoleDto>.ToDto(Role entity)
    {
        return new RoleDto
        {
            Name = entity.Name,
            Code = entity.Code
        };
    }

    Role IBaseConverter<Role, RoleDto>.ToEntity(RoleDto dto)
    {
        throw new NotImplementedException();
    }
}
