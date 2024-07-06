namespace StoreSp.Services.Impl;

using System.Net;
using StoreSp.Commons;
using StoreSp.Dtos.request;
using StoreSp.Dtos.response;

public class UserServiceImpl : IUserService
{
    public IResult AddUser(CreateUserDto createUserDto, UserFireStore userFireStore)
    {
        if (userFireStore is null)
        {
            return Results.NoContent();
        }

        userFireStore!.Add(createUserDto);

        return Results.Created("", new HttpStatusConfig
        {
            status = HttpStatusCode.Created,
            message = "Created Success",
            data = null
        });
    }

    public IResult GetAllUsers(UserFireStore userFireStore)
    {
        return Results.Ok(new HttpStatusConfig
        {
            status = HttpStatusCode.OK,
            message = "Success",
            data = userFireStore!.GetAllUser().Result
        });
    }

    public IResult GetUserById(string id, UserFireStore userFireStore)
    {
        return Results.Ok(new HttpStatusConfig
        {
            status = HttpStatusCode.OK,
            message = "Success",
            data = userFireStore!.GetUser(id).Result
        });
    }
}
