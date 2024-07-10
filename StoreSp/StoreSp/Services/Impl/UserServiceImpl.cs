namespace StoreSp.Services.Impl;

using System.Net;
using StoreSp.Stores;
using StoreSp.Commonds;
using StoreSp.Dtos.request;
using StoreSp.Dtos.response;
using System.Security.Cryptography;

public class UserServiceImpl : IUserService
{
    public IAuthService? authService { get; set; }
    public IEmailService? emailService { get; set; }

    public UserServiceImpl()
    {
        authService = new AuthServiceImpl();
        emailService = new EmailServiceImpl();
    }

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

    public IResult Register(RegisterUserDto registerUserDto, UserFireStore userFireStore)
    {
        if (userFireStore is null)
        {
            return Results.NotFound(new HttpStatusConfig
            {
                status = HttpStatusCode.Created,
                message = "Database not found",
                data = null
            });
        }

        var user = userFireStore!.Register(registerUserDto);
        emailService!.SendEmail(new EmailDto
        {
            Email = user.Email,
            Subject = "Xac thuc email",
            Message = EmailFormConfig.EMAIL_VERIFY($"http://localhost:5181/api/users/verify/{user.VerificationToken}", user.Email, "http://localhost:5181")
        });
        return Results.Created("", new HttpStatusConfig
        {
            status = HttpStatusCode.Created,
            message = "Register Success",
            data = new UserTokenDto
            {
                Token = authService!.GenerateToken(user),
                User = userFireStore.userConverter.ToDto(user)
            }
        });
    }

    public IResult Login(LoginUserDto loginUserDto, UserFireStore userFireStore)
    {
        if (userFireStore is null)
        {
            return Results.NotFound(new HttpStatusConfig
            {
                status = HttpStatusCode.UnprocessableEntity,
                message = "Database not found",
                data = null
            });
        }

        if (userFireStore.Login(loginUserDto) != null)
        {
            var user = userFireStore.Login(loginUserDto);
            //nam thang ngay mac dinh 1111/11/11 
            if (user.VerifiedAt.ToDateTime().Year == 1111)
            {
                return Results.BadRequest(new HttpStatusConfig
                {
                    status = HttpStatusCode.UnprocessableEntity,
                    message = "User not verified yet",
                    data = null
                });
            }
            return Results.Ok(new HttpStatusConfig
            {
                status = HttpStatusCode.OK,
                message = "Login success",
                data = new UserTokenDto
                {
                    Token = authService!.GenerateToken(user),
                    User = userFireStore.userConverter.ToDto(user)
                }
            });
        }
        else
        {
            return Results.BadRequest(new HttpStatusConfig
            {
                status = HttpStatusCode.UnprocessableEntity,
                message = "Email , Phone or password incorrect",
                data = null
            });
        }
    }

    public IResult GetUserByToken(string authorization, UserFireStore userFireStore)
    {
        string[] arrListStr = authorization.Split(' ');
        string token = arrListStr[1];
        if (authService!.ValidateToken(token))
        {
            var email = authService!.GetEmailByToken(token);
            if (userFireStore.GetUserByEmail(email) != null)
            {
                var user = userFireStore.GetUserByEmail(email);
                return Results.Ok(new HttpStatusConfig
                {
                    status = HttpStatusCode.OK,
                    message = "Success",
                    data = user
                });
            }
            else
            {
                return Results.BadRequest(new HttpStatusConfig
                {
                    status = HttpStatusCode.BadRequest,
                    message = "Can not find user",
                    data = null
                });
            }
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
    }

    public IResult VerifyUser(string token, UserFireStore userFireStore)
    {
        if (authService!.ValidateToken(token))
        {
            var email = authService!.GetEmailByToken(token);
            if (!userFireStore.checkValidToken(email, token))
            {
                return Results.BadRequest(new HttpStatusConfig
                {
                    status = HttpStatusCode.BadRequest,
                    message = "Token invalid",
                    data = null
                });
            }

            if (userFireStore.checkIsVerified(email))
            {
                return Results.BadRequest(new HttpStatusConfig
                {
                    status = HttpStatusCode.BadRequest,
                    message = "Token has expired",
                    data = null
                });
            }

            if (userFireStore.VerifyUser(email) != null)
            {
                return Results.Ok(new HttpStatusConfig
                {
                    status = HttpStatusCode.OK,
                    message = "USer verified successfully",
                    data = null
                });
            }
            else
            {
                return Results.BadRequest(new HttpStatusConfig
                {
                    status = HttpStatusCode.BadRequest,
                    message = "Can not find user",
                    data = null
                });
            }
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
    }

    public IResult ForgetPassword(string email, UserFireStore userFireStore)
    {
        string genCode = Convert.ToHexString(RandomNumberGenerator.GetBytes(4));
        if (userFireStore.ForgetPasword(email, genCode).Result != null)
        {
            emailService!.SendEmail(new EmailDto
            {
                Email = email,
                Subject = "Quên mật khẩu!!",
                Message = EmailFormConfig.EMAIL_FORGET_PASSWORD(genCode, email, "http://localhost:5181")
            });
            return Results.Ok(new HttpStatusConfig
            {
                status = HttpStatusCode.OK,
                message = "Request has accepted",
                data = null
            });
        }
        else
        {
            return Results.BadRequest(new HttpStatusConfig
            {
                status = HttpStatusCode.BadRequest,
                message = "Can not find user",
                data = null
            });
        }
    }
}
