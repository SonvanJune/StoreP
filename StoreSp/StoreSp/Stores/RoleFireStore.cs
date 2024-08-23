using Google.Cloud.Firestore;
using StoreSp.Context;
using StoreSp.Converters;
using StoreSp.Converters.request;
using StoreSp.Converters.response;
using StoreSp.Dtos.request;
using StoreSp.Dtos.response;
using StoreSp.Models;

namespace StoreSp.Stores.Stores;

public class RoleFireStore
{
    private readonly AppDbContext? _appDbContext = null;

    public RoleFireStore()
    {
        _appDbContext = AppDbContext.GetInstance();
    }
    private readonly IBaseConverter<Role, CreateRoleDto> createRoleConverter = new CreateRoleConverter();
    private readonly IBaseConverter<Role,RoleDto> roleConverter = new RoleConverter();

    public async Task Add(CreateRoleDto roleDto)
    {
        var role = createRoleConverter.ToEntity(roleDto);
        _appDbContext!.Roles.Add(role);
        await _appDbContext!.SaveChangesAsync();
        return;
    }

    public Task<List<RoleDto>> GetAllRoles()
    {
        var role = _appDbContext!.Roles.ToList();
        return Task.FromResult(role.Select(roleConverter.ToDto).ToList());
    }

}
