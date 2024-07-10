using System.Globalization;
using Google.Cloud.Firestore;
using StoreSp.Dtos.request;
using StoreSp.Entities;

namespace StoreSp.Converters.request;

public class RegisterUserConverter : IBaseConverter<User, RegisterUserDto>
{
    RegisterUserDto IBaseConverter<User, RegisterUserDto>.ToDto(User entity)
    {
        throw new NotImplementedException();
    }

    User IBaseConverter<User, RegisterUserDto>.ToEntity(RegisterUserDto dto)
    {
        var unspecified = new DateTime(1111, 11, 11, 11, 11, 11, DateTimeKind.Unspecified);
        var specified = DateTime.SpecifyKind(unspecified, DateTimeKind.Utc);
        return new User
        {
            Name = dto.Name,
            Email = dto.Email,
            Phone = dto.Phone,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Status = 0,
            Avatar = dto.Avatar,
            CreateAt = new Timestamp(),
            UpdateAt = new Timestamp(),
            VerifiedAt = Timestamp.FromDateTime(specified),
            ResetTokenExpires = Timestamp.FromDateTime(specified)
        };
    }
}
