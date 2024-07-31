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
        }).WithParameterValidation();

        group.MapGet("/users/token", ([FromHeader] string authorization) =>
        {
            return userService.GetUserByToken(authorization);
        });

        group.MapGet("/users/email/verify/{token}", (string token) =>
        {
            return userService.VerifyUserByEmail(token);
        });

        group.MapGet("/users/phone/verify/{token}", (string token) =>
        {
            return userService.VerifyUserByPhone(token);
        });

        group.MapGet("/users/check-verify/{token}", (string token) =>
        {
            return userService.CheckVerify(token);
        });

        group.MapPost("/users/email/forgot-password/", (ForgetPasswordDto dto) =>
        {
            return userService.ForgetPasswordByEmail(dto.Email);
        }).WithParameterValidation();

        group.MapPost("/users/email/check-reset-code/", (ResetCodeDto dto) =>
        {
            return userService.CheckResetCode(dto);
        }).WithParameterValidation();

        group.MapPost("/users/email/reset-password/", (ResetPasswordDto dto) =>
        {
            return userService.ResetPasswordOfEmail(dto);
        }).WithParameterValidation();

        group.MapPost("/users/google-login", (GoogleLoginDto dto) =>
        {
            return userService.GoogleLogin(dto);
        }).WithParameterValidation();

        group.MapPost("/users/google-register", (GoogleRegisterDto dto) =>
        {
            return userService.GoogleRegister(dto);
        }).WithParameterValidation();

        group.MapPost("/users/address", (CreateAddressDto dto) =>
        {
            return userService.AddAdress(dto);
        }).WithParameterValidation();

        group.MapGet("/users/address/{username}", (string username) =>
        {
            return userService.GetAddress(username);
        });

        group.MapGet("/users/role", ([FromQuery] string code) =>
        {
            return userService.GetUserByRole(code);
        }).RequireAuthorization("quan-tri-vien");

        group.MapGet("/test", () =>
        {
            return "";
        });

        group.MapPost("/users/update-status", (UpdateStatusUserDto dto) =>
        {
            return userService.UpdateStatusUser(dto);
        }).WithParameterValidation().RequireAuthorization("quan-tri-vien");
        return group;
    }
}
