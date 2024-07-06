using Google.Cloud.Firestore;
using StoreSp.Converters;
using StoreSp.Entities;

namespace StoreSp.Stores.Stores;

public class RoleFireStore : FirestoreService
{
    public RoleFireStore(FirestoreDb firestoreDb) : base(firestoreDb){}
    private const string _collectionRole = "Roles";
    private IBaseConverter<Role, CreateRoleDto> createRoleConverter = new CreateRoleConverter();

    public Task Add(CreateRoleDto roleDto)
    {
        var roleDb = _firestoreDb.Collection(_collectionRole);
        var role = createRoleConverter.ToEntity(roleDto);
        return roleDb.AddAsync(role);
    }
}
