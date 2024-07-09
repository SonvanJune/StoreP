using Google.Cloud.Firestore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using StoreSp.Converters;
using StoreSp.Converters.request;
using StoreSp.Converters.response;
using StoreSp.Dtos.request;
using StoreSp.Dtos.response;
using StoreSp.Entities;

namespace StoreSp.Stores;

public class UserFireStore(FirestoreDb firestoreDb) : FirestoreService(firestoreDb)
{
    ///
    private const string _collectionUser = "Users";
    private const string _collectionRole = "Roles";
    public readonly IBaseConverter<User, UserDto> userConverter = new UserConverter();
    private readonly IBaseConverter<User, CreateUserDto> createUserConverter = new CreateUserConverter();
    private readonly IBaseConverter<User, RegisterUserDto> registerUserConverter = new RegisterUserConverter();

    public Task<List<UserDto>> GetAllUser()
    {
        var snapshot = base.GetSnapshots(_collectionUser);
        var user = snapshot.Documents.Select(s => s.ConvertTo<User>()).ToList();
        return Task.FromResult(user.Select(userConverter.ToDto).ToList());
    }

    public Task<UserDto> GetUser(string id)
    {
        var snapshot = base.GetSnapshots(_collectionUser);
        var user = snapshot.Documents.Select(s => s.ConvertTo<User>()).ToList().Find(u => u.Id == id);
        return Task.FromResult(userConverter.ToDto(user!));
    }

    public Task Add(CreateUserDto userDto)
    {
        var userDb = _firestoreDb.Collection(_collectionUser);
        var roleDb = base.GetSnapshots(_collectionRole);
        var user = createUserConverter.ToEntity(userDto);
        var role = roleDb.Documents.Select(r => r.ConvertTo<Role>()).ToList().Find(r => r.Id == userDto.RoleId);
        if (role != null)
        {
            user.roleId = role.Id;
            user.role = role;
        }
        return userDb.AddAsync(user);
    }

    public User Register(RegisterUserDto userDto)
    {
        var userDb = _firestoreDb.Collection(_collectionUser);
        var roleDb = base.GetSnapshots(_collectionRole);

        var user = registerUserConverter.ToEntity(userDto);
        var role = roleDb.Documents.Select(r => r.ConvertTo<Role>()).ToList().Find(r => r.Id == userDto.RoleId);
        if (role == null)
        {
            throw new InvalidOperationException("Role not found");
        }
        user.roleId = role.Id;
        user.role = role;
        userDb.AddAsync(user);
        return user;
    }

    public User Login(LoginUserDto userDto){
        var userDb = base.GetSnapshots(_collectionUser);
        var roleDb = base.GetSnapshots(_collectionRole);

        var user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == userDto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(userDto.Password, user.Password))
        {
            return null!;
        }
        var role = roleDb.Documents.Select(r => r.ConvertTo<Role>()).ToList().Find(r => r.Id == user!.roleId);
        user.role = role;
        return user;
    }

    public UserDto GetUserByEmail(string email){
        var userDb = base.GetSnapshots(_collectionUser);
        var user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == email);
        if (user == null)
        {
            return null!;
        }
        return userConverter.ToDto(user);
    }
}
