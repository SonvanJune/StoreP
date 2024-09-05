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

    public static UserFireStore userFireStore = new UserFireStore();
    public static SystemFirestore systemFirestore = new SystemFirestore();

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
            message = "Tạo thành công",
            data = null
        });
    }

    IResult IUserService.GetAllUsers()
    {
        return Results.Ok(new HttpStatusConfig
        {
            status = HttpStatusCode.OK,
            message = "Thành công",
            data = userFireStore!.GetAllUser().Result
        });
    }

    IResult IUserService.GetUserById(string id)
    {
        return Results.Ok(new HttpStatusConfig
        {
            status = HttpStatusCode.OK,
            message = "Thành công",
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
                message = "Không tìm thấy cơ sở dữ liệu",
                data = null
            });
        }

        var user = userFireStore!.Register(registerUserDto).Result;
        if (user == null)
        {
            return Results.BadRequest(new HttpStatusConfig
            {
                status = HttpStatusCode.BadRequest,
                message = "Email hoặc số điện thoại đã tồn tại!!",
                data = null
            });
        }

        if (user.Email != null && user.Email != "")
        {
            emailService!.SendEmail(new EmailDto
            {
                Email = user.Email,
                Subject = "Xác thực email",
                Message = EmailFormConfig.EMAIL_VERIFY($"http://localhost:5181/api/users/email/verify/{user.VerificationToken}", user.Email, "http://localhost:5181")
            });
        }

        return Results.Created("", new HttpStatusConfig
        {
            status = HttpStatusCode.Created,
            message = "Đăng ký thành công",
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
                message = "Không tìm thấy cơ sở dữ liệu",
                data = null
            });
        }

        var user = userFireStore.Login(loginUserDto).Result;
        if (user != null)
        {
            if (user.IsGoogleAccount == 1)
            {
                return Results.BadRequest(new HttpStatusConfig
                {
                    status = HttpStatusCode.UnprocessableEntity,
                    message = "Tài khoản này không phải tài khoản hệ thống",
                    data = null
                });
            }
            //nam thang ngay mac dinh 1111/11/11 
            if (user.VerifiedAt.Year == 1111)
            {
                return Results.BadRequest(new HttpStatusConfig
                {
                    status = HttpStatusCode.UnprocessableEntity,
                    message = "Người dùng vânx chưa xác thực",
                    data = null
                });
            }
            return Results.Ok(new HttpStatusConfig
            {
                status = HttpStatusCode.OK,
                message = "Đăng nhập thành công",
                data = new UserTokenDto
                {
                    Token = authService!.GenerateToken(user),
                    User = userFireStore.userConverter.ToDto(user),
                    RoleCode = user!.Role!.Code
                }
            });
        }
        else
        {
            return Results.BadRequest(new HttpStatusConfig
            {
                status = HttpStatusCode.UnprocessableEntity,
                message = "Email , số điện thoại hoặc mật khẩu không đúng",
                data = null
            });
        }
    }

    IResult IUserService.GetUserByToken(TokenDto tokenDto)
    {
        if (tokenDto.RefreshToken is null && tokenDto.Token != null)
        {
            if (authService!.ValidateToken(tokenDto.Token))
            {
                var email = authService!.GetFirstByToken(tokenDto.Token);
                var user = userFireStore!.GetUserByUsername(email);
                if (user != null)
                {
                    return Results.Ok(new HttpStatusConfig
                    {
                        status = HttpStatusCode.OK,
                        message = "Thành công",
                        data = null
                    });
                }
                else
                {
                    return Results.BadRequest(new HttpStatusConfig
                    {
                        status = HttpStatusCode.BadRequest,
                        message = "Không tìm thấy user",
                        data = null
                    });
                }
            }
            return Results.BadRequest(new HttpStatusConfig
            {
                status = HttpStatusCode.BadRequest,
                message = "Token hết hạn",
                data = null
            });
        }
        else
        {
            if (authService!.ValidateToken(tokenDto.RefreshToken!))
            {
                var email = authService!.GetFirstByToken(tokenDto.RefreshToken!);
                var user = userFireStore!.GetUserByUsername(email);
                return Results.Ok(new HttpStatusConfig
                {
                    status = HttpStatusCode.OK,
                    message = "ok",
                    data = new UserTokenDto
                    {
                        Token = authService!.GenerateToken(user),
                        User = userFireStore.userConverter.ToDto(user),
                        RoleCode = user!.Role!.Code
                    }
                });
            }
            return Results.BadRequest(new HttpStatusConfig
            {
                status = HttpStatusCode.BadRequest,
                message = "Refresh token has expired",
                data = null
            });
        }
    }

    IResult IUserService.VerifyUserByEmail(string token)
    {
        if (authService!.ValidateToken(token))
        {
            var email = authService!.GetFirstByToken(token);
            if (!userFireStore!.CheckValidToken(email, token))
            {
                return Results.BadRequest(new HttpStatusConfig
                {
                    status = HttpStatusCode.BadRequest,
                    message = "Token không hợp lệ",
                    data = null
                });
            }

            if (userFireStore.CheckIsVerified(email))
            {
                return Results.BadRequest(new HttpStatusConfig
                {
                    status = HttpStatusCode.BadRequest,
                    message = "Token hết hạn",
                    data = null
                });
            }

            if (userFireStore.VerifyUser(email) != null)
            {
                return Results.Ok(new HttpStatusConfig
                {
                    status = HttpStatusCode.OK,
                    message = "Ngươif dùng xác thực thành công",
                    data = null
                });
            }
            else
            {
                return Results.BadRequest(new HttpStatusConfig
                {
                    status = HttpStatusCode.BadRequest,
                    message = "Không tìm thấy người dùng",
                    data = null
                });
            }
        }
        else
        {
            return Results.BadRequest(new HttpStatusConfig
            {
                status = HttpStatusCode.BadRequest,
                message = "Token hết hạn",
                data = null
            });
        }
    }

    IResult IUserService.ForgetPasswordByEmail(string email)
    {
        string genCode = Convert.ToHexString(RandomNumberGenerator.GetBytes(2));
        if (userFireStore!.ForgetPaswordByEmail(email, genCode).Result != null)
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
                message = "Yêu cầu đã được chấp nhận",
                data = null
            });
        }
        else
        {
            return Results.BadRequest(new HttpStatusConfig
            {
                status = HttpStatusCode.BadRequest,
                message = "Không thể tìm thấy tài khoản, có thể bạn đăng nhập bằng google",
                data = null
            });
        }
    }

    IResult IUserService.ResetPasswordOfEmail(ResetPasswordDto dto)
    {
        if (userFireStore!.ResetPasswordOfEmail(dto).Result != null)
        {
            return Results.Ok(new HttpStatusConfig
            {
                status = HttpStatusCode.OK,
                message = "Mật khẩu đã được thay đổi thành công",
                data = null
            });
        }
        else
        {
            return Results.BadRequest(new HttpStatusConfig
            {
                status = HttpStatusCode.BadRequest,
                message = "Mã không hợp lệ",
                data = null
            });
        }
    }

    IResult IUserService.CheckVerify(string token)
    {
        if (authService!.ValidateToken(token))
        {
            var username = authService!.GetFirstByToken(token);
            var user = userFireStore!.GetUserByUsername(username);
            if (user != null)
            {
                if (user.VerifiedAt.Year != 1111)
                {
                    return Results.Ok(new HttpStatusConfig
                    {
                        status = HttpStatusCode.OK,
                        message = "Người dùng đã xác thực thành công",
                        data = new UserTokenDto
                        {
                            Token = authService!.GenerateToken(user),
                            User = userFireStore.userConverter.ToDto(user),
                            RoleCode = user!.Role!.Code
                        }
                    });
                }
                else
                {
                    return Results.BadRequest(new HttpStatusConfig
                    {
                        status = HttpStatusCode.BadRequest,
                        message = "Người dùng vẫn chưa xác thực",
                        data = null
                    });
                }
            }
            else
            {
                return Results.BadRequest(new HttpStatusConfig
                {
                    status = HttpStatusCode.BadRequest,
                    message = "Không thể tìm thấy người dùng",
                    data = null
                });
            }
        }
        else
        {
            return Results.BadRequest(new HttpStatusConfig
            {
                status = HttpStatusCode.BadRequest,
                message = "Token hết hạn",
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
                message = "Taif khoản google này chưa đươcj kích hoạt",
                data = null
            });
        }

        var user = userFireStore!.GoogleRegister(googleRegisterDto).Result;
        if (user == null)
        {
            return Results.BadRequest(new HttpStatusConfig
            {
                status = HttpStatusCode.BadRequest,
                message = "Tài khoản này đã tồn tại, mời bạn đăng nhập",
                data = null
            });
        }
        return Results.Created("", new HttpStatusConfig
        {
            status = HttpStatusCode.Created,
            message = "Đăng ký thành công",
            data = new UserTokenDto
            {
                Token = authService!.GenerateToken(user),
                User = userFireStore.userConverter.ToDto(user),
                RoleCode = user!.Role!.Code
            }
        });
    }

    IResult IUserService.GoogleLogin(GoogleLoginDto googleLoginDto)
    {

        if (googleLoginDto.EmailVerified == false)
        {
            return Results.BadRequest(new HttpStatusConfig
            {
                status = HttpStatusCode.BadRequest,
                message = "Tài khoản google này chưa được kích hoạt",
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


        var user = userFireStore.GoogleLogin(googleLoginDto).Result;
        if (user != null)
        {
            //nam thang ngay mac dinh 1111/11/11 
            if (user.VerifiedAt.Year == 1111)
            {
                return Results.BadRequest(new HttpStatusConfig
                {
                    status = HttpStatusCode.UnprocessableEntity,
                    message = "Người dùng chưa xác thực",
                    data = null
                });
            }
            return Results.Ok(new HttpStatusConfig
            {
                status = HttpStatusCode.OK,
                message = "Đăng nhập thành công",
                data = new UserTokenDto
                {
                    Token = authService!.GenerateToken(user),
                    User = userFireStore.userConverter.ToDto(user),
                    RoleCode = user!.Role!.Code
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

    IResult IUserService.GetUserByRole(string roleCode)
    {
        if (userFireStore!.GetUserByRole(roleCode) == null)
        {
            return Results.NotFound(new HttpStatusConfig
            {
                status = HttpStatusCode.NotFound,
                message = "Không tìm thấy",
                data = null
            });
        }

        return Results.Ok(new HttpStatusConfig
        {
            status = HttpStatusCode.OK,
            message = "Thành công",
            data = userFireStore!.GetUserByRole(roleCode).Result
        });
    }

    IResult IUserService.UpdateStatusUser(UpdateStatusUserDto dto)
    {
        if (userFireStore!.UpdateStatus(dto).Result == null)
        {
            return Results.BadRequest(new HttpStatusConfig
            {
                status = HttpStatusCode.BadRequest,
                message = "Không tìm thấy người dùng",
                data = null
            });
        }
        else
        {
            return Results.Ok(new HttpStatusConfig
            {
                status = HttpStatusCode.Created,
                message = "Cập nhật người dùng thành công",
                data = null
            });
        }
    }

    IResult IUserService.CheckResetCode(ResetCodeDto dto)
    {
        if (userFireStore!.CheckResetCode(dto).Result != null)
        {
            return Results.Ok(new HttpStatusConfig
            {
                status = HttpStatusCode.OK,
                message = "Mã chính xác",
                data = null
            });
        }
        else
        {
            return Results.BadRequest(new HttpStatusConfig
            {
                status = HttpStatusCode.BadRequest,
                message = "Mã này không hợp lệ",
                data = null
            });
        }
    }

    IResult IUserService.VerifyUserByPhone(string token)
    {
        if (authService!.ValidateToken(token))
        {
            var phone = authService!.GetFirstByToken(token);

            if (userFireStore!.CheckIsVerified(phone))
            {
                return Results.BadRequest(new HttpStatusConfig
                {
                    status = HttpStatusCode.BadRequest,
                    message = "Token hết hạn",
                    data = null
                });
            }

            if (userFireStore.VerifyUser(phone) != null)
            {
                var user = userFireStore.VerifyUser(phone).Result;
                return Results.Ok(new HttpStatusConfig
                {
                    status = HttpStatusCode.OK,
                    message = "người dùng xác thực thành công",
                    data = new UserTokenDto
                    {
                        Token = authService!.GenerateToken(user),
                        User = userFireStore.userConverter.ToDto(user),
                        RoleCode = user!.Role!.Code
                    }
                });
            }
            else
            {
                return Results.BadRequest(new HttpStatusConfig
                {
                    status = HttpStatusCode.BadRequest,
                    message = "Không thể tìm thấy người dùng",
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

    IResult IUserService.AddAdress(CreateAddressDto dto)
    {
        if (userFireStore!.AddAdress(dto).Result is null)
        {
            return Results.BadRequest(new HttpStatusConfig
            {
                status = HttpStatusCode.NotFound,
                message = "Không tìm thấy người dùng",
                data = null
            });
        }


        return Results.Created("", new HttpStatusConfig
        {
            status = HttpStatusCode.Created,
            message = "Tạo thành công",
            data = null
        });
    }

    IResult IUserService.GetAddress(string username)
    {
        return Results.Ok(new HttpStatusConfig
        {
            status = HttpStatusCode.OK,
            message = "Thành công",
            data = userFireStore!.GetAddress(username).Result
        });
    }

    IResult IUserService.UpdateUser(UpdateUserDto dto, string username)
    {
        if (userFireStore!.UpdateUser(dto , username).Result is null)
        {
            return Results.BadRequest(new HttpStatusConfig
            {
                status = HttpStatusCode.NotFound,
                message = "Không tìm thấy user",
                data = null
            });
        }


        return Results.Created("", new HttpStatusConfig
        {
            status = HttpStatusCode.Created,
            message = "Cập nhật thành công",
            data = null
        });
    }

    IResult IUserService.GetCount(string username)
    {
        return Results.Ok(new HttpStatusConfig
        {
            status = HttpStatusCode.OK,
            message = "Thành công",
            data = systemFirestore!.GetCount(username)
        });
    }
}
