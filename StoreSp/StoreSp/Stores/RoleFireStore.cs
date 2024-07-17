using Google.Cloud.Firestore;
using StoreSp.Converters;
using StoreSp.Converters.request;
using StoreSp.Converters.response;
using StoreSp.Dtos.request;
using StoreSp.Dtos.response;
using StoreSp.Entities;

namespace StoreSp.Stores.Stores;

public class RoleFireStore(FirestoreDb firestoreDb) : FirestoreService(firestoreDb)
{
    public static string _collectionRole = "Roles";
    private readonly IBaseConverter<Role, CreateRoleDto> createRoleConverter = new CreateRoleConverter();
    private readonly IBaseConverter<Role,RoleDto> roleConverter = new RoleConverter();

    public Task Add(CreateRoleDto roleDto)
    {
        var roleDb = _firestoreDb.Collection(_collectionRole);
        var role = createRoleConverter.ToEntity(roleDto);
        return roleDb.AddAsync(role);
    }

    public Task<List<RoleDto>> GetAllRoles()
    {
        var snapshot = base.GetSnapshots(_collectionRole);
        var role = snapshot.Documents.Select(s => s.ConvertTo<Role>()).ToList();
        return Task.FromResult(role.Select(roleConverter.ToDto).ToList());
    }

}
