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

    public static UserFireStore? userFireStore { get; set; }

    public UserServiceImpl()
    {
        authService = new AuthServiceImpl();
        emailService = new EmailServiceImpl();
    }

    IResult IUserService.AddUser(CreateUserDto createUserDto)
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

    IResult IUserService.GetAllUsers()
    {
        return Results.Ok(new HttpStatusConfig
        {
            status = HttpStatusCode.OK,
            message = "Success",
            data = userFireStore!.GetAllUser().Result
        });
    }

    IResult IUserService.GetUserById(string id)
    {
        return Results.Ok(new HttpStatusConfig
        {
            status = HttpStatusCode.OK,
            message = "Success",
            data = userFireStore!.GetUser(id).Result
        });
    }

    IResult IUserService.Register(RegisterUserDto registerUserDto)
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
        if (user == null)
        {
            return Results.BadRequest(new HttpStatusConfig
            {
                status = HttpStatusCode.BadRequest,
                message = "Email or Phone already exists",
                data = null
            });
        }

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
            data = authService!.GenerateToken(user)
        });
    }

    IResult IUserService.Login(LoginUserDto loginUserDto)
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

    IResult IUserService.GetUserByToken(string authorization)
    {
        string[] arrListStr = authorization.Split(' ');
        string token = arrListStr[1];
        if (authService!.ValidateToken(token))
        {
            var email = authService!.GetFirstByToken(token);
            if (userFireStore!.GetUserByEmail(email) != null)
            {
                var user = userFireStore.GetUserByEmail(email);
                return Results.Ok(new HttpStatusConfig
                {
                    status = HttpStatusCode.OK,
                    message = "Success",
                    data = userFireStore.userConverter.ToDto(user)
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

    IResult IUserService.VerifyUser(string token)
    {
        if (authService!.ValidateToken(token))
        {
            var email = authService!.GetFirstByToken(token);
            if (!userFireStore!.CheckValidToken(email, token))
            {
                return Results.BadRequest(new HttpStatusConfig
                {
                    status = HttpStatusCode.BadRequest,
                    message = "Token invalid",
                    data = null
                });
            }

            if (userFireStore.CheckIsVerified(email))
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

    IResult IUserService.ForgetPassword(string email)
    {
        string genCode = Convert.ToHexString(RandomNumberGenerator.GetBytes(4));
        if (userFireStore!.ForgetPasword(email, genCode).Result != null)
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
                message = "Can not find user or may be you registered with google account",
                data = null
            });
        }
    }

    IResult IUserService.ResetPassword(ResetPasswordDto dto)
    {
        if (userFireStore!.ResetPassword(dto).Result != null)
        {
            return Results.Ok(new HttpStatusConfig
            {
                status = HttpStatusCode.OK,
                message = "Password has been changed successfully",
                data = null
            });
        }
        else
        {
            return Results.BadRequest(new HttpStatusConfig
            {
                status = HttpStatusCode.BadRequest,
                message = "This code is not valid",
                data = null
            });
        }
    }

    IResult IUserService.CheckVerify(string token)
    {
        if (authService!.ValidateToken(token))
        {
            var email = authService!.GetFirstByToken(token);
            if (userFireStore!.GetUserByEmail(email) != null)
            {
                var user = userFireStore.GetUserByEmail(email);
                if (user.VerifiedAt.ToDateTime().Year != 1111)
                {
                    return Results.Ok(new HttpStatusConfig
                    {
                        status = HttpStatusCode.OK,
                        message = "User has been verified",
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
                        status = HttpStatusCode.BadRequest,
                        message = "User has not been verified yet",
                        data = null
                    });
                }
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

    IResult IUserService.GoogleRegister(GoogleRegisterDto googleRegisterDto)
    {
        if (googleRegisterDto.EmailVerified == false)
        {
            return Results.BadRequest(new HttpStatusConfig
            {
                status = HttpStatusCode.BadRequest,
                message = "Tai khoan google nay chua duoc kich hoat",
                data = null
            });
        }

        var user = userFireStore!.GoogleRegister(googleRegisterDto);
        if (user == null)
        {
            return Results.BadRequest(new HttpStatusConfig
            {
                status = HttpStatusCode.BadRequest,
                message = "Tai khoan nay da ton tai, moi ban dang nhap",
                data = null
            });
        }
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

    IResult IUserService.GoogleLogin(GoogleLoginDto googleLoginDto){

        if (googleLoginDto.EmailVerified == false)
        {
            return Results.BadRequest(new HttpStatusConfig
            {
                status = HttpStatusCode.BadRequest,
                message = "Tai khoan google nay chua duoc kich hoat",
                data = null
            });
        }

        if (userFireStore is null)
        {
            return Results.NotFound(new HttpStatusConfig
            {
                status = HttpStatusCode.UnprocessableEntity,
                message = "Database not found",
                data = null
            });
        }

        if (userFireStore.GoogleLogin(googleLoginDto) != null)
        {
            var user = userFireStore.GoogleLogin(googleLoginDto);
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
                message = "The account not found or may be you registered this email by normal register of app",
                data = null
            });
        }
    }
}
