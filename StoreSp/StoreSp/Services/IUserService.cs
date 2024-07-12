using StoreSp.Dtos.request;
using StoreSp.Stores;

namespace StoreSp.Services;

public interface IUserService
{
   public IResult AddUser(CreateUserDto createUserDto);
   public IResult GetAllUsers();
   public IResult GetUserById(string id);
   public IResult Register(RegisterUserDto registerUserDto);
   public IResult CheckVerify(string toke);
   public IResult Login(LoginUserDto loginUserDto);
   public IResult GetUserByToken(string toke);
   public IResult VerifyUser(string toke);
   public IResult ForgetPassword(string email);
   public IResult ResetPassword(ResetPasswordDto dto);
}
