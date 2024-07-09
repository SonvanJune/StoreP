using Google.Cloud.Firestore;
using StoreSp.Dtos.request;
using StoreSp.Entities;

namespace StoreSp.Converters.request;

public class RegisterUserConverter:IBaseConverter<User,RegisterUserDto>
{
    RegisterUserDto IBaseConverter<User, RegisterUserDto>.ToDto(User entity)
    {
        throw new NotImplementedException();
    }

    User IBaseConverter<User, RegisterUserDto>.ToEntity(RegisterUserDto dto)
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
