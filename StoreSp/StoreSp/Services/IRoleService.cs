using StoreSp.Stores.Stores;

namespace StoreSp.Services;

public interface IRoleService
{
    public IResult AddRole(CreateRoleDto createRoleDto, RoleFireStore userFireStore);
}
