using StoreSp.Dtos.request;
using StoreSp.Services;
using StoreSp.Services.Impl;
using StoreSp.Stores.Stores;

namespace StoreSp.Endpoints;

public static class RoleEndpoint
{
    public static IRoleService? roleService { get; set; }
    
    public static RouteGroupBuilder MapRoleEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/roles");
        roleService = new RoleServiceImpl();

        group.MapPost("/", (CreateRoleDto createRoleDto) =>
        {
            return roleService.AddRole(createRoleDto);
        }).WithParameterValidation();

        group.MapGet("/", () =>
        {
            return roleService.GetAllRoles();
        });

        // group.MapGet("/{id}", (string id) =>
        // {
        //     // return userService.GetUserById(id, userFireStore!);
        // });

        return group;
    }
}
