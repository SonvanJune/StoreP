using StoreSp.Dtos.request;
using StoreSp.Stores.Stores;

namespace StoreSp.Services;

public interface IRoleService
{
    public IResult AddRole(CreateRoleDto createRoleDto);
    public IResult GetAllRoles();
}
