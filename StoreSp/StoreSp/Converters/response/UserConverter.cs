namespace StoreSp.Converters.response;
using StoreSp.Dtos.response;
using StoreSp.Entities;
public class UserConverter : IBaseConverter<User, UserDto>
{
    UserDto IBaseConverter<User, UserDto>.ToDto(User entity)
    {
        return new UserDto
        {
            Id = entity.Id,
            Name = entity.Name
        };
    }

    User IBaseConverter<User, UserDto>.ToEntity(UserDto dto)
    {
        return new User
        {
            Name = dto.Name
        };
    }
}



