using Microsoft.AspNetCore.Mvc;
using StoreSp.Commonds;
using StoreSp.Dtos.request;
using StoreSp.Services;
using StoreSp.Services.Impl;

namespace StoreSp.Endpoints;

public static class UserEndpoint
{
    public static IUserService? userService { get; set; }

    public static RouteGroupBuilder MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/");
        userService = new UserServiceImpl();

        group.MapPost("/users", (CreateUserDto createUserDto) =>
        {
            return userService.AddUser(createUserDto);
        });

        group.MapGet("/users", () =>
        {
            return userService.GetAllUsers();
        });

        group.MapGet("/users/{id}", (string id) =>
        {
            return userService.GetUserById(id);
        });

        group.MapPost("/register", (RegisterUserDto dto) =>
        {
            return userService.Register(dto);
        }).WithParameterValidation();

        group.MapPost("/login", (LoginUserDto dto) =>
        {
            return userService.Login(dto);
        }).WithParameterValidation();

        group.MapPost("/admin/login", (LoginUserDto dto) =>
        {
            return userService.Login(dto);
        }).WithParameterValidation().RequireAuthorization("quan-tri-vien");

        group.MapGet("/users/token", ([FromHeader] string authorization) =>
        {
            return userService.GetUserByToken(authorization);
        });

        group.MapGet("/users/verify/{token}", (string token) =>
        {
            return userService.VerifyUser(token);
        });

        group.MapGet("/users/check-verify/{token}", (string token) =>
        {
            return userService.CheckVerify(token);
        });

        group.MapPost("/users/forgot-password/", ([FromBody] string email) =>
        {
            return userService.ForgetPassword(email);
        });

        group.MapPost("/users/reset-password/", (ResetPasswordDto dto) =>
        {
            return userService.ResetPassword(dto);
        }).WithParameterValidation();

        group.MapPost("/users/google-login", (GoogleLoginDto dto) =>
        {
            return userService.GoogleLogin(dto);
        });

        group.MapPost("/users/google-register", (GoogleRegisterDto dto) =>
        {
            return userService.GoogleRegister(dto);
        });

        group.MapGet("/users/role", ([FromQuery] string code) =>
        {
            return userService.GetUserByRole(code);
        });

        group.MapGet("/test", () =>
        {
            return VariableConfig.Application["a"];
        });
        return group;
    }
}
