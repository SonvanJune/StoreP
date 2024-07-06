using Google.Cloud.Firestore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using StoreSp.Converters;
using StoreSp.Converters.response;
using StoreSp.Dtos.request;
using StoreSp.Dtos.response;
using StoreSp.Entities;
using StoreSp.Stores;

namespace StoreSp;

public class UserFireStore : FirestoreService
{
    private const string _collectionUser = "Users";
    private const string _collectionRole = "Roles";
    private IBaseConverter<User, UserDto> userConverter = new UserConverter();
    private IBaseConverter<User, CreateUserDto> createUserConverter = new CreateUserConverter();

    public UserFireStore(FirestoreDb firestoreDb) : base(firestoreDb) { }

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
}
