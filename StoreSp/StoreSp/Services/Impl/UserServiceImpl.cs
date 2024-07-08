namespace StoreSp.Services.Impl;

using System.Net;
using StoreSp.Stores;
using StoreSp.Commons;
using StoreSp.Dtos.request;
using StoreSp.Dtos.response;

public class UserServiceImpl : IUserService
{
    public IAuthService? authService { get; set; }

    public UserServiceImpl()
    {
        authService = new AuthServiceImpl();
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

        userFireStore!.Register(registerUserDto);
        return Results.Created("", new HttpStatusConfig
        {
            status = HttpStatusCode.Created,
            message = "Register Success",
            data = null
        });
    }

    public IResult Login(LoginUserDto loginUserDto, UserFireStore userFireStore)
    {
        if (userFireStore is null)
        {
            return Results.NotFound(new HttpStatusConfig
            {
                status = HttpStatusCode.BadRequest,
                message = "Database not found",
                data = null
            });
        }

        if (userFireStore.Login(loginUserDto) != null)
        {
            var user = userFireStore.Login(loginUserDto);
            return Results.Ok(new HttpStatusConfig
            {
                status = HttpStatusCode.OK,
                message = "Login success",
                data = authService!.GenerateToken(user)
            });
        }
        else
        {
            return Results.BadRequest(new HttpStatusConfig
            {
                status = HttpStatusCode.BadRequest,
                message = "Email or password incorrect",
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
        else{
            return Results.BadRequest(new HttpStatusConfig
            {
                status = HttpStatusCode.BadRequest,
                message = "Token has expired",
                data = null
            });
        }
    }
}
