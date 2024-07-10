namespace StoreSp.Converters.response;

using StoreSp.Commonds;
using StoreSp.Dtos.response;
using StoreSp.Entities;
public class UserConverter : IBaseConverter<User, UserDto>
{
    UserDto IBaseConverter<User, UserDto>.ToDto(User entity)
    {
        return new UserDto
        {
            Name = entity.Name,
            CreatedAt = entity.CreateAt.ToDateTime().ToShortDateString(),
            Email = entity.Email,
            Phone = entity.Phone,
            PasswordHash = entity.PasswordHash,
            Avatar = entity.Avatar,
            Status = entity.Status,
            VerifiedAt = entity.VerifiedAt.ToDateTime().ToShortDateString()
        };
    }

    User IBaseConverter<User, UserDto>.ToEntity(UserDto dto)
    {
        throw new NotSupportedException();
    }
}



