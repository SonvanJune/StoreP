using StoreSp.Dtos.request;

namespace StoreSp.Services;

public interface IUserService
{
   public IResult AddUser(CreateUserDto createUserDto, UserFireStore userFireStore);
   public IResult GetAllUsers(UserFireStore userFireStore);
   public IResult GetUserById(string id, UserFireStore userFireStore);
}
