using Google.Cloud.Firestore;
using StoreSp.Converters;
using StoreSp.Converters.response;
using StoreSp.Dtos.response;
using StoreSp.Entities;
using StoreSp.Stores;

namespace StoreSp;

public class UserFireStore:FirestoreService
{
    private const string _collectionName = "Users";
    private IBaseConverter<User,UserDto> userConverter = new UserConverter();

    public UserFireStore(FirestoreDb firestoreDb) : base(firestoreDb){}

    public Task<List<UserDto>> GetAllUser()
    {
        var snapshot = base.GetSnapshots(_collectionName);
        var user = snapshot.Documents.Select(s => s.ConvertTo<User>()).ToList();
        return Task.FromResult(user.Select(userConverter.ToDto).ToList());
    }

    public Task<UserDto> GetUser(string id)
    {
        var snapshot = base.GetSnapshots(_collectionName);
        var user = snapshot.Documents.Select(s => s.ConvertTo<User>()).ToList().Find(u => u.Id == id);
        return Task.FromResult(userConverter.ToDto(user!));
    }


    public Task Add(UserDto userDto)
    {
        var collection = _firestoreDb.Collection(_collectionName);
        var user = userConverter.ToEntity(userDto);
        return collection.AddAsync(user);
    }
}
