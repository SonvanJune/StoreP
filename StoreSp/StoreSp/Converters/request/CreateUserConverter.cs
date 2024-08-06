﻿using Google.Cloud.Firestore;
using StoreSp.Dtos.request;
using StoreSp.Entities;

namespace StoreSp.Converters.request;

public class CreateUserConverter : IBaseConverter<User, CreateUserDto>
{
    CreateUserDto IBaseConverter<User, CreateUserDto>.ToDto(User entity)
    {
        throw new NotImplementedException();
    }

    User IBaseConverter<User, CreateUserDto>.ToEntity(CreateUserDto dto)
    {
        return new User
        {
            Name = dto.Name,
            Email = dto.Email,
            Phone = dto.Phone,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Status = 0,
            Avatar = dto.Avatar,
            CreateAt = new Timestamp(),
            Account = 0,
            DeviceToken = dto.DeviceToken
        };
    }
}
