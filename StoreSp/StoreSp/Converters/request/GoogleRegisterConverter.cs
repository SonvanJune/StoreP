using System.Globalization;
using Google.Cloud.Firestore;
using StoreSp.Dtos.request;
using StoreSp.Entities;

namespace StoreSp.Converters.request;

public class GoogleRegisterConverter : IBaseConverter<User, GoogleRegisterDto>
{
    GoogleRegisterDto IBaseConverter<User, GoogleRegisterDto>.ToDto(User entity)
    {
        throw new NotImplementedException();
    }

    User IBaseConverter<User, GoogleRegisterDto>.ToEntity(GoogleRegisterDto dto)
    {
        var unspecified = new DateTime(1111, 11, 11, 11, 11, 11, DateTimeKind.Unspecified);
        var specified = DateTime.SpecifyKind(unspecified, DateTimeKind.Utc);
        return new User
        {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = null!,
            Status = 0,
            Avatar = dto.Avatar,
            CreateAt = Timestamp.FromDateTime(DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)),
            UpdateAt = new Timestamp(),
            VerifiedAt = Timestamp.FromDateTime(specified),
            ResetTokenExpires = Timestamp.FromDateTime(specified)
        };
    }
}



