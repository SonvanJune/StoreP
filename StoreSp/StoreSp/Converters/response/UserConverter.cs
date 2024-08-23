namespace StoreSp.Converters.response;

using StoreSp.Dtos.response;
using StoreSp.Models;
public class UserConverter : IBaseConverter<User, UserDto>
{
    UserDto IBaseConverter<User, UserDto>.ToDto(User entity)
    {
        return new UserDto
        {
            Name = entity.Name,
            CreatedAt = entity.CreateAt.ToString(),
            Email = entity.Email!,
            Phone = entity.Phone!,
            Avatar = entity.Avatar!,
            Status = entity.Status,
            RefreshToken = entity.RefreshToken ?? null,
            VerifiedAt = entity.VerifiedAt.ToString()
        };
    }

    User IBaseConverter<User, UserDto>.ToEntity(UserDto dto)
    {
        throw new NotSupportedException();
    }
}



