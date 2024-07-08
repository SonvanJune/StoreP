using Google.Cloud.Firestore;
using StoreSp.Converters;
using StoreSp.Dtos.request;
using StoreSp.Entities;

namespace StoreSp;

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
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Status = 0,
            Active = 0,
            Avatar = dto.Avatar,
            CreateAt = new Timestamp()
        };
    }
}
