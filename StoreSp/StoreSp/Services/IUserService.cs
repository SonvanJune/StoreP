using StoreSp.Dtos.request;
using StoreSp.Stores;

namespace StoreSp.Services;

public interface IUserService
{
   public IResult AddUser(CreateUserDto createUserDto, UserFireStore userFireStore);
   public IResult GetAllUsers(UserFireStore userFireStore);
   public IResult GetUserById(string id, UserFireStore userFireStore);
   public IResult Register(RegisterUserDto registerUserDto, UserFireStore userFireStore);
   public IResult Login(LoginUserDto loginUserDto, UserFireStore userFireStore);
   public IResult GetUserByToken(string token,UserFireStore userFireStore);
   public IResult VerifyUser(string token,UserFireStore userFireStore);
   public IResult ForgetPassword(string email, UserFireStore userFireStore);
   public IResult ResetPassword(ResetPasswordDto dto, UserFireStore userFireStore);
}
