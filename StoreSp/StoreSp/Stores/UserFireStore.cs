using Google.Cloud.Firestore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using StoreSp.Converters;
using StoreSp.Converters.request;
using StoreSp.Converters.response;
using StoreSp.Dtos.request;
using StoreSp.Dtos.response;
using StoreSp.Entities;
using StoreSp.Services;
using StoreSp.Services.Impl;

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
        user.VerificationToken = AuthServiceImpl.CreateRandomToken(user);
        userDb.AddAsync(user);
        return user;
    }

    public User Login(LoginUserDto loginUserDto)
    {
        var userDb = base.GetSnapshots(_collectionUser);
        var roleDb = base.GetSnapshots(_collectionRole);
        User user;

        if (userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == loginUserDto.Username) == null)
        {
            if (userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Phone == loginUserDto.Username) == null)
            {
                return null!;
            }
            else
            {
                user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Phone == loginUserDto.Username)!;
            }
        }
        else
        {
            user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == loginUserDto.Username)!;
        }

        if (!BCrypt.Net.BCrypt.Verify(loginUserDto.Password, user.PasswordHash))
        {
            return null!;
        }

        var role = roleDb.Documents.Select(r => r.ConvertTo<Role>()).ToList().Find(r => r.Id == user!.roleId);
        user.role = role;
        return user;
    }

    public UserDto GetUserByEmail(string email)
    {
        var userDb = base.GetSnapshots(_collectionUser);
        var user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == email);
        if (user == null)
        {
            return null!;
        }
        return userConverter.ToDto(user);
    }

    public async Task<User> VerifyUser(string email)
    {
        var userDb = base.GetSnapshots(_collectionUser);
        var user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == email);
        if (user == null)
        {
            return null!;
        }

        var unspecified = DateTime.UtcNow;
        var specified = DateTime.SpecifyKind(unspecified, DateTimeKind.Utc);
        DocumentReference docref = _firestoreDb.Collection(_collectionUser).Document(user.Id);
        Dictionary<string, object> data = new Dictionary<string, object>{
            {"VerifiedAt" , specified}
        };

        DocumentSnapshot snapshot = await docref.GetSnapshotAsync();
        if (snapshot.Exists)
        {
            await docref.UpdateAsync(data);
        }
        return user;
    }

    public bool checkIsVerified(string email)
    {
        var userDb = base.GetSnapshots(_collectionUser);
        var user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == email);
        if (user == null) return false;
        return user.VerifiedAt.ToDateTime().Year != 1111;
    }

    public bool checkValidToken(string email, string token)
    {
        var userDb = base.GetSnapshots(_collectionUser);
        var user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == email);
        return user!.VerificationToken == token;
    }

    public async Task<User> ForgetPasword(string email, string randomCode)
    {
        var userDb = base.GetSnapshots(_collectionUser);
        var user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == email);
        if (user == null)
        {
            return null!;
        }

        var unspecified = DateTime.UtcNow.AddDays(1);
        var specified = DateTime.SpecifyKind(unspecified, DateTimeKind.Utc);
        DocumentReference docref = _firestoreDb.Collection(_collectionUser).Document(user.Id);
        Dictionary<string, object> data = new Dictionary<string, object>{
            {"PasswordReestToken" , BCrypt.Net.BCrypt.HashPassword(randomCode)},
            {"ResetTokenExpires" , specified}
        };

        DocumentSnapshot snapshot = await docref.GetSnapshotAsync();
        if (snapshot.Exists)
        {
            await docref.UpdateAsync(data);
        }
        return user;
    }

    public async Task<User> ResetPassword(ResetPasswordDto resetPasswordDto)
    {
        var userDb = base.GetSnapshots(_collectionUser);
        var userList = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList();
        User user = null!;
        foreach (var u in userList)
        {
            if (u.PasswordReestToken != "")
            {
                if (BCrypt.Net.BCrypt.Verify(resetPasswordDto.Code, u.PasswordReestToken))
                {
                    user = u;
                    break;
                }
            }
        }

        if (user == null)
        {
            return null!;
        }

        var unspecified = new DateTime(1111, 11, 11, 11, 11, 11, DateTimeKind.Unspecified);
        var specified = DateTime.SpecifyKind(unspecified, DateTimeKind.Utc);
        DocumentReference docref = _firestoreDb.Collection(_collectionUser).Document(user.Id);
        Dictionary<string, object> data = new Dictionary<string, object>{
            {"PasswordHash" , BCrypt.Net.BCrypt.HashPassword(resetPasswordDto.Password)},
            {"PasswordReestToken" , ""},
            {"ResetTokenExpires" , Timestamp.FromDateTime(specified)}
        };

        DocumentSnapshot snapshot = await docref.GetSnapshotAsync();
        if (snapshot.Exists)
        {
            await docref.UpdateAsync(data);
        }
        return user;
    }
}
