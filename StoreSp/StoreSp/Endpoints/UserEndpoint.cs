using System.Net;
using Microsoft.AspNetCore.Mvc;
using StoreSp.Commonds;
using StoreSp.Dtos.request;
using StoreSp.Services;
using StoreSp.Services.Impl;

namespace StoreSp.Endpoints;

public static class UserEndpoint
{
    public static IUserService? userService { get; set; }
    public static INotificationService? notificationService { get; set; }
    public static IAuthService? authService { get; set; }

    public static RouteGroupBuilder MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/");
        userService = new UserServiceImpl();
        authService = new AuthServiceImpl();
        notificationService = new NotificationServiceImpl();

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

        group.MapGet("/users/notifications/{username}", (string username, [FromQuery] int status) =>
        {
            return notificationService!.GetNotifications(username, status);
        });

        group.MapGet("/users/system", ([FromHeader] string authorization) =>
        {
            if (authService.GetResult(authorization) == 1)
            {
                string[] str = authorization.Split(' ');
                var username = authService.GetFirstByToken(str[1]);
                return userService.GetCount(username);
            }
            else
            {
                return Results.BadRequest(new HttpStatusConfig
                {
                    status = HttpStatusCode.BadRequest,
                    message = "Token has expired",
                    data = null
                });
            }
        }).RequireAuthorization();

        group.MapPost("/users/notifications/read/{notificationId}", (string notificationId) =>
        {
            return notificationService!.ReadNotification(notificationId);
        });

        group.MapPost("/users/notifications/makeNotRead/{notificationId}", (string notificationId) =>
        {
            return notificationService!.MakeNotReadNotification(notificationId);
        });

        group.MapDelete("/users/notifications/read/all/{username}", (string username) =>
        {
            return notificationService!.ReadALLNotification(username);
        });

        group.MapDelete("/users/notifications/delete/{notificationId}", (string notificationId) =>
        {
            return notificationService!.DeleteNotification(notificationId);
        });

        group.MapDelete("/users/notifications/delete/all/{username}", (string username, [FromQuery] int status) =>
        {
            return notificationService!.DeleteAllNotifications(username, status);
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

        group.MapPost("/users/token", (TokenDto tokenDto) =>
        {
            return userService.GetUserByToken(tokenDto);
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

        group.MapPost("/users/address", (CreateAddressDto dto, [FromHeader] string authorization) =>
        {
            if (authService.GetResult(authorization) == 1)
            {
                return userService.AddAdress(dto);
            }
            else
            {
                return Results.BadRequest(new HttpStatusConfig
                {
                    status = HttpStatusCode.BadRequest,
                    message = "Token has expired",
                    data = null
                });
            }

        }).WithParameterValidation().RequireAuthorization("nguoi-mua");

        group.MapGet("/users/address/{username}", (string username, [FromHeader] string authorization) =>
        {
            if (authService.GetResult(authorization) == 1)
            {
                return userService.GetAddress(username);
            }
            else
            {
                return Results.BadRequest(new HttpStatusConfig
                {
                    status = HttpStatusCode.BadRequest,
                    message = "Token has expired",
                    data = null
                });
            }
        });

        group.MapGet("/users/role", ([FromQuery] string code, [FromHeader] string authorization) =>
        {
            if (authService.GetResult(authorization) == 1)
            {
                return userService.GetUserByRole(code);
            }
            else
            {
                return Results.BadRequest(new HttpStatusConfig
                {
                    status = HttpStatusCode.BadRequest,
                    message = "Token has expired",
                    data = null
                });
            }
        }).RequireAuthorization("quan-tri-vien");

        group.MapGet("/test", () =>
        {
            return "";
        });

        group.MapPost("/users/update-status", (UpdateStatusUserDto dto, [FromHeader] string authorization) =>
        {
            if (authService.GetResult(authorization) == 1)
            {
                return userService.UpdateStatusUser(dto);
            }
            else
            {
                return Results.BadRequest(new HttpStatusConfig
                {
                    status = HttpStatusCode.BadRequest,
                    message = "Token has expired",
                    data = null
                });
            }
        }).WithParameterValidation().RequireAuthorization("quan-tri-vien");

        group.MapPost("/users/update-profile", (UpdateUserDto dto, [FromHeader] string authorization) =>
        {
            if (authService.GetResult(authorization) == 1)
            {
                string[] str = authorization.Split(' ');
                var username = authService.GetFirstByToken(str[1]);
                return userService.UpdateUser(dto, username);
            }
            else
            {
                return Results.BadRequest(new HttpStatusConfig
                {
                    status = HttpStatusCode.BadRequest,
                    message = "Token has expired",
                    data = null
                });
            }
        }).WithParameterValidation().RequireAuthorization();
        return group;
    }
}
