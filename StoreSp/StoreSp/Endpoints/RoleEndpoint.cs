using StoreSp.Services;
using StoreSp.Stores.Stores;

namespace StoreSp.Endpoints;

public static class RoleEndpoint
{
    public static RoleFireStore? roleFireStore { get; set; }
    public static IRoleService? roleService { get; set; }
    public static RouteGroupBuilder MapRoleEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("roles");
        roleService = new RoleServiceImpl();

        group.MapPost("/", (CreateRoleDto createRoleDto) =>
        {
            return roleService.AddRole(createRoleDto, roleFireStore!);
        });

        group.MapGet("/", () =>
        {
            return roleService.GetAllRoles(roleFireStore!);
        });

        // group.MapGet("/{id}", (string id) =>
        // {
        //     // return userService.GetUserById(id, userFireStore!);
        // });

        return group;
    }
}
