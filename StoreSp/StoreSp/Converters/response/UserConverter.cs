namespace StoreSp.Converters.response;

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
            Avatar = entity.Avatar,
            Status = entity.Status,
            VerifiedAt = entity.VerifiedAt.ToDateTime().ToShortDateString(),
            Address = entity.Address
        };
    }

    User IBaseConverter<User, UserDto>.ToEntity(UserDto dto)
    {
        throw new NotSupportedException();
    }
}



