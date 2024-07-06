using System.CodeDom;
using StoreSp.Dtos.request;
using StoreSp.Dtos.response;
using StoreSp.Services;
using StoreSp.Services.Impl;

namespace StoreSp.Endpoints;

public static class UserEndpoint
{
    public static UserFireStore? userFireStore { get; set; }
    public static IUserService? userService { get; set;}
    public static RouteGroupBuilder MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("users");
        userService = new UserServiceImpl();

        group.MapPost("/", (CreateUserDto createUserDto) =>
        {
            return userService.AddUser(createUserDto,userFireStore!);
        });

        group.MapGet("/", () =>
        {
            return userService.GetAllUsers(userFireStore!);
        });

        group.MapGet("/{id}", (string id) =>{
            return userService.GetUserById(id, userFireStore!);
        });

        return group;
    }
}
