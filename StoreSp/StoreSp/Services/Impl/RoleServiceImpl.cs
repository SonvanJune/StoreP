using System.Net;
using StoreSp.Commonds;
using StoreSp.Dtos.request;
using StoreSp.Stores.Stores;

namespace StoreSp.Services.Impl;

public class RoleServiceImpl : IRoleService
{
    public static RoleFireStore? roleFireStore { get; set; }

    public IResult AddRole(CreateRoleDto createRoleDto)
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

    public IResult GetAllRoles()
    {
        return Results.Ok(new HttpStatusConfig
        {
            status = HttpStatusCode.OK,
            message = "Success",
            data = roleFireStore!.GetAllRoles().Result
        });
    }
}
