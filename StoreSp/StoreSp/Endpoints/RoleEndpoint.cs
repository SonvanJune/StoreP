using Microsoft.AspNetCore.Mvc;
using StoreSp.Dtos.request;
using StoreSp.Services;
using StoreSp.Services.Impl;

namespace StoreSp.Endpoints;

public static class RoleEndpoint
{
    public static IRoleService? roleService { get; set; }
    public static IAuthService? authService { get; set; }
    
    public static RouteGroupBuilder MapRoleEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/roles");
        roleService = new RoleServiceImpl();
        authService = new AuthServiceImpl();

        group.MapPost("/", (CreateRoleDto createRoleDto , [FromHeader] string authorization) =>
        {
            return authService.GetResult(authorization, roleService.AddRole(createRoleDto));
        }).WithParameterValidation();

        group.MapGet("/", ([FromHeader] string authorization) =>
        {
            return authService.GetResult(authorization, roleService.GetAllRoles());
        }).RequireAuthorization("quan-tri-vien");

        return group;
    }
}
