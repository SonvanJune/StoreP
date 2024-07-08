using System.Net;
using StoreSp.Commons;
using StoreSp.Services;
using StoreSp.Stores.Stores;

namespace StoreSp;

public class RoleServiceImpl: IRoleService
{
    public IResult AddRole(CreateRoleDto createRoleDto, RoleFireStore roleFireStore)
    {
        if (roleFireStore is null)
        {
            return Results.NoContent();
        }

        roleFireStore!.Add(createRoleDto);

        return Results.Created("", new HttpStatusConfig
        {
            status = HttpStatusCode.Created,
            message = "Created Success",
            data = null
        });
    }

    public IResult GetAllRoles(RoleFireStore roleFireStore)
    {
        return Results.Ok(new HttpStatusConfig
        {
            status = HttpStatusCode.OK,
            message = "Success",
            data = roleFireStore!.GetAllRoles().Result
        });
    }
}
