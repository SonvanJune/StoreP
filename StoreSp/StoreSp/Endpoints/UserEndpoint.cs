using Microsoft.AspNetCore.Mvc;
using StoreSp.Dtos.request;
using StoreSp.Dtos.response;
using StoreSp.Services;
using StoreSp.Services.Impl;
using StoreSp.Stores;

namespace StoreSp.Endpoints;

public static class UserEndpoint
{
    public static UserFireStore? userFireStore { get; set; }
    public static IUserService? userService { get; set;}

    public static RouteGroupBuilder MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/");
        userService = new UserServiceImpl();

        group.MapPost("/users", (CreateUserDto createUserDto) =>
        {
            return userService.AddUser(createUserDto,userFireStore!);
        });

        group.MapGet("/users", () =>
        {
            return userService.GetAllUsers(userFireStore!);
        });

        group.MapGet("/users/{id}", (string id) =>{
            return userService.GetUserById(id, userFireStore!);
        });

        group.MapPost("/register", (RegisterUserDto dto) =>
        {
            return userService.Register(dto,userFireStore!);
        }).WithParameterValidation();

        group.MapPost("/login", (LoginUserDto dto) =>
        {
            return userService.Login(dto,userFireStore!);
        }).WithParameterValidation();

        group.MapGet("/users/token", ([FromHeader]string authorization) =>
        {
            return userService.GetUserByToken(authorization,userFireStore!);
        });

        // group.MapPost("/users/sendEmail", (EmailDto dto)=>{
        //     emailService.SendEmail(dto);
        // });

        group.MapGet("/users/verify/{token}" ,(string token)=>{
            return userService.VerifyUser(token,userFireStore!);
        });

        group.MapPost("/users/forgot-password/" ,(string email)=>{
            return userService.ForgetPassword(email,userFireStore!);
        });

        return group;
    }
}
