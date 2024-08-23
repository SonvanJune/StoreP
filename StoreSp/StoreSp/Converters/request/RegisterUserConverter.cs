using Google.Cloud.Firestore;
using StoreSp.Dtos.request;
using StoreSp.Models;

namespace StoreSp.Converters.request;

public class RegisterUserConverter : IBaseConverter<User, RegisterUserDto>
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
            PasswordHash = dto.Password != null ? BCrypt.Net.BCrypt.HashPassword(dto.Password): null!,
            Status = 0,
            Avatar = dto.Avatar,
            CreateAt = DateTime.Now,
            UpdateAt = DateTime.Now,
            VerifiedAt = new DateTime(1111, 11, 11, 11, 11, 11),
            ResetTokenExpires = new DateTime(1111, 11, 11, 11, 11, 11),
            Account = 0
        };
    }
}
