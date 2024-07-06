namespace StoreSp.Services.Impl;

using StoreSp.Dtos.request;
using StoreSp.Dtos.response;

public class UserServiceImpl:IUserService
{
    public IResult AddUser(CreateUserDto createUserDto, UserFireStore userFireStore)
    {
        if (userFireStore is null)
        {
            return Results.NoContent();
        }
        userFireStore!.Add(new UserDto
        {
            Name = createUserDto.Name
        });
        return Results.Created("", null);
    }

    public IResult GetAllUsers(UserFireStore userFireStore)
    {
        return Results.Ok(userFireStore!.GetAllUser().Result);
    }

    public IResult GetUserById(string id, UserFireStore userFireStore)
    {
        return Results.Ok(userFireStore!.GetUser(id).Result);
    }
}
