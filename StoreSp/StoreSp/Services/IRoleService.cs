using StoreSp.Dtos.request;

namespace StoreSp.Services;

public interface IRoleService
{
    public IResult AddRole(CreateRoleDto createRoleDto);
    public IResult GetAllRoles();
}
