using Microsoft.AspNetCore.Mvc;
using StoreSp.Dtos.request;
using StoreSp.Services;
using StoreSp.Services.Impl;

namespace StoreSp.Endpoints;

public static class UploadEndpoint
{
    public static IUploadService? UploadService { get; set; }
    public static IAuthService? authService { get; set; }

    public static RouteGroupBuilder MapUploadEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/uploads");
        UploadService = new UploadServiceImpl();
        authService = new AuthServiceImpl();

        group.MapPost("/", (UploadFilesDto request , [FromHeader] string authorization) =>
        {
            return authService.GetResult(authorization, UploadService.UploadFiles(request));
        }).WithParameterValidation();
        return group;
    }
}
