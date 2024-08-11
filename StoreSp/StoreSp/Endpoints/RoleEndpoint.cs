using System.Net;
using Microsoft.AspNetCore.Mvc;
using StoreSp.Commonds;
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
            if (authService.GetResult(authorization) == 1)
            {
                return roleService.AddRole(createRoleDto);
            }
            else
            {
                return Results.BadRequest(new HttpStatusConfig
                {
                    status = HttpStatusCode.BadRequest,
                    message = "Token has expired",
                    data = null
                });
            }
        }).WithParameterValidation();

        group.MapGet("/", ([FromHeader] string authorization) =>
        {
            if (authService.GetResult(authorization) == 1)
            {
                return roleService.GetAllRoles();
            }
            else
            {
                return Results.BadRequest(new HttpStatusConfig
                {
                    status = HttpStatusCode.BadRequest,
                    message = "Token has expired",
                    data = null
                });
            }
        }).RequireAuthorization("quan-tri-vien");

        return group;
    }
}
