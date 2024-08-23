using Google.Cloud.Firestore;
using StoreSp.Dtos.request;
using StoreSp.Models;

namespace StoreSp.Converters.request;

public class GoogleRegisterConverter : IBaseConverter<User, GoogleRegisterDto>
{
    GoogleRegisterDto IBaseConverter<User, GoogleRegisterDto>.ToDto(User entity)
    {
        throw new NotImplementedException();
    }

    User IBaseConverter<User, GoogleRegisterDto>.ToEntity(GoogleRegisterDto dto)
    {
        return new User
        {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = null!,
            Status = 0,
            Avatar = dto.Avatar,
            CreateAt = DateTime.Now,
            UpdateAt = DateTime.Now,
            VerifiedAt = new DateTime(1111, 11, 11, 11, 11, 11),
            ResetTokenExpires = new DateTime(1111, 11, 11, 11, 11, 11),
            Account = 0,
            DeviceToken = dto.DeviceToken
        };
    }
}



