﻿namespace StoreSp.Converters.response;
using StoreSp.Dtos.response;
using StoreSp.Entities;
public class UserConverter : IBaseConverter<User, UserDto>
{
    UserDto IBaseConverter<User, UserDto>.ToDto(User entity)
    {
        return new UserDto
        {
            Id = entity.Id,
            Name = entity.Name,
            CreatedAt = entity.CreateAt.ToDateTime().ToShortDateString(),
            Email = entity.Email,
            Phone = entity.Phone,
            Password = entity.Password,
            Avatar = entity.Avatar,
            Status = entity.Status,
            Active = entity.Active
        };
    }

    User IBaseConverter<User, UserDto>.ToEntity(UserDto dto)
    {
        throw new NotSupportedException();
    }
}



