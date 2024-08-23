using System.Net;
using StoreSp.Commonds;
using StoreSp.Dtos.request;
using StoreSp.Stores.Stores;

namespace StoreSp.Services.Impl;

public class RoleServiceImpl : IRoleService
{
    public static RoleFireStore roleFireStore = new RoleFireStore();

    public IResult AddRole(CreateRoleDto createRoleDto)
    {
        if (roleFireStore is null)
        {
            return Results.NoContent();
        }

        var a = roleFireStore!.Add(createRoleDto);

        return Results.Created("", new HttpStatusConfig
        {
            status = HttpStatusCode.Created,
            message = "Tạo thành công",
            data = null
        });
    }

    public IResult GetAllRoles()
    {
        return Results.Ok(new HttpStatusConfig
        {
            status = HttpStatusCode.OK,
            message = "Thành công",
            data = roleFireStore!.GetAllRoles().Result
        });
    }
}
